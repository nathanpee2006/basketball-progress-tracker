import type { SessionDetail } from "./session";

export type ZoneId = "paint" | "midrange" | "threePoint" | "freeThrow";

export interface ZoneStats {
  id: ZoneId;
  label: string;
  makes: number;
  attempts: number;
}

export type CourtMode = "view" | "edit";

export function sessionDetailToZoneStats(session: SessionDetail): ZoneStats[] {
  return [
    { id: "paint", label: "Paint", makes: session.paintMakes, attempts: session.paintAttempts },
    { id: "midrange", label: "Midrange", makes: session.midrangeMakes, attempts: session.midrangeAttempts },
    { id: "threePoint", label: "Three Point", makes: session.threePointMakes, attempts: session.threePointAttempts },
    { id: "freeThrow", label: "Free Throw", makes: session.freeThrowMakes, attempts: session.freeThrowAttempts },
  ];
}