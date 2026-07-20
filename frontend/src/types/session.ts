export interface Session {
  id: number;
  date: string;
  paintShotPercentage: number;
  midrangeShotPercentage: number;
  threePointShotPercentage: number;
  freeThrowShotPercentage: number;
  overallShotPercentage: number;
};

export interface Drill {
  id: number;
  name: string;
  completionTimeInSeconds: number; 
}

export interface SessionDetail extends Session {
  paintMakes: number;
  paintAttempts: number;
  midrangeMakes: number;
  midrangeAttempts: number;
  threePointMakes: number;
  threePointAttempts: number;
  freeThrowMakes: number;
  freeThrowAttempts: number;
  overallMakes: number;
  overallAttempts: number;
  drills: Drill[];
}
