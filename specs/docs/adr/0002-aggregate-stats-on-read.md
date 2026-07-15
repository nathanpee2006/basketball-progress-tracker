# Aggregate stats computed on read, not cached

Zone shooting aggregates (makes, attempts) are stored at the session level, but percentages and cross-session career statistics are never cached on player records. All percentages and multi-session analytics are computed from session data on read. This keeps the schema simple, prevents stale data, and ensures consistency when sessions are edited.
