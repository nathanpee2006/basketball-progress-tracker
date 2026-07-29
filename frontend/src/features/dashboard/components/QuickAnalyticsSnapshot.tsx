import { Skeleton } from "@/components/ui/skeleton";
import type { Consistency } from "../types/consistency";
import { MetricCard } from "@/components/analytics/MetricCard";

interface QuickAnalyticsSnapshotProps {
  consistency: Consistency | null;
  isLoading: boolean;
}

export function QuickAnalyticsSnapshot({
  consistency,
  isLoading,
}: QuickAnalyticsSnapshotProps) {
  if (isLoading) {
    return (
      <div className="grid grid-cols-2 gap-4 sm:grid-cols-3">
        <Skeleton className="h-24 w-full" />
        <Skeleton className="h-24 w-full" />
        <Skeleton className="h-24 w-full" />
      </div>
    );
  }

  return (
    <div className="grid grid-cols-2 gap-4 sm:grid-cols-3">
      <MetricCard label="Total Sessions" value={consistency?.totalSessions ?? 0} />
      <MetricCard
        label="Avg / Week"
        value={(consistency?.avgSessionsPerWeek ?? 0).toFixed(1)}
      />
      <MetricCard
        label="Avg / Month"
        value={(consistency?.avgSessionsPerMonth ?? 0).toFixed(1)}
      />
    </div>
  );
}