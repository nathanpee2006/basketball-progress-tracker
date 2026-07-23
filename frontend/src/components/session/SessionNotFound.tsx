import { SearchX } from "lucide-react";
import { Button } from "@/components/ui/button";
import {
  Empty,
  EmptyHeader,
  EmptyMedia,
  EmptyTitle,
  EmptyDescription,
  EmptyContent,
} from "@/components/ui/empty";

interface SessionNotFoundProps {
  onBackToSessions: () => void;
}

export function SessionNotFound({ onBackToSessions }: SessionNotFoundProps) {
  return (
    <Empty>
      <EmptyHeader>
        <EmptyMedia variant="icon">
          <SearchX />
        </EmptyMedia>
        <EmptyTitle>Session not found</EmptyTitle>
        <EmptyDescription>
          This session may have been deleted or the link is incorrect.
        </EmptyDescription>
      </EmptyHeader>
      <EmptyContent>
        <Button variant="outline" onClick={onBackToSessions}>
          Back to sessions
        </Button>
      </EmptyContent>
    </Empty>
  );
}