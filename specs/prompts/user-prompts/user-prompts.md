# Prompts used during development

`/grill-with-docs` (Refer to /specs/prompts/agent-skills) Help me to create the models for the database. I want to be able to track my basketball progress over time. Things like how well I shot. What shooting areas in the court I am lacking in. Suggest other things that might be useful for knowing if a basketball player is improving or not.

`/to-prd` Generate a PRD based on our conversation.

`/to-issues` Generate issues based on the PRD created.

`/grill-with-docs` Rather than a daily streak, I want the streak to continue if the Player has at least one session logged in a week. So it would be a weekly streak rather than a daily one.

@Models/Player.cs I'm not going to store fields such as a PasswordHash. If you look @../frontend/src/App.tsx I am using Clerk as my managed auth provider. The dillema right now is that I need to have a PlayerId. Should I use the clerk sub claim as the PlayerId or do use both the player id and clerk sub claim in the database. Please provide tradeoffs and your recommendation as the best approach.

What are your thoughts on using middleware @backend/Program.cs to create a new Player before they can access an API endpoint? What other alternative approaches can be done? Remember I am using Clerk as auth.

@frontend/src/App.tsx and @backend/Program.cs I have a react vite frontend that uses Clerk for auth. In the backend which is an ASP.NET core minimal api should I be validating the claims of jwt tokens from the frontend? If so, what are those? Please include basis for your answers.

Please investigate this error message. How to fix this. fail: Microsoft.EntityFrameworkCore.Query[10100] An exception occurred while iterating over the results of a query for context type 'Backend.Data.AppDbContext'. System.ObjectDisposedException: Cannot access a disposed context instance. A common cause of this error is disposing a context instance that was resolved from dependency injection and then later trying to use the same context instance elsewhere in your application. This may occur if you are calling 'Dispose' on the context instance, or wrapping it in a using statement. If you are using dependency injection, you should let the dependency injection container take care of disposing context instances. Object name: 'AppDbContext'.

@backend/Services/ Please add ISessionService and SessionService that performs a query to list all sessions of the current player. Then @backend/Program.cs register the SessionService and use it to fetch the data from the DB in the /api/session endpoint. Also create SessionResponse record that is a DTO.

Review my GET /api/sessions endpoint for any errors and improvements that I can make. (Large code snippet not included)

Please implement the boilerplate code while adhering to my project's directory stucture to setup a /api/webhooks endpoint that will listen for POST requests. I will use a clerk webhook.

Implement test cases for the GetSessionEndpoint. Give suggestions as well for other test cases that might be helpful.
    1. 401 when no JWT token included in request     
    2. 404 when no player exists for the given ClerkUserId    
    3. 404 when no session exists for the given session id     
    4. 404 when valid session id is provided but the session does not belong to the player     
    5. 200 when session exists for the given session id with expected data and drill responses

Justify why a useCallback function is necessary for deleting a session. From my understanding useCallback is used to return a function back to me on initial render. React will then check that when the array of dependencies change it will provide me a new function. Why would I need a new function when I only need one function to delete a session?