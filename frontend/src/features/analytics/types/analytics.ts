import type { ZoneId } from "@/types/court";
 
export interface AnalyticsZoneStat {
  zone: ZoneId;
  attempts: number;
  makes: number;
  percentage: number; 
}
 
export interface SessionZoneStats {
  sessionId: number;
  date: string; 
  zones: AnalyticsZoneStat[];
}
 
export interface FreeThrowTrendPoint {
  date: string;
  attempts: number;
  makes: number;
  percentage: number;
}
 
export interface ShootingAnalyticsResponse {
  shootingByZone: AnalyticsZoneStat[];
  shootingByZonePerSession: SessionZoneStats[];
  weakestShootingZone: ZoneId | "";
  freeThrowPercentage: number;
  freeThrowTrend: FreeThrowTrendPoint[];
  fromDate: string | null;
  toDate: string | null;
}
 
export interface DateRangeParams {
  from?: string; // "yyyy-MM-dd"
  to?: string;
}

export const ZONE_LABELS: Record<string, string> = {
  paint: "Paint",
  midrange: "Midrange",
  threePoint: "Three Point",
};