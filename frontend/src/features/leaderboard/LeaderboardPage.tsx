import { LeaderboardRankings } from "./components/leaderboard-rankings";
import { useLeaderboard } from "./useLeaderboard";
import { SelectZone } from "./components/SelectZone";
import { toast } from "sonner";
import { useEffect } from "react";

export function LeaderboardPage() {
  const { leaderboard, isLoading, error, setZone } = useLeaderboard();

  const rankings = leaderboard.map((entry) => ({
    userId: String(entry.playerId),
    rank: entry.rank ?? null,
    userName: entry.playerName,
    value: entry.shotPercentage,
    avatarUrl: entry.imageUrl,
    byline:
      !entry.isQualified ?  
      "Not qualified yet. Shoot at least 20 attempts for this zone to qualify for leaderboard."
      : undefined,
  }));

  useEffect(() => {
    if (error) {
      toast.error(`Failed to load leaderboard. ${error.message}`); 
    }
  }, [error]);

  return (
    <section className="space-y-6">
      <div className="space-y-2">
        <h2 className="text-xl font-semibold">Leaderboard</h2>
        <p className="text-sm text-muted-foreground">
          View the other player's weekly shooting performance and try to beat them!
        </p>
      </div>

      <SelectZone setZone={setZone} />

      {isLoading && (
        <p className="text-sm text-muted-foreground">Loading leaderboard...</p>
      )}

      {!isLoading && !error && <LeaderboardRankings rankings={rankings} />}
    </section>
  );
}
