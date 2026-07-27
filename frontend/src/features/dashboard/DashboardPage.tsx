import { useNavigate } from "react-router";
import { useConsistency } from "./useConsistency";
import { useSessions } from "../sessions-list/useSessions";
import { WeeklyStreakHero } from "./components/WeeklyStreakHero";
import { LogSessionButton } from "../../components/session/LogSessionButton";
import { QuickAnalyticsSnapshot } from "./components/QuickAnalyticsSnapshot";
import { SessionsSkeletonList } from "../../components/session/SessionsSkeletonList";
import { SessionsEmptyState } from "../../components/session/SessionsEmptyState";
import { SessionsList } from "@/components/session/SessionsList";

const RECENT_SESSIONS_LIMIT = 5;

export function DashboardPage() {
  const navigate = useNavigate();
  const { consistency, isLoading: isConsistencyLoading } = useConsistency();
  const {
    sessions,
    isLoading: isSessionsLoading,
    error: SessionError,
    deleteSession,
  } = useSessions();

  const recentSessions = (sessions ?? []).slice(0, RECENT_SESSIONS_LIMIT);

  const handleView = (id: number) => navigate(`/sessions/${id}`);
  const handleEdit = (id: number) => navigate(`/sessions/${id}/edit`);
  const handleDelete = async (id: number) => await deleteSession(id);
  const handleCreate = () => navigate("/sessions/new");

  return (
    <section className="space-y-6">
      <div className="space-y-2">
        <h2 className="text-xl font-semibold">Dashboard</h2>
        <p className="text-sm text-muted-foreground">
          Overview for streaks, recent sessions, and quick actions.
        </p>
      </div>

      <WeeklyStreakHero
        consistency={consistency}
        isLoading={isConsistencyLoading}
      />

      <LogSessionButton onClick={handleCreate} />

      <QuickAnalyticsSnapshot
        consistency={consistency}
        isLoading={isConsistencyLoading}
      />

      {isSessionsLoading && <SessionsSkeletonList />}

      {!isSessionsLoading && !SessionError && recentSessions.length === 0 && (
        <SessionsEmptyState onCreateClick={handleCreate} />
      )}

      {!isSessionsLoading && !SessionError && recentSessions.length > 0 && (
        <SessionsList
          sessions={recentSessions}
          onView={handleView}
          onEdit={handleEdit}
          onDelete={handleDelete}
        />
      )}
    </section>
  );
}
