using System.Net;
using System.Net.Http.Json;
using Backend.Data;
using Backend.Data.Models;
using Backend.Features.Sessions;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace backend.Tests.Features.Sessions;

public class GetSessionsTests(ApiFixture api) : IClassFixture<ApiFixture>, IAsyncLifetime
{
    public Task InitializeAsync() => api.ResetAsync();
    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Returns_unauthorized_when_no_token()
    {
        var client = api.CreateClient();

        var response = await client.GetAsync("/api/sessions");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Returns_not_found_when_no_matching_player()
    {
        var client = api.CreateClient().AuthenticateAs("clerk_unknown_user");

        var response = await client.GetAsync("/api/sessions");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Returns_empty_list_when_player_has_no_sessions()
    {
        const string clerkUserId = "clerk_user_1";
        using var scope = api.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Players.Add(new Player { ClerkUserId = clerkUserId });
        await db.SaveChangesAsync();
        var client = api.CreateClient().AuthenticateAs(clerkUserId);

        var response = await client.GetAsync("/api/sessions");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var sessions = await response.Content.ReadFromJsonAsync<List<GetSessions.Response>>();
        sessions.Should().BeEmpty();
    }

    [Fact]
    public async Task Returns_sessions_with_correct_percentages_and_ordering()
    {
        const string clerkUserId = "clerk_user_2";
        int playerId;

        using var scope = api.Services.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var player = new Player { ClerkUserId = clerkUserId };
        db.Players.Add(player);
        await db.SaveChangesAsync();
        playerId = player.Id;

        db.Sessions.AddRange(
            new Session
            {
                PlayerId = playerId,
                Date = new DateOnly(2026, 1, 10),
                CreatedAt = new DateTime(2026, 1, 10, 9, 0, 0, DateTimeKind.Utc),
                PaintMakes = 5,
                PaintAttempts = 10,
                MidrangeMakes = 3,
                MidrangeAttempts = 10,
                ThreePointMakes = 2,
                ThreePointAttempts = 10,
                FreeThrowMakes = 8,
                FreeThrowAttempts = 10
            },
            new Session
            {
                PlayerId = playerId,
                Date = new DateOnly(2026, 1, 12),
                CreatedAt = new DateTime(2026, 1, 12, 9, 0, 0, DateTimeKind.Utc),
                PaintMakes = 0,
                PaintAttempts = 0,
                MidrangeMakes = 0,
                MidrangeAttempts = 0,
                ThreePointMakes = 0,
                ThreePointAttempts = 0,
                FreeThrowMakes = 0,
                FreeThrowAttempts = 0
            },
            new Session
            {
                PlayerId = playerId,
                Date = new DateOnly(2026, 1, 12),
                CreatedAt = new DateTime(2026, 1, 12, 15, 0, 0, DateTimeKind.Utc),
                PaintMakes = 10,
                PaintAttempts = 10,
                MidrangeMakes = 10,
                MidrangeAttempts = 10,
                ThreePointMakes = 10,
                ThreePointAttempts = 10,
                FreeThrowMakes = 10,
                FreeThrowAttempts = 10
            }
        );
        await db.SaveChangesAsync();
        var client = api.CreateClient().AuthenticateAs(clerkUserId);

        var response = await client.GetAsync("/api/sessions");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var sessions = await response.Content.ReadFromJsonAsync<List<GetSessions.Response>>();
        sessions.Should().HaveCount(3);

        // Ordering: Date desc, then CreatedAt desc
        sessions[0].Date.Should().Be(new DateOnly(2026, 1, 12));
        sessions[0].OverallShotPercentage.Should().Be(100); // 15:00 session (all makes)
        sessions[1].Date.Should().Be(new DateOnly(2026, 1, 12));
        sessions[1].OverallShotPercentage.Should().Be(0); // 09:00 session (all zero attempts)
        sessions[2].Date.Should().Be(new DateOnly(2026, 1, 10));

        var jan10 = sessions[2];
        jan10.PaintShotPercentage.Should().Be(50);
        jan10.MidrangeShotPercentage.Should().Be(30);
        jan10.ThreePointShotPercentage.Should().Be(20);
        jan10.FreeThrowShotPercentage.Should().Be(80);
        jan10.OverallShotPercentage.Should().Be(45); // 18/40
    }
}