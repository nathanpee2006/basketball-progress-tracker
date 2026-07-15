# Clerk webhook triggers user.created event to create new player

For context, before a player's session data was retrieved, their `PlayerId` was retrieved by the `ClerkUserId` through the `PlayerService.GetOrCreateAsync` (PS no longer exists as it has been replaced with `GetByClerkUserIdAsync`) class member. The problem was that the /api/sessions GET endpoint violated what a "safe" method meant. By definition, an HTTP method is considered safe if it is intended to be read-only—meaning calling it should not cause any side effects on the server, such as creating, modifying, or deleting resource state. In this case, I was getting/'creating' a player in the database before retrieving user data. To address, I have two options.

POST a /players endpoint after signup
- simple to setup
- if the user's browser crashes or loses internet connection after signup, the user account maybe registered in Clerk's DB but not in applications's DB

or

*Make a Clerk webhook send a POST request to /api/webhooks to create a new player when the event type is `user.created` after signup
- more setup required (webhook verification). However, it is reliable in the sense that I know that that the webhook is coming from Clerk and nobody else.
- reliable as it can retry on failures