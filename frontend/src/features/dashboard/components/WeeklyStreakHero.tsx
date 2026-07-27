import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { Skeleton } from "@/components/ui/skeleton";
import { Flame } from "lucide-react";
import type { Consistency } from "../types.ts/consistency";

interface WeeklyStreakHeroProps {
  consistency: Consistency | null;
  isLoading: boolean;
}

export function WeeklyStreakHero({ consistency, isLoading }: WeeklyStreakHeroProps) {
  const isActiveThisWeek = (consistency?.currentStreakWeeks ?? 0) > 0;

  return (
    <Card>
      <CardHeader className="flex flex-row items-center justify-between">
        <CardTitle className="flex items-center gap-2 text-xl">
          <Flame className="h-5 w-5 text-orange-500" />
          Weekly Streak
        </CardTitle>
        {isLoading ? (
          <Skeleton className="h-6 w-28" />
        ) : (
          <Badge variant={isActiveThisWeek ? "secondary" : "outline"}>
            {isActiveThisWeek ? "Active this week" : "No sessions this week"}
          </Badge>
        )}
      </CardHeader>
      <CardContent>
        {isLoading ? (
          <Skeleton className="h-10 w-40" />
        ) : (
          <>
            <div className="flex items-baseline gap-2">
              <span className="text-4xl font-bold">
                {consistency?.currentStreakWeeks ?? 0}
              </span>
              <span className="text-muted-foreground">
                week{consistency?.currentStreakWeeks === 1 ? "" : "s"} in a row
              </span>
            </div>
            <p className="mt-1 text-sm text-muted-foreground">
              Longest streak: {consistency?.longestStreakWeeks ?? 0} weeks
            </p>
          </>
        )}
      </CardContent>
    </Card>
  );
}