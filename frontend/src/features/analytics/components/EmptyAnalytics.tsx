import { BarChart3 } from "lucide-react";
import { Card, CardContent } from "@/components/ui/card";

export function EmptyAnalytics() {
  return (
    <Card>
      <CardContent className="flex flex-col items-center gap-2 py-12 text-center">
        <BarChart3 className="h-8 w-8 text-muted-foreground" />
        <p className="text-sm text-muted-foreground">Log sessions to see analytics</p>
      </CardContent>
    </Card>
  );
}