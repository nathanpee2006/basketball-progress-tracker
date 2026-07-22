# Basketball progress database models

## Problem Statement

I want to track whether a basketball player is improving over time, not just whether they showed up. Right now the data model is too coarse to answer questions like: which shooting zones are weak, whether free throws are improving, how fast drills are getting, and whether progress is consistent across sessions.

## Solution

Create a basketball progress model centered on a dated `Session` owned by a `Player`. A session should store shooting totals by zone, free throw makes and attempts, and any timed drills linked to that session. Percentages and long-range summaries should be derived when read, not stored, so the data stays consistent when sessions are edited.

## User Stories

1. As a player, I want to log a basketball session on a calendar day, so that my progress is tied to a real timeline.
2. As a player, I want to record shooting results by zone, so that I can see which areas of the court I am weak in.
3. As a player, I want to record made and attempted shots without storing percentages, so that accuracy is always derived from the source numbers.
4. As a player, I want to see my shooting percentage for each zone over time, so that I can measure improvement.
5. As a player, I want to see which zone has the lowest make rate, so that I know what to practice next.
6. As a player, I want to see how many shots I took in each zone, so that I can compare volume with accuracy.
7. As a player, I want to record free throw makes and misses, so that I can track one of the most important scoring skills.
8. As a player, I want to see my free throw percentage over time, so that I can tell whether my routine is working.
9. As a player, I want to log timed drills such as sprinting and dribbling, so that I can measure conditioning and ball handling improvement.
10. As a player, I want each drill to be attached to a session but not required for every session, so that I can log shooting-only or drill-only work.
11. As a player, I want to compare drill times across sessions, so that I can see whether I am getting faster.
12. As a player, I want to see a weekly streak based on ISO weeks, so that I can stay consistent week to week.
13. As a player, I want backfilled past-week sessions to keep historical records without repairing a broken streak, so that streaks reflect current consistency.
14. As a player, I want my progress to be calculated from session data on read, so that old summaries never go stale.
15. As a player, I want to track session frequency over time, so that I can see whether I am practicing often enough to improve.
16. As a player, I want to compare recent performance against older performance, so that I can tell whether progress is real or just a one-off good day.
17. As a player, I want the model to support future metrics like consistency and shot volume, so that the tracker can grow with my training needs.

## Implementation Decisions

- Model the authenticated person as a `Player` who owns all progress data.
- Model basketball work as a dated `Session` in the player’s local timezone.
- Store shooting totals directly with the session, grouped by zone.
- Start with zone buckets for paint, midrange, and three-point line.
- Store made and attempted counts, not percentages.
- Derive all percentages from makes and attempts when reading data.
- Do not store cross-session aggregate stats on the player record.
- Keep free throw tracking separate from zone shooting and store it as session-level makes and attempts.
- Model drills as separate records linked to a session, so a session can contain shooting only, drills only, or both.
- Use a drill name or label for timed exercises such as sprinting and dribbling.
- Store drill completion time as the primary drill performance metric.
- Treat streaks as weekly (ISO week, Monday-Sunday) in the player's local timezone.
- Only sessions dated in the current ISO week can extend streaks; prior-week backfills do not repair streaks, including edited session dates.
- Leave advanced metrics such as consistency scoring, shot-location coordinates, and video analysis for a later iteration.

## Testing Decisions

- Test through the highest useful seam: the API surface and its persistence behavior, not entity internals.
- Good tests should prove that saving a session produces the expected shooting and drill records, and that reading the session returns derived percentages correctly.
- Good tests should prove streaks are calculated by ISO weeks in the player's local timezone.
- Good tests should prove current-week sessions can extend streaks while prior-week backfills cannot repair a broken streak.
- Good tests should prove editing session dates follows the same current-week-only streak rule.
- Good tests should prove that sessions can exist with only shooting data, only drills, or both.
- Good tests should prove that derived metrics are recalculated from session data after updates or deletes.
- Prior art in this repo is minimal today, so the first tests should focus on external API behavior and database-backed round trips.

## Out of Scope

- Shot-by-shot persistence for every individual attempt.
- Player leaderboard features.
- Opponent scouting or game-by-game stat breakdowns.
- Advanced wearable, biomechanical, or video-derived measurements.
- Machine-generated training recommendations.
- Team-level or multi-player analytics.

## Further Notes

- Useful improvement signals beyond shooting include free throw accuracy, zone balance, total shot volume, drill times, consistency between sessions, and practice frequency.
- If the zone taxonomy grows beyond the initial buckets, the model may need to shift from fixed columns to a more flexible zone-stat structure later.
- The most valuable first release is the one that makes progress visible without making the database brittle.
