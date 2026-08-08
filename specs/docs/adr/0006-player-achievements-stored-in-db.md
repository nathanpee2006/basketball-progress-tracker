# Player and Achievements are a many-to-many relationship and junction table signifies whether they achieved an achievement or not

When deciding whether a player achieved an achievement after a session creation I had two options:

Derived-only (compute on every request, nothing stored)

Pro: single source of truth (Sessions table), zero migration/sync risk, trivial to add/change badge definitions since nothing needs backfilling.
Con: every request recomputes across full session history — fine at 100 sessions, gets expensive at 5,000+ per player if you're doing this on every Dashboard load. Also loses the unlock moment — you can't show "achieved 3 days ago" or trigger a celebratory toast on the exact session that crossed the threshold, since you're just recalculating current state each time.

Stored-only (write a row when unlocked, e.g. in an PlayerAchievement table)

Pro: fast reads (indexed lookup, not aggregation), you get a real achievedAt timestamp, you can fire "achievement unlocked" events/notifications at the moment it happens.
Con: you now have two sources of truth. If you ever change a badge's threshold or fix a bug in the makes-counting logic, existing rows are now wrong and need a backfill job. You also need to run the unlock-check logic somewhere on write (session creation), which adds complexity to that path.