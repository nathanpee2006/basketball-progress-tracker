import type { Consistency } from "./types.ts/consistency";
import type { FetchError } from "@/types/fetchError";
import { useCallback, useEffect, useRef, useState } from "react";
import { useAuth } from "@clerk/react";

export function useConsistency(): {
  consistency: Consistency | null;
  isLoading: boolean;
  error: FetchError | null;
  refetch: () => Promise<void>;
} {
  const [consistency, setConsistency] = useState<Consistency | null>(null);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<FetchError | null>(null);
  const abortRef = useRef<AbortController | null>(null);
  const { getToken } = useAuth();

  const CONSISTENCY_URL = import.meta.env.VITE_API_URL + "/analytics/consistency";

  const fetchConsistency = useCallback(async (): Promise<void> => {
    abortRef.current?.abort();
    const controller = new AbortController();
    abortRef.current = controller;
    setIsLoading(true);
    setError(null);

    let token: string | null;
    try {
      token = await getToken({
        template: "jwt-basketball-progress-tracker",
      });
    } catch {
      const authError = {
        message: "Failed to get auth token",
        status: 401,
      } as FetchError;
      setIsLoading(false);
      setError(authError);
      throw authError;
    }

    try {
      const response = await fetch(CONSISTENCY_URL, {
        headers: {
          Authorization: `Bearer ${token}`,
          "Content-Type": "application/json",
        },
        signal: controller.signal,
      });
      if (!response.ok) {
        const errData = await response.json().catch(() => ({}));
        throw Object.assign(
          new Error(errData.message || response.statusText),
          { status: response.status },
        );
      }
      const result: Consistency = await response.json();
      setConsistency(result);
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
        setIsLoading(false);
      }
    }
  }, [CONSISTENCY_URL, getToken]);

  useEffect(() => {
    fetchConsistency();
    return () => abortRef.current?.abort();
  }, [fetchConsistency, getToken]);

  return { consistency, isLoading, error, refetch: fetchConsistency };
}
