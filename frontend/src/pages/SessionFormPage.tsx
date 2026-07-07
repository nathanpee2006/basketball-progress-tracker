import { useParams } from "react-router";

type SessionFormPageProps = {
  mode: "create" | "edit";
};

export function SessionFormPage({ mode }: SessionFormPageProps) {
  const { id } = useParams();

  return (
    <section className="space-y-2">
      <h2 className="text-xl font-semibold">
        {mode === "create" ? "New Session" : "Edit Session"}
      </h2>
      <p className="text-sm text-muted-foreground">
        {mode === "create"
          ? "Create flow for logging zone shooting, free throws, and optional drills."
          : `Editing session ${id}.`}
      </p>
    </section>
  );
}
