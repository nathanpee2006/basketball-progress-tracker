import type { SessionDetail } from "@/types/session";
import type { SessionFormValues } from "./schemas/sessionFormSchema";
import type { FetchError } from "@/types/fetchError";
import { useCallback, useEffect, useRef, useState } from "react";
import { useAuth } from "@clerk/react";

export function useCreateSession(): {
  createSession: (data: SessionFormValues) => Promise<SessionDetail>;
  isLoading: boolean;
  error: FetchError | null;
} {
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<FetchError | null>(null);
  const abortRef = useRef<AbortController | null>(null);
  const { getToken } = useAuth();

  const SESSIONS_URL = import.meta.env.VITE_API_URL + "/sessions";

  const createSession = useCallback(
    async (data: SessionFormValues): Promise<SessionDetail> => {
      const controller = new AbortController();
      abortRef.current = controller;
      setIsLoading(true);
      setError(null);

      try {
        const token = await getToken({
          template: "jwt-testing-template",
        });

        try {
          const response = await fetch(SESSIONS_URL, {
            method: "POST",
            headers: {
              Authorization: `Bearer ${token}`,
              "Content-Type": "application/json",
            },
            body: JSON.stringify(data),
            signal: controller.signal,
          });
          if (!response.ok) {
            const errData = await response.json().catch(() => ({}));
            throw Object.assign(
              new Error(errData.message || response.statusText),
              { status: response.status },
            );
          }
          const result: SessionDetail = await response.json();
          return result;
        } catch (err: unknown) {
          const error = err as FetchError;
          if (error.name !== "AbortError") {
            setError({
              message: error.message,
              status: error.status,
            } as FetchError);
          }
          throw error;
        } finally {
          if (abortRef.current === controller) {
            setIsLoading(false);
          }
        }
      } catch (error) {
        const authError = {
          message: "Failed to get auth token",
          status: 401,
        } as FetchError;
        setError(authError);
        throw authError;
      }
    },
    [SESSIONS_URL, getToken],
  );

  useEffect(() => {
    return () => abortRef.current?.abort();
  }, []);

  return { createSession, isLoading, error };
}