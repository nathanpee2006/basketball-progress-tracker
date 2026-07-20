import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import type { Drill } from "@/types/session";

function formatDuration(seconds: number): string {
  const mins = Math.floor(seconds / 60);
  const secs = seconds % 60;
  return `${mins}:${secs.toString().padStart(2, "0")}`;
}

interface SessionDrillListProps {
  drills: Drill[];
}

export function SessionDrillList({ drills }: SessionDrillListProps) {
  if (drills.length === 0) {
    return null; // no drills recorded — omit the card rather than show an empty one
  }

  return (
    <Card>
      <CardHeader>
        <CardTitle>Drills</CardTitle>
      </CardHeader>
      <CardContent className="space-y-1">
        {drills.map((drill) => (
          <div key={drill.id} className="flex items-center justify-between text-sm">
            <span>{drill.name}</span>
            <span className="text-muted-foreground">
              {formatDuration(drill.completionTimeInSeconds)}
            </span>
          </div>
        ))}
      </CardContent>
    </Card>
  );
}