import type { FetchError } from "@/types/fetchError";
import { useCallback, useEffect, useRef, useState } from "react";
import { useAuth } from "@clerk/react";
import { format } from "date-fns";
import type { DateRange } from "react-day-picker";
import type { ShootingAnalyticsResponse } from "./types/analytics";

export function useShootingAnalytics(): {
  analytics: ShootingAnalyticsResponse | null;
  isLoading: boolean;
  error: FetchError | null;
  dateRange: DateRange | undefined;
  setDateRange: (range: DateRange | undefined) => void;
  refetch: () => Promise<void>;
} {
  const [analytics, setAnalytics] = useState<ShootingAnalyticsResponse | null>(null);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<FetchError | null>(null);
  const [dateRange, setDateRange] = useState<DateRange | undefined>(undefined);
  const abortRef = useRef<AbortController | null>(null);
  const { getToken } = useAuth();

  const ANALYTICS_BASE_URL =
    import.meta.env.VITE_API_URL + "/analytics/shooting";

  const fetchAnalytics = useCallback(async (): Promise<void> => {
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
    if (dateRange?.from) params.set("from", format(dateRange.from, "yyyy-MM-dd"));
    if (dateRange?.to) params.set("to", format(dateRange.to, "yyyy-MM-dd"));
    const url = params.toString()
      ? `${ANALYTICS_BASE_URL}?${params.toString()}`
      : ANALYTICS_BASE_URL;

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
      const result: ShootingAnalyticsResponse = await response.json();
      setAnalytics(result);
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
  }, [ANALYTICS_BASE_URL, getToken, dateRange]);

  useEffect(() => {
    fetchAnalytics();
    return () => abortRef.current?.abort();
  }, [fetchAnalytics, getToken]);

  return { analytics, isLoading, error, dateRange, setDateRange, refetch: fetchAnalytics };
}