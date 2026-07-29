import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { AnalyticsSkeleton } from "./components/AnalyticsSkeleton";
import { EmptyAnalytics } from "./components/EmptyAnalytics";
import { MetricCard } from "@/components/analytics/MetricCard";
import { ZonePercentageCard } from "./components/ZonePercentageCard";
import { ZoneVolumeCard } from "./components/ZoneVolumeCard";
import { Sparkline } from "./components/Sparkline";
import { DateRangePicker } from "./components/DateRangePicker";
import { useShootingAnalytics } from "./useShootingAnalytics";
import { ZONE_LABELS } from "./types/analytics";

export function AnalyticsPage() {
  const { analytics, isLoading, error, dateRange, setDateRange } = useShootingAnalytics();

  // "No data" = no sessions in the selected range, not merely a zero percentage
  // (a real 0% zone should still render its bar, not trigger the empty state).
  const hasNoData = !isLoading && !error && analytics !== null && analytics.shootingByZonePerSession.length === 0;

  return (
    <section className="space-y-4">
      <div className="flex flex-col gap-2 sm:flex-row sm:items-center sm:justify-between">
        <div className="space-y-2">
          <h2 className="text-xl font-semibold">Analytics</h2>
          <p className="text-sm text-muted-foreground">
            Metrics and trends for zones, free throws, drills, and consistency.
          </p>
        </div>
        <DateRangePicker value={dateRange} onChange={setDateRange} />
      </div>

      {isLoading && <AnalyticsSkeleton />}

      {!isLoading && error && (
        <Card>
          <CardContent className="py-8 text-center text-sm text-muted-foreground">
            {error.message}
          </CardContent>
        </Card>
      )}

      {!isLoading && !error && hasNoData && <EmptyAnalytics />}

      {!isLoading && !error && analytics && !hasNoData && (
        <>
          <div className="grid gap-4 sm:grid-cols-2">
            <MetricCard
              label="Weakest Zone"
              value={
                  ZONE_LABELS[analytics.weakestShootingZone] ?? analytics.weakestShootingZone
              }
            />
            <MetricCard label="Free Throw %" value={`${analytics.freeThrowPercentage}%`} />
          </div>

          <ZonePercentageCard zones={analytics.shootingByZone} />

          <Card>
            <CardHeader className="pb-2">
              <CardTitle className="text-sm font-medium text-muted-foreground">
                Free Throw % Trend
              </CardTitle>
            </CardHeader>
            <CardContent>
              <Sparkline
                points={analytics.freeThrowTrend.map((p) => ({
                  date: p.date,
                  value: p.percentage,
                  label: `${p.makes}/${p.attempts} (${p.percentage}%)`,
                }))}
              />
            </CardContent>
          </Card>

          <ZoneVolumeCard zones={analytics.shootingByZone} />
        </>
      )}
    </section>
  );
}
