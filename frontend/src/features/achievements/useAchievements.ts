import { useState, useEffect, useRef, useCallback } from "react";
import { useAuth } from "@clerk/react";
import type { FetchError } from "@/types/fetchError";
import type { UserAchievement } from "./types/achievement";

export function useAchievements(): {
  achievements: UserAchievement[];
  isLoading: boolean;
  error: FetchError | null;
} {
  const [achievements, setAchievements] = useState<UserAchievement[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<FetchError | null>(null);
  const abortRef = useRef<AbortController | null>(null);
  const { getToken, isSignedIn } = useAuth();

  const  ACHIEVEMENTS_URL= import.meta.env.VITE_API_URL + "/achievements";

  const fetchSessions = useCallback(async () => {
    // Abort any in-flight request
    if (abortRef.current) abortRef.current.abort();
    const controller = new AbortController();
    abortRef.current = controller;
    setIsLoading(true);
    setError(null);

    try {
      const token = await getToken({
        template: "jwt-basketball-progress-tracker",
      });

      try {
        const response = await fetch(`${ACHIEVEMENTS_URL}`, {
          headers: {
            Authorization: `Bearer ${token}`,
          },
          signal: controller.signal,
        });
        if (!response.ok) {
          const errData = await response.json().catch(() => ({}));
          throw Object.assign(
            new Error(errData.message || response.statusText),
            {
              status: response.status,
            },
          );
        }
        const result: UserAchievement[] = await response.json();
        setAchievements(result);
      } catch (err: unknown) {
        const error = err as FetchError;
        if (error.name !== "AbortError") {
          setError({
            message: error.message,
            status: error.status,
          } as FetchError);
        }
      } finally {
        if (abortRef.current === controller) {
          // make sure we only set isLoading to false if this is the latest request and not the old request
          setIsLoading(false);
        }
      }
    } catch (error) {
      setError({
        message: "Failed to get auth token",
        status: 401,
      } as FetchError);
    }
  }, [ACHIEVEMENTS_URL, getToken]);

  useEffect(() => {
    if (!isSignedIn) return;
    fetchSessions();
    return () => abortRef.current?.abort();
  }, [fetchSessions, isSignedIn]);

  return { achievements, isLoading, error, };
}