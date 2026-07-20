using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Backend.Data;
using Backend.Data.Models;
using Backend.Features.Sessions;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace backend.Tests.Features.Sessions;

public class GetSessionTests(ApiFixture api) : IClassFixture<ApiFixture>, IAsyncLifetime
{
    public Task InitializeAsync() => api.ResetAsync();
    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Returns_unauthorized_when_no_token()
    {
        var client = api.CreateClient();

        var response = await client.GetAsync("/api/sessions/1");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Returns_not_found_when_no_matching_player()
    {
        var client = api.CreateClient().AuthenticateAs("clerk_unknown_user");

        var response = await client.GetAsync("/api/sessions/1");

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
        var response = await client.GetAsync("/api/sessions/99999");

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

        // requester is authenticated, but tries to fetch owner's session id
        var client = api.CreateClient().AuthenticateAs(requesterClerkUserId);

        var response = await client.GetAsync($"/api/sessions/{ownerSession.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Returns_session_with_expected_data_and_drills()
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
            MidrangeMakes = 3,
            MidrangeAttempts = 10,
            ThreePointMakes = 2,
            ThreePointAttempts = 10,
            FreeThrowMakes = 8,
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

        var response = await client.GetAsync($"/api/sessions/{session.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<GetSession.Response>();

        result.Should().NotBeNull();
        result.Id.Should().Be(session.Id);
        result.Date.Should().Be(new DateOnly(2026, 1, 10));
        result.PaintMakes.Should().Be(5);
        result.PaintAttempts.Should().Be(10);
        result.PaintShotPercentage.Should().Be(50);
        result.MidrangeMakes.Should().Be(3);
        result.MidrangeAttempts.Should().Be(10);
        result.MidrangeShotPercentage.Should().Be(30);
        result.ThreePointMakes.Should().Be(2);
        result.ThreePointAttempts.Should().Be(10);
        result.ThreePointShotPercentage.Should().Be(20);
        result.FreeThrowMakes.Should().Be(8);
        result.FreeThrowAttempts.Should().Be(10);
        result.FreeThrowShotPercentage.Should().Be(80);
        result.OverallMakes.Should().Be(18);
        result.OverallAttempts.Should().Be(40);
        result.OverallShotPercentage.Should().Be(45);

        result.Drills.Should().HaveCount(2);
        result.Drills.Should().Contain(d => d.Name == "4x4" && d.CompletionTimeInSeconds == 150);
        result.Drills.Should().Contain(d => d.Name == "Suicides" && d.CompletionTimeInSeconds == 120);
    }

    [Fact]
    public async Task Returns_empty_drills_list_when_session_has_no_drills()
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
        // deliberately no Drills added

        var client = api.CreateClient().AuthenticateAs(clerkUserId);

        var response = await client.GetAsync($"/api/sessions/{session.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var rawJson = await response.Content.ReadAsStringAsync();
        using var document = JsonDocument.Parse(rawJson);
        var drillsElement = document.RootElement.GetProperty("drills");

        drillsElement.ValueKind.Should().Be(JsonValueKind.Array,
            "the API should serialize an empty drills collection as [] , not null");
        drillsElement.GetArrayLength().Should().Be(0);

        var result = await response.Content.ReadFromJsonAsync<GetSession.Response>();
        result.Should().NotBeNull();
        result.Drills.Should().NotBeNull();
        result.Drills.Should().BeEmpty();
    }

    [Fact]
    public async Task Returns_zero_percentages_when_all_attempts_are_zero()
    {
        const string clerkUserId = "clerk_user_4";

        using var scope = api.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var player = new Player { ClerkUserId = clerkUserId };
        db.Players.Add(player);
        await db.SaveChangesAsync();

        var session = new Session
        {
            PlayerId = player.Id,
            Date = new DateOnly(2026, 1, 10),
            PaintMakes = 0,
            PaintAttempts = 0,
            MidrangeMakes = 0,
            MidrangeAttempts = 0,
            ThreePointMakes = 0,
            ThreePointAttempts = 0,
            FreeThrowMakes = 0,
            FreeThrowAttempts = 0
        };
        db.Sessions.Add(session);
        await db.SaveChangesAsync();

        var client = api.CreateClient().AuthenticateAs(clerkUserId);

        var response = await client.GetAsync($"/api/sessions/{session.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<GetSession.Response>();

        result.Should().NotBeNull();
        result.PaintAttempts.Should().Be(0);
        result.PaintShotPercentage.Should().Be(0);
        result.MidrangeAttempts.Should().Be(0);
        result.MidrangeShotPercentage.Should().Be(0);
        result.ThreePointAttempts.Should().Be(0);
        result.ThreePointShotPercentage.Should().Be(0);
        result.FreeThrowAttempts.Should().Be(0);
        result.FreeThrowShotPercentage.Should().Be(0);
        result.OverallAttempts.Should().Be(0);
        result.OverallShotPercentage.Should().Be(0);
    }
}