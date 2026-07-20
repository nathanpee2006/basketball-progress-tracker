import { useEffect } from "react";
import { toast } from "sonner";
import { useNavigate } from "react-router";
import { useSessions } from "./useSessions";
import { SessionsSkeletonList } from "./components/SessionsSkeletonList";
import { SessionsEmptyState } from "./components/SessionsEmptyState";
import { SessionsList } from "./components/SessionsList";

export function SessionsPage() {
  const { sessions, isLoading, error } = useSessions();
  const navigate = useNavigate();

  useEffect(() => {
    if (error) toast.error(error.message);
  }, [error]);

  const handleView = (id: number) => {
    navigate(`/sessions/${id}`); 
  };

  const handleEdit = (id: number) => {
    // TODO
  };

  const handleDelete = (id: number) => {
    // TODO
  };

  const handleCreate = () => {
    // TODO
  };

  return (
    <section className="space-y-2">
      <h2 className="text-xl font-semibold">Sessions</h2>
      <p className="text-sm text-muted-foreground">
        Session history list with view, edit, and delete actions.
      </p>

      {isLoading && <SessionsSkeletonList />}

      {!isLoading && !error && sessions.length === 0 && (
        <SessionsEmptyState onCreateClick={handleCreate} />
      )}

      {!isLoading && sessions.length > 0 && (
        <SessionsList
          sessions={sessions}
          onView={handleView}
          onEdit={handleEdit}
          onDelete={handleDelete}
        />
      )}
    </section>
  );
}
