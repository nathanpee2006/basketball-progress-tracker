import { useState, useEffect, useRef, useCallback } from "react";
import { useAuth } from "@clerk/react";
import type { SessionDetail } from "@/types/session";
import type { FetchError } from "@/types/fetchError";

export function useSession(sessionId: number): {
  session: SessionDetail | null;
  isLoading: boolean;
  error: FetchError | null;
} {
  const [session, setSession] = useState<SessionDetail | null>(null);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<FetchError | null>(null);
  const abortRef = useRef<AbortController | null>(null);
  const { getToken, isSignedIn } = useAuth();

  const SESSION_URL = import.meta.env.VITE_API_URL + "/sessions/" + sessionId;

  const fetchSessionById = useCallback(async () => {
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
        const response = await fetch(`${SESSION_URL}`, {
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
        const result: SessionDetail = await response.json();
        setSession(result);
      } catch (err: unknown) {
        const error = err as FetchError;
        if (error.name !== "AbortError") {
          setError({
            message: error.message,
            status: error.status,
          } as FetchError);
        }
      } finally {
        if (abortRef.current === controller) { // make sure we only set isLoading to false if this is the latest request and not the old request
          setIsLoading(false);
        }
      }
    } catch (error) {
      setError({
        message: "Failed to get auth token",
        status: 401,
      } as FetchError);
    }
  }, [SESSION_URL, getToken]);

  useEffect(() => {
    if (!isSignedIn) return;
    if (sessionId) {
      fetchSessionById();
    }
    return () => abortRef.current?.abort();
  }, [sessionId, fetchSessionById, isSignedIn]);

  return { session, isLoading, error };
}
