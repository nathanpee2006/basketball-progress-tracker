# Player uses an internal ID plus Clerk user ID

Player records keep an internal `PlayerId` as the primary key and store Clerk's `sub` claim in a separate unique `ClerkUserId` field. This keeps the database decoupled from the auth provider, preserves simple relational joins, and leaves room to change auth providers later without rewriting primary keys. However, it does introduce an additional lookup step when querying by Clerk user ID, which is acceptable for now.
