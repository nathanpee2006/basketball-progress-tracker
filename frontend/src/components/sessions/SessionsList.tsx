import { SessionCard } from "./SessionCard";
import type { Session } from "types/session";

interface SessionsListProps {
  sessions: Session[];
  onView: (id: Session["id"]) => void;
  onEdit: (id: Session["id"]) => void;
  onDelete: (id: Session["id"]) => void;
}

export function SessionsList({ sessions, onView, onEdit, onDelete }: SessionsListProps) {
  return (
    <>
      {sessions.map((session) => (
        <SessionCard
          key={session.id}
          session={session}
          onView={onView}
          onEdit={onEdit}
          onDelete={onDelete}
        />
      ))}
    </>
  );
}