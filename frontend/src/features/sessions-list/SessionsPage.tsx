import { useEffect } from "react";
import { toast } from "sonner";
import { useNavigate } from "react-router";
import { useSessions } from "./useSessions";
import { SessionsSkeletonList } from "../../components/session/SessionsSkeletonList";
import { SessionsEmptyState } from "../../components/session/SessionsEmptyState";
import { LogSessionButton } from "../../components/session/LogSessionButton";
import { SessionsList } from "@/components/session/SessionsList";


export function SessionsPage() {
  const { sessions, isLoading, error, deleteSession } = useSessions();
  const navigate = useNavigate();

  useEffect(() => {
    if (error) toast.error(error.message);
  }, [error]);

  const handleView = (id: number) => {
    navigate(`/sessions/${id}`);
  };

  const handleEdit = (id: number) => {
    navigate(`/sessions/${id}/edit`); 
  };

  const handleDelete = async (id: number) => {
    try {
      await deleteSession(id);
      toast.success("Session deleted successfully");
    } catch {
      
    }
  };

  const handleCreate = () => {
    navigate("/sessions/new");
  };

  return (
    <section className="space-y-2">
      <div className="flex items-center justify-between">
        <div>
          <h2 className="text-xl font-semibold">Sessions</h2>
          <p className="text-sm text-muted-foreground">
            Session history list with view, edit, and delete actions.
          </p>
        </div>
        <LogSessionButton onClick={handleCreate} />
      </div>

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
