# Drills are separate records linked to sessions

Drills (timed exercises like sprints and dribbling) are stored as separate records with a foreign key to the session, not nested within the session. This allows a session to contain only shooting data, only drill data, or both, and makes it easier to query drill history independently.
