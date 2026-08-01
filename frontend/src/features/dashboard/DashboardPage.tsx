import { Link, useNavigate } from "react-router";
import { useConsistency } from "./useConsistency";
import { useSessions } from "../sessions-list/useSessions";
import { WeeklyStreakHero } from "./components/WeeklyStreakHero";
import { LogSessionButton } from "../../components/session/LogSessionButton";
import { QuickAnalyticsSnapshot } from "./components/QuickAnalyticsSnapshot";
import { SessionsSkeletonList } from "../../components/session/SessionsSkeletonList";
import { SessionsEmptyState } from "../../components/session/SessionsEmptyState";
import { SessionsList } from "@/components/session/SessionsList";
import { toast } from "sonner";
import { useEffect } from "react";
import { AchievementGrid } from "../achievements/components/achievement-grid";
import { useAchievements } from "../achievements/useAchievements";

const RECENT_SESSIONS_LIMIT = 2;

export function DashboardPage() {
  const navigate = useNavigate();
  const {
    consistency,
    isLoading: isConsistencyLoading,
    error: ConsistencyError,
  } = useConsistency();
  const {
    sessions,
    isLoading: isSessionsLoading,
    error: SessionError,
    deleteSession,
  } = useSessions();
  const {
    achievements,
    isLoading: isAchievementsLoading,
    error: AchievementError,
  } = useAchievements();

  useEffect(() => {
    if (SessionError) toast.error(SessionError.message);
    if (ConsistencyError) toast.error(ConsistencyError.message);
    if (AchievementError) toast.error(AchievementError.message);
  }, [SessionError, ConsistencyError, AchievementError]);

  const recentSessions = (sessions ?? []).slice(0, RECENT_SESSIONS_LIMIT);

  const handleView = (id: number) => navigate(`/sessions/${id}`);
  const handleEdit = (id: number) => navigate(`/sessions/${id}/edit`);
  const handleDelete = async (id: number) => {
    try {
      await deleteSession(id);
      toast.success("Session deleted successfully");
    } catch {
      toast.error("Failed to delete session");
    }
  };
  const handleCreate = () => navigate("/sessions/new");

  const achieved = (achievements ?? [])
    .filter((a) => a.achievedAt !== null)
    .sort(
      (a, b) =>
        new Date(b.achievedAt!).getTime() - new Date(a.achievedAt!).getTime(),
    );
  const unachieved = (achievements ?? []).filter((a) => a.achievedAt === null);
  const recentAchievements = [
    ...achieved.slice(0, 2),
    ...unachieved.slice(0, 4 - Math.min(achieved.length, 2)),
  ];

  return (
    <section className="space-y-6">
      <div className="space-y-2">
        <h2 className="text-xl font-semibold">Dashboard</h2>
        <p className="text-sm text-muted-foreground">
          Overview for streaks, achievements, recent sessions, and quick
          actions.
        </p>
      </div>

      <WeeklyStreakHero
        consistency={consistency}
        isLoading={isConsistencyLoading}
      />

      <div className="flex items-center justify-between gap-4">
        <LogSessionButton onClick={handleCreate} />
        <Link to="/achievements">View All</Link>
      </div>

      {!isAchievementsLoading &&
        !AchievementError &&
        recentAchievements.length > 0 && (
          <AchievementGrid achievements={recentAchievements} columns={4} />
        )}

      <QuickAnalyticsSnapshot
        consistency={consistency}
        isLoading={isConsistencyLoading}
      />

      {isSessionsLoading && <SessionsSkeletonList />}

      {!isSessionsLoading && !SessionError && recentSessions.length === 0 && (
        <SessionsEmptyState onCreateClick={handleCreate} />
      )}

      {!isSessionsLoading && recentSessions.length > 0 && (
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
