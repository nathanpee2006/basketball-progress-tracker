import { useState, useEffect, useRef, useCallback } from "react";
import { useAuth } from "@clerk/react";
import type { Session } from "@/types/session";
import type { FetchError } from "@/types/fetchError";

export function useSessions(): {
  sessions: Session[];
  isLoading: boolean;
  error: FetchError | null;
} {
  const [sessions, setSessions] = useState<Session[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<FetchError | null>(null);
  const abortRef = useRef<AbortController | null>(null);
  const { getToken, isSignedIn } = useAuth();

  const SESSIONS_URL = import.meta.env.VITE_API_URL + "/sessions";

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
        const response = await fetch(`${SESSIONS_URL}`, {
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
        const result : Session[] = await response.json();
        setSessions(result);
      } catch (err: unknown) {
        const error = err as FetchError;
        if (error.name !== "AbortError") {
          setError({
            message: error.message,
            status: error.status,
          } as FetchError);
        }
      } finally {
        setIsLoading(false);
      }
    } catch (error) {
      setError({
        message: "Failed to get auth token",
        status: 401,
      } as FetchError);
    }
  }, [SESSIONS_URL]);

  useEffect(() => {
    if (!isSignedIn) return;
    fetchSessions();
    return () => abortRef.current?.abort();
  }, [fetchSessions, isSignedIn]);

  return { sessions, isLoading, error };
}
