import { Card, CardHeader } from "@/components/ui/card";
import { Skeleton } from "@/components/ui/skeleton";

type SessionsSkeletonListProps = {
  count?: number;
}

export function SessionsSkeletonList({ count = 5 }: SessionsSkeletonListProps) {
  return (
    <div className="space-y-2">
      {Array.from({ length: count }).map((_, i) => (
        <Card key={i} className="w-full">
          <CardHeader>
            <Skeleton className="h-4 w-1/3" />
            <Skeleton className="h-3 w-2/3" />
          </CardHeader>
        </Card>
      ))}
    </div>
  );
}