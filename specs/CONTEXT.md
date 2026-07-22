# basketball-progress-tracker

This context covers the language used to track a basketball player's logged activity, history, and progress over time.

## Language

**Session**:
A logged basketball activity assigned to a calendar date.
_Avoid_: Workout, practice, game, event

**Streak**:
The count of consecutive ISO weeks (Monday-Sunday) in the player's local timezone with at least one session in each week, where only sessions dated in the current ISO week can extend the streak.
_Avoid_: Run, sequence, chain

**Player**:
The authenticated person who owns their logged basketball activity and progress.
_Avoid_: User, account, coach

**Shot**:
An individual shooting attempt recorded with made/missed status and a zone.
_Avoid_: Attempt, action

**Zone**:
A categorized shooting area on the court: paint, midrange, three-point line.
_Avoid_: Area, region, spot

**Free Throw**:
A session-level aggregate tracking makes and misses from free-throw attempts; stored as totals, not individual shots.
_Avoid_: Foul shot, penalty shot

**Drill**:
A timed basketball exercise (e.g., dribbling, sprint) labeled by name and tracked by completion time.
_Avoid_: Workout, exercise, routine
