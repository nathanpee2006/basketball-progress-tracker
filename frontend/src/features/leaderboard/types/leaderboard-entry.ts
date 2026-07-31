export interface LeaderboardEntry {
  playerId: number;
  rank: number | null;
  playerName: string;
  imageUrl?: string | null;
  shotPercentage: number;
  isQualified: boolean;
}
