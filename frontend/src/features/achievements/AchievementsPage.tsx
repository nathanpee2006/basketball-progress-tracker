import { useEffect } from "react";
import { toast } from "sonner";
import { useAchievements } from "./useAchievements";
import { AchievementList } from "./components/achievement-list";

export function AchievementsPage() {

  const { achievements, isLoading, error } = useAchievements();

  useEffect(() => {
    if (error) {
      toast.error(error.message);
    }
  }, [error]);


  if (isLoading) {
    return <p>Loading achievements...</p>;
  }

  return (
    <section className="space-y-6">
      <div className="space-y-2">
        <h2 className="text-xl font-semibold">Achievements</h2>
        <p className="text-sm text-muted-foreground">
            Achievements are unlocked by completing specific milestones in your basketball training. Track your progress and celebrate your accomplishments as you reach new heights in your skills and performance.
        </p>
      </div>

      <AchievementList achievements={achievements} />

    </section>
  );
}
