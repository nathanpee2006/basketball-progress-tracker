import {
  Card,
  CardAction,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { SessionCardMenu } from "@/components/sessions-list/SessionCardMenu";
import type { Session } from "@/types/session";

type SessionCardProps = {
  session: Session;
  onView: (id: Session["id"]) => void;
  onEdit: (id: Session["id"]) => void;
  onDelete: (id: Session["id"]) => void;
}

export function SessionCard({ session, onView, onEdit, onDelete }: SessionCardProps) {
  return (
    <Card className="w-full">
      <CardHeader>
        <CardTitle>{new Date(session.date).toLocaleDateString()}</CardTitle>
        <CardDescription>
          {`Paint ${session.paintShotPercentage}% · Mid ${session.midrangeShotPercentage}% · 3PT ${session.threePointShotPercentage}% · FT ${session.freeThrowShotPercentage}% · Overall ${session.overallShotPercentage}%`}
        </CardDescription>
        <CardAction>
          <SessionCardMenu
            onView={() => onView(session.id)}
            onEdit={() => onEdit(session.id)}
            onDelete={() => onDelete(session.id)}
          />
        </CardAction>
      </CardHeader>
      <CardContent />
    </Card>
  );
}