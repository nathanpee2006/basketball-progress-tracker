using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace backend.Tests.Features;

public class TestAuthHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    public const string SchemeName = "TestScheme";
    public const string UserIdHeader = "X-Test-ClerkUserId";

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // No header => treat as unauthenticated so RequireAuthorization() -> 401 still works.
        if (!Request.Headers.TryGetValue(UserIdHeader, out var clerkUserId) ||
            string.IsNullOrWhiteSpace(clerkUserId))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        var claims = new[] { new Claim(ClaimTypes.NameIdentifier, clerkUserId!) };
        var identity = new ClaimsIdentity(claims, SchemeName);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, SchemeName);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}

public static class TestAuthHttpClientExtensions
{
    public static HttpClient AuthenticateAs(this HttpClient client, string clerkUserId)
    {
        client.DefaultRequestHeaders.Remove(TestAuthHandler.UserIdHeader);
        client.DefaultRequestHeaders.Add(TestAuthHandler.UserIdHeader, clerkUserId);
        return client;
    }
}