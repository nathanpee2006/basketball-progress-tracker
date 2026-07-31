export interface LeaderboardEntry {
  playerId: number;
  rank?: number;
  playerName: string;
  imageUrl?: string;
  shotPercentage: number;
  isQualified: boolean;
}
