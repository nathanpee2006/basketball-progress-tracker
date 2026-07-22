# Surface consistency and streak behavior

## What to build

Add API endpoints to compute and display player consistency metrics. Streaks should be weekly and calculated in the player's local timezone using ISO weeks (Monday-Sunday).

A streak is the count of consecutive ISO weeks with at least one session dated in each week. Only sessions dated in the current ISO week can extend the streak. Sessions dated in prior ISO weeks are treated as backfill and do not repair a broken streak, including when session dates are edited.

Also track session frequency (total sessions, sessions per week/month) to measure practice consistency.

## Acceptance criteria

- [ ] Streak calculation uses ISO weeks (Monday-Sunday), not daily boundaries
- [ ] Streak respects player's local timezone for week boundaries
- [ ] Sessions dated in the current ISO week can extend the streak
- [ ] Sessions dated in prior ISO weeks do not repair a broken streak
- [ ] Editing a session's date follows the same current-week-only streak rule
- [ ] Deleting a session recalculates streaks correctly
- [ ] API GET endpoint returns current streak length
- [ ] API GET endpoint returns longest streak (all-time)
- [ ] API GET endpoint returns total session count
- [ ] API GET endpoint returns sessions per week (average)
- [ ] API GET endpoint returns sessions per month (average)
- [ ] Streak is derived on read from session data, never cached
- [ ] All calculations work correctly with backfilled or edited session dates under the non-repair rule

## Blocked by

- ISSUE-001 (Capture session shooting totals and free throws)
