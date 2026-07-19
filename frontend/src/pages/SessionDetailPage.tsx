import { useNavigate, useParams } from "react-router";
import { useSession } from "@/hooks/useSession";
import { SessionDetailSkeleton } from "@/components/sessions-detail/SessionDetailSkeleton";
import { SessionNotFound } from "@/components/sessions-detail/SessionNotFound";
import { SessionDetailHeader } from "@/components/sessions-detail/SessionDetailHeader";
import { SessionStatsBreakdown } from "@/components/sessions-detail/SessionStatsBreakdown";
import { SessionDrillList } from "@/components/sessions-detail/SessionDrillList";
import { CourtVisualization } from "@/components/sessions-detail/CourtVisualization";
import { sessionDetailToZoneStats } from "@/types/court";

export function SessionDetailPage() {
  const { id } = useParams();
  const navigate = useNavigate();
  const { session, isLoading, error } = useSession(Number(id));

  if (isLoading) {
    return <SessionDetailSkeleton />;
  }

  if (error || !session) {
    return <SessionNotFound onBackToSessions={() => navigate("/sessions")} />;
  }

  return (
    <section className="space-y-4">
      <SessionDetailHeader
        date={session.date}
        onEdit={() => navigate(`/sessions/${session.id}/edit`)}
      />
      <CourtVisualization mode="view" zones={sessionDetailToZoneStats(session)} />
      <SessionStatsBreakdown session={session} />
      <SessionDrillList drills={session.drills} />
    </section>
  );
}