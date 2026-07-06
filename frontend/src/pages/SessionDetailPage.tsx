import { useParams } from "react-router";

export function SessionDetailPage() {
  const { id } = useParams();

  return (
    <section className="space-y-2">
      <h2 className="text-xl font-semibold">Session Detail</h2>
      <p className="text-sm text-muted-foreground">
        Detail view for session {id}.
      </p>
    </section>
  );
}
