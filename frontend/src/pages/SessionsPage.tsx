import { MoreVertical, ClipboardList } from "lucide-react";
import { useSessions } from "@/hooks/useSessions";
import {
  Card,
  CardAction,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { Skeleton } from "@/components/ui/skeleton";
import { Button } from "@/components/ui/button";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import {
  Empty,
  EmptyHeader,
  EmptyMedia,
  EmptyTitle,
  EmptyDescription,
  EmptyContent,
} from "@/components/ui/empty";
import { toast } from "sonner";

export function SessionsPage() {
  const { sessions, isLoading, error } = useSessions();

  return (
    <section className="space-y-2">
      <h2 className="text-xl font-semibold">Sessions</h2>
      <p className="text-sm text-muted-foreground">
        Session history list with view, edit, and delete actions.
      </p>

      {isLoading && (
        <div className="space-y-2">
          {Array.from({ length: 5 }).map((_, i) => (
            <Card key={i} className="w-full">
              <CardHeader>
                <Skeleton className="h-4 w-1/3" />
                <Skeleton className="h-3 w-2/3" />
              </CardHeader>
            </Card>
          ))}
        </div>
      )}

      {error && toast.error(error.message)}

      {!isLoading && !error && sessions.length === 0 && (
        <Empty>
          <EmptyHeader>
            <EmptyMedia variant="icon">
              <ClipboardList />
            </EmptyMedia>
            <EmptyTitle>No sessions yet</EmptyTitle>
            <EmptyDescription>
              Track your progress by logging your first session.
            </EmptyDescription>
          </EmptyHeader>
          <EmptyContent>
            <Button>Log your first session</Button>
          </EmptyContent>
        </Empty>
      )}

      {!isLoading &&
        sessions.map((session) => (
          <Card key={session.id} className="w-full">
            <CardHeader>
              <CardTitle>
                {new Date(session.date).toLocaleDateString()}
              </CardTitle>
              <CardDescription>
                {`Paint ${session.paintShotPercentage}% · Mid ${session.midrangeShotPercentage}% · 3PT ${session.threePointShotPercentage}% · FT ${session.freeThrowShotPercentage}% . Overall ${session.overallShotPercentage}%`}
              </CardDescription>
              <CardAction>
                <DropdownMenu>
                  <DropdownMenuTrigger
                    render={
                      <Button variant="ghost" size="icon">
                        <MoreVertical className="h-4 w-4" />
                      </Button>
                    }
                  ></DropdownMenuTrigger>
                  <DropdownMenuContent align="end">
                    <DropdownMenuItem>View</DropdownMenuItem>
                    <DropdownMenuItem>Edit</DropdownMenuItem>
                    <DropdownMenuItem variant="destructive">
                      Delete
                    </DropdownMenuItem>
                  </DropdownMenuContent>
                </DropdownMenu>
              </CardAction>
            </CardHeader>
            <CardContent />
          </Card>
        ))}
    </section>
  );
}
