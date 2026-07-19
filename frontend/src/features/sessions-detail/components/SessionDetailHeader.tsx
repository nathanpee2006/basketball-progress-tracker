import { Button } from "@/components/ui/button";

interface SessionDetailHeaderProps {
  date: string;
  onEdit: () => void;
}

export function SessionDetailHeader({ date, onEdit }: SessionDetailHeaderProps) {
  return (
    <div className="flex items-center justify-between">
      <div>
        <h2 className="text-xl font-semibold">
          {new Date(date).toLocaleDateString(undefined, {
            weekday: "long",
            year: "numeric",
            month: "long",
            day: "numeric",
          })}
        </h2>
        <p className="text-sm text-muted-foreground">Session detail</p>
      </div>
      <Button variant="outline" onClick={onEdit}>
        Edit
      </Button>
    </div>
  );
}