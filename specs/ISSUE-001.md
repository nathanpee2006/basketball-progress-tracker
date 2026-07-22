# Capture session shooting totals and free throws

## What to build

Add database schema and API endpoints to record a session with shooting totals grouped by zone (paint, midrange, three-point) and free throw makes/attempts. Sessions are stored with a calendar date in the player's local timezone. Percentages should be derived on read, never stored.

A session record should include:
- Calendar date (in player's timezone)
- Makes and attempts for each shooting zone (paint, midrange, three-point)
- Free throw makes and attempts
- Player ownership (authenticated user)

The API should accept a session creation payload with zone totals and free throw totals, persist them, and return the full session with derived percentages on read.

## Acceptance criteria

- [✅] Database schema supports Player ownership of Sessions
- [✅] Session schema stores makes/attempts for each zone (paint, midrange, three-point)
- [✅] Session schema stores free throw makes/attempts as session-level totals
- [✅] Session schema stores a calendar date in the player's local timezone
- [ ] API POST endpoint accepts a session creation payload with all zone data and free throw data
- [ ] API returns 400 on invalid zone or free throw data
- [✅] API GET endpoint returns a session with derived percentages for each zone and free throws (never stored percentages)
- [ ] Editing a session's date or totals updates the record without storing stale percentages
- [✅] Player can only read/write their own sessions (authenticated via Player ID)

## Blocked by

None - can start immediately.
