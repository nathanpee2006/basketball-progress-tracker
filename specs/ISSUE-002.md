# Record timed drills on a session

## What to build

Add schema and API support for timed drills (e.g., sprints, dribbling exercises) as separate records linked to a session. A drill should include a name or label, completion time in seconds, and a reference to the parent session. A session can have zero, one, or many drills.

Drills are optional per session—a player can log shooting-only sessions, drill-only sessions, or both together.

## Acceptance criteria

- [✅] Database schema supports Drill records with session foreign key
- [✅] Drill schema stores a name/label (text field for drill description)
- [✅] Drill schema stores completion time in seconds
- [✅] Drill schema stores creation timestamp
- [ ] API POST endpoint accepts a drill creation payload (name, time, session ID)
- [✅] API returns 400 on invalid drill data (missing name, invalid time)
- [✅ ] API GET endpoint returns a session with all linked drills
- [✅] A session can exist with zero drills (shooting only)
- [ ] A session can exist with only drills (no shooting data)
- [ ] Player can only read/write drills on their own sessions

## Blocked by

None - can start immediately.
