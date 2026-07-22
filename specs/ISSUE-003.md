# Read progress analytics from session history

## What to build

Add API endpoints to query a player's session history and derive progress metrics on read. Analytics should include:

- Shooting percentage by zone (all-time and per-session)
- Weakest shooting zone (lowest make rate)
- Free throw percentage trends
- Total shot volume by zone
- Session history ordered by date
- Drill time trends for a given drill name

All aggregates are derived from session data on read; nothing is cached on the player record.

## Acceptance criteria

- [ ] API GET endpoint returns all sessions for the authenticated player, ordered by date
- [ ] API GET endpoint returns career shooting percentage by zone (derived from all sessions)
- [ ] API GET endpoint returns the zone with the lowest career make rate
- [ ] API GET endpoint returns career free throw percentage (derived from all sessions)
- [ ] API GET endpoint returns total shots attempted per zone across all sessions
- [ ] API GET endpoint returns drill history for a given drill name (all times, sorted by date)
- [ ] API GET endpoint supports date range filtering (from/to) for all analytics
- [ ] All percentages are calculated on read, never stored or cached
- [ ] Performance is acceptable with typical session volumes (sub-second response)

## Blocked by

- ISSUE-001 (Capture session shooting totals and free throws)
