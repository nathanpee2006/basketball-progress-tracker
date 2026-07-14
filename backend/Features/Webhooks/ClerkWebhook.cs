using System.Text;
using System.Text.Json;
using Backend.Common.Endpoints;
using Backend.Data;
using Backend.Data.Models;
using Microsoft.EntityFrameworkCore;
using Svix;
using Svix.Exceptions;

namespace Backend.Features.Webhooks;

public static class ClerkWebhook
{
    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("/webhooks", Handler)
                .WithTags("Webhooks");
        }
    }

    public static async Task<IResult> Handler(
        HttpContext httpContext,
        IConfiguration configuration,
        AppDbContext dbContext,
        CancellationToken cancellationToken)
    {
        string rawBody;
        using (var reader = new StreamReader(httpContext.Request.Body, Encoding.UTF8))
        {
            rawBody = await reader.ReadToEndAsync(cancellationToken);
        }

        if (string.IsNullOrEmpty(rawBody))
            return Results.BadRequest();

        var signingSecret = configuration["Clerk:WebhookSigningSecret"]
                            ?? throw new InvalidOperationException("Clerk webhook signing secret is not configured.");
        var webhook = new Webhook(signingSecret);
        try
        {
            webhook.Verify(
                rawBody.AsSpan(),
                headerName => httpContext.Request.Headers.TryGetValue(headerName, out var value)
                    ? value.ToString()
                    : null);
        }
        catch (Exception ex) when (ex is WebhookVerificationException or EmptyWebhookSecretException)
        {
            return Results.BadRequest();
        }

        using var jsonDoc = JsonDocument.Parse(rawBody);
        var root = jsonDoc.RootElement;
        var eventType = root.GetProperty("type").GetString();

        if (eventType != "user.created")
        {
            return Results.NoContent();
        }

        var data = root.GetProperty("data");
        var clerkUserId = data.GetProperty("id").GetString();
        if (string.IsNullOrEmpty(clerkUserId))
        {
            return Results.BadRequest();
        }

        var timeZone = ResolveTimeZone(data);

        var exists = await dbContext.Players.AnyAsync(p => p.ClerkUserId == clerkUserId, cancellationToken);
        if (!exists)
        {
            var player = new Player
            {
                ClerkUserId = clerkUserId,
                TimeZone = timeZone,
                CreatedAt = DateTime.UtcNow
            };
            dbContext.Players.Add(player);

            await dbContext.SaveChangesAsync(cancellationToken);
        }

        return TypedResults.Ok(); 
    }

    private static string ResolveTimeZone(JsonElement data)
    {
        if (data.TryGetProperty("unsafe_metadata", out var metadata)
            && metadata.TryGetProperty("timeZone", out var tzElement)
            && tzElement.ValueKind == JsonValueKind.String)
        {
            var tz = tzElement.GetString();
            if (!string.IsNullOrEmpty(tz) && TimeZoneInfo.TryFindSystemTimeZoneById(tz, out _))
            {
                return tz;
            }
        }

        return "UTC";
    }
}

