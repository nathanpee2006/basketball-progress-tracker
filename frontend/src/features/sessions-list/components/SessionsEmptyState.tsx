import { ClipboardList } from "lucide-react";
import { Button } from "@/components/ui/button";
import {
  Empty,
  EmptyHeader,
  EmptyMedia,
  EmptyTitle,
  EmptyDescription,
  EmptyContent,
} from "@/components/ui/empty";

type SessionsEmptyStateProps = {
  onCreateClick: () => void;
}

export function SessionsEmptyState({ onCreateClick }: SessionsEmptyStateProps) {
  return (
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
        <Button onClick={onCreateClick}>Log your first session</Button>
      </EmptyContent>
    </Empty>
  );
}