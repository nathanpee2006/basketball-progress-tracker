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

        var signingSecret = configuration["Clerk:WebhookSigningSecret"] ??
                            throw new InvalidOperationException("Clerk webhook signing secret is not configured.");
        try
        {
            var webhook = new Webhook(signingSecret);
            webhook.Verify(
                rawBody.AsSpan(),
                headerName => headerName is not null &&
                              httpContext.Request.Headers.TryGetValue(headerName, out var value)
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

        var firstName = data.GetProperty("first_name").GetString();
        var lastName = data.GetProperty("last_name").GetString();

        var emailAddresses = data.GetProperty("email_addresses").EnumerateArray();
        var primaryEmailIdProp = data.GetProperty("primary_email_address_id");
        if (primaryEmailIdProp.ValueKind != JsonValueKind.String)
        {
            return Results.BadRequest();
        }
        var primaryEmailAddressId = primaryEmailIdProp.GetString();
        var primaryEmailEntry = emailAddresses.FirstOrDefault(e =>
            e.TryGetProperty("id", out var idProp) && idProp.GetString() == primaryEmailAddressId);
        if (primaryEmailEntry.ValueKind != JsonValueKind.Object ||
            !primaryEmailEntry.TryGetProperty("email_address", out var emailProp) ||
            emailProp.ValueKind != JsonValueKind.String)
        {
            return Results.BadRequest();
        }
        var primaryEmail = emailProp.GetString();

        var imageUrl = data.GetProperty("image_url").GetString();

        var timeZone = ResolveTimeZone(data);

        var exists = await dbContext.Players.AnyAsync(p => p.ClerkUserId == clerkUserId && p.Email == primaryEmail,
            cancellationToken);
        if (!exists)
        {
            var player = new Player
            {
                ClerkUserId = clerkUserId,
                FirstName = firstName,
                LastName = lastName,
                Email = primaryEmail,
                ImageUrl = imageUrl,
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

