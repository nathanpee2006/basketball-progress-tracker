import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { ZONE_LABELS, type AnalyticsZoneStat } from "../types/analytics";
import { Progress } from "@/components/ui/progress";

interface ZoneVolumeCardProps {
  zones: AnalyticsZoneStat[];
}

export function ZoneVolumeCard({ zones }: ZoneVolumeCardProps) {
  // Relative volume, not percentage: each bar is scaled against the busiest zone,
  // not against 100. A zone with the most attempts always reads as a full bar.
  const maxAttempts = Math.max(...zones.map((z) => z.attempts), 1);

  return (
    <Card>
      <CardHeader className="pb-2">
        <CardTitle className="text-sm font-medium text-muted-foreground">
          Shot Volume by Zone
        </CardTitle>
      </CardHeader>
      <CardContent className="space-y-4">
        {zones.map((zone) => (
          <div key={zone.zone} className="space-y-1">
            <div className="flex items-center justify-between text-sm">
              <span className="font-medium">{ZONE_LABELS[zone.zone] ?? zone.zone}</span>
              <span className="text-muted-foreground">{zone.attempts} attempts</span>
            </div>
            <Progress value={(zone.attempts / maxAttempts) * 100} />
          </div>
        ))}
      </CardContent>
    </Card>
  );
}