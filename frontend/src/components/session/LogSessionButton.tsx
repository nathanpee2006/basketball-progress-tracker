import { Button } from "@/components/ui/button";
import { CalendarPlus } from "lucide-react";

interface LogSessionButtonProps {
  onClick: () => void;
}

export function LogSessionButton({ onClick }: LogSessionButtonProps) {
  return (
    <Button
      size="lg"
      className="w-full bg-orange-500 hover:bg-orange-600 sm:w-auto sm:self-start"
      onClick={onClick}
    >
      <CalendarPlus className="mr-2 h-4 w-4" />
      Log Session
    </Button>
  );
}