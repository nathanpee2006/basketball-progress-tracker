import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Separator } from "@/components/ui/separator";
import type { SessionDetail } from "../../types/session";

interface StatRowProps {
  label: string;
  makes: number;
  attempts: number;
  percentage: number;
}

function StatRow({ label, makes, attempts, percentage }: StatRowProps) {
  return (
    <div className="flex items-center justify-between py-2 text-sm">
      <span className="text-muted-foreground">{label}</span>
      <span>
        {makes}/{attempts} · {percentage}%
      </span>
    </div>
  );
}

interface SessionStatsBreakdownProps {
  session: SessionDetail;
}

export function SessionStatsBreakdown({ session }: SessionStatsBreakdownProps) {
  return (
    <Card>
      <CardHeader>
        <CardTitle>Shot Breakdown</CardTitle>
      </CardHeader>
      <CardContent>
        <StatRow
          label="Paint"
          makes={session.paintMakes}
          attempts={session.paintAttempts}
          percentage={session.paintShotPercentage}
        />
        <Separator />
        <StatRow
          label="Midrange"
          makes={session.midrangeMakes}
          attempts={session.midrangeAttempts}
          percentage={session.midrangeShotPercentage}
        />
        <Separator />
        <StatRow
          label="Three Point"
          makes={session.threePointMakes}
          attempts={session.threePointAttempts}
          percentage={session.threePointShotPercentage}
        />
        <Separator />
        <StatRow
          label="Free Throw"
          makes={session.freeThrowMakes}
          attempts={session.freeThrowAttempts}
          percentage={session.freeThrowShotPercentage}
        />
        <Separator />
        <StatRow
          label="Overall"
          makes={session.overallMakes}
          attempts={session.overallAttempts}
          percentage={session.overallShotPercentage}
        />
      </CardContent>
    </Card>
  );
}