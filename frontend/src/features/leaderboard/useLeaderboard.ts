import type { FetchError } from "@/types/fetchError";
import { useCallback, useEffect, useRef, useState } from "react";
import { useAuth } from "@clerk/react";
import type { LeaderboardEntry } from "./types/leaderboard-entry";
import type { ZoneId } from "@/types/court";

export function useLeaderboard(): {
  leaderboard: LeaderboardEntry[];
  isLoading: boolean;
  setZone: (zone: ZoneId | null) => void;
  error: FetchError | null;
  refetch: () => Promise<void>;
} {
  const [leaderboard, setLeaderboard] = useState<LeaderboardEntry[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<FetchError | null>(null);
  const [zone, setZone] = useState<ZoneId | null>(null);
  const abortRef = useRef<AbortController | null>(null);
  const { getToken } = useAuth();

  const LEADERBOARD_URL = import.meta.env.VITE_API_URL + "/leaderboard";

  const fetchLeaderboard = useCallback(async (): Promise<void> => {
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
      if (abortRef.current === controller) {
        setIsLoading(false);
      }
      setError(authError);
      return;
    }

    const params = new URLSearchParams();
    if (zone) params.set("zone", zone);
    const url = params.toString()
      ? `${LEADERBOARD_URL}?${params.toString()}`
      : LEADERBOARD_URL;

    try {
      const response = await fetch(url, {
        headers: {
          Authorization: `Bearer ${token}`,
          "Content-Type": "application/json",
        },
        signal: controller.signal,
      });

      if (!response.ok) {
        const errData = await response.json().catch(() => ({}));
        throw Object.assign(new Error(errData.message || response.statusText), {
          status: response.status,
        });
      }

      const result: LeaderboardEntry[] = await response.json();
      setLeaderboard(result);
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
  }, [LEADERBOARD_URL, getToken, zone]);

  useEffect(() => {
    fetchLeaderboard();
    return () => abortRef.current?.abort();
  }, [fetchLeaderboard]);

  return { leaderboard, isLoading, error, setZone, refetch: fetchLeaderboard };
}
