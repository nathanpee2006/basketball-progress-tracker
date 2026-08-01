export interface Achievement {
  id: string;
  name: string;
  description?: string | null;
  trigger: "metric" | "api" | "streak";
  badgeUrl?: string | null;
  progress?: number;
  rarity?: number;
}

export interface UserAchievement extends Achievement {
  achievedAt: string | null;
}
