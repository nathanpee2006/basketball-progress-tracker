import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Progress } from "@/components/ui/progress";
import { ZONE_LABELS, type AnalyticsZoneStat } from "../types/analytics";

interface ZonePercentageCardProps {
  zones: AnalyticsZoneStat[];
}

export function ZonePercentageCard({ zones }: ZonePercentageCardProps) {
  return (
    <Card>
      <CardHeader className="pb-2">
        <CardTitle className="text-sm font-medium text-muted-foreground">
          Career Shooting % by Zone
        </CardTitle>
      </CardHeader>
      <CardContent className="space-y-4">
        {zones.map((zone) => (
          <div key={zone.zone} className="space-y-1">
            <div className="flex items-center justify-between text-sm">
              <span className="font-medium">{ZONE_LABELS[zone.zone] ?? zone.zone}</span>
              <span className="text-muted-foreground">
                {zone.percentage}% ({zone.makes}/{zone.attempts})
              </span>
            </div>
            {/* Progress value is the zone's own make percentage, scale is fixed 0-100 */}
            <Progress value={zone.percentage} />
          </div>
        ))}
      </CardContent>
    </Card>
  );
}