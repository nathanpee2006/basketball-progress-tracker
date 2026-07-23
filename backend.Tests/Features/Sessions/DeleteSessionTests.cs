using System.Net;
using Backend.Data;
using Backend.Data.Models;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace backend.Tests.Features.Sessions;

public class DeleteSessionTests(ApiFixture api) : IClassFixture<ApiFixture>, IAsyncLifetime
{
    public Task InitializeAsync() => api.ResetAsync();
    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Returns_unauthorized_when_no_token()
    {
        var client = api.CreateClient();

        var response = await client.DeleteAsync("/api/sessions/1");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Returns_not_found_when_no_matching_player()
    {
        var client = api.CreateClient().AuthenticateAs("clerk_unknown_user");

        var response = await client.DeleteAsync("/api/sessions/1");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Returns_not_found_when_session_id_does_not_exist()
    {
        const string clerkUserId = "clerk_user_1";
        using var scope = api.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Players.Add(new Player { ClerkUserId = clerkUserId });
        await db.SaveChangesAsync();

        var client = api.CreateClient().AuthenticateAs(clerkUserId);
        var response = await client.DeleteAsync("/api/sessions/99999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Returns_not_found_when_session_belongs_to_another_player()
    {
        const string ownerClerkUserId = "clerk_owner";
        const string requesterClerkUserId = "clerk_requester";

        using var scope = api.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var owner = new Player { ClerkUserId = ownerClerkUserId };
        var requester = new Player { ClerkUserId = requesterClerkUserId };
        db.Players.AddRange(owner, requester);
        await db.SaveChangesAsync();

        var ownerSession = new Session
        {
            PlayerId = owner.Id,
            Date = new DateOnly(2026, 1, 10),
            PaintMakes = 5,
            PaintAttempts = 10,
            MidrangeMakes = 5,
            MidrangeAttempts = 10,
            ThreePointMakes = 5,
            ThreePointAttempts = 10,
            FreeThrowMakes = 5,
            FreeThrowAttempts = 10
        };
        db.Sessions.Add(ownerSession);
        await db.SaveChangesAsync();

        var client = api.CreateClient().AuthenticateAs(requesterClerkUserId);

        var response = await client.DeleteAsync($"/api/sessions/{ownerSession.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        // the session should still exist untouched
        var stillExists = await db.Sessions.AnyAsync(s => s.Id == ownerSession.Id);
        stillExists.Should().BeTrue();
    }

    [Fact]
    public async Task Returns_no_content_on_successful_deletion()
    {
        const string clerkUserId = "clerk_user_2";

        using var scope = api.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var player = new Player { ClerkUserId = clerkUserId };
        db.Players.Add(player);
        await db.SaveChangesAsync();

        var session = new Session
        {
            PlayerId = player.Id,
            Date = new DateOnly(2026, 1, 10),
            PaintMakes = 5,
            PaintAttempts = 10,
            MidrangeMakes = 5,
            MidrangeAttempts = 10,
            ThreePointMakes = 5,
            ThreePointAttempts = 10,
            FreeThrowMakes = 5,
            FreeThrowAttempts = 10
        };
        db.Sessions.Add(session);
        await db.SaveChangesAsync();

        var client = api.CreateClient().AuthenticateAs(clerkUserId);

        var response = await client.DeleteAsync($"/api/sessions/{session.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var deleted = await db.Sessions.AnyAsync(s => s.Id == session.Id);
        deleted.Should().BeFalse();
    }

    [Fact]
    public async Task Deletes_associated_drills_when_session_is_deleted()
    {
        const string clerkUserId = "clerk_user_3";

        using var scope = api.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var player = new Player { ClerkUserId = clerkUserId };
        db.Players.Add(player);
        await db.SaveChangesAsync();

        var session = new Session
        {
            PlayerId = player.Id,
            Date = new DateOnly(2026, 1, 10),
            PaintMakes = 5,
            PaintAttempts = 10,
            MidrangeMakes = 5,
            MidrangeAttempts = 10,
            ThreePointMakes = 5,
            ThreePointAttempts = 10,
            FreeThrowMakes = 5,
            FreeThrowAttempts = 10
        };
        db.Sessions.Add(session);
        await db.SaveChangesAsync();

        db.Drills.AddRange(
            new Drill { SessionId = session.Id, Name = "4x4", CompletionTimeInSeconds = 150 },
            new Drill { SessionId = session.Id, Name = "Suicides", CompletionTimeInSeconds = 120 }
        );
        await db.SaveChangesAsync();

        var client = api.CreateClient().AuthenticateAs(clerkUserId);

        var response = await client.DeleteAsync($"/api/sessions/{session.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var sessionExists = await db.Sessions.AnyAsync(s => s.Id == session.Id);
        sessionExists.Should().BeFalse();

        var remainingDrills = await db.Drills.CountAsync(d => d.SessionId == session.Id);
        remainingDrills.Should().Be(0, "drills should be cascade-deleted along with their parent session");
    }

    [Theory]
    [InlineData("abc")]
    [InlineData("1.5")]
    public async Task Returns_bad_request_for_non_numeric_id(string invalidId)
    {
        const string clerkUserId = "clerk_user_4";

        using var scope = api.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Players.Add(new Player { ClerkUserId = clerkUserId });
        await db.SaveChangesAsync();

        var client = api.CreateClient().AuthenticateAs(clerkUserId);

        var response = await client.DeleteAsync($"/api/sessions/{invalidId}");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Returns_not_found_for_non_positive_id(int nonPositiveId)
    {
        const string clerkUserId = "clerk_user_5";

        using var scope = api.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Players.Add(new Player { ClerkUserId = clerkUserId });
        await db.SaveChangesAsync();

        var client = api.CreateClient().AuthenticateAs(clerkUserId);

        var response = await client.DeleteAsync($"/api/sessions/{nonPositiveId}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
