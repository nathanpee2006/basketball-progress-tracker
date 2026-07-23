using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Backend.Data;
using Backend.Data.Models;
using Backend.Features.Sessions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace backend.Tests.Features.Sessions;

public class UpdateSessionTests(ApiFixture api) : IClassFixture<ApiFixture>, IAsyncLifetime
{
    public Task InitializeAsync() => api.ResetAsync();
    public Task DisposeAsync() => Task.CompletedTask;

    private static UpdateSession.Request ValidRequest() => new(
        Date: new DateOnly(2026, 1, 10),
        PaintMakes: 10,
        PaintAttempts: 10,
        MidrangeMakes: 3,
        MidrangeAttempts: 10,
        ThreePointMakes: 5,
        ThreePointAttempts: 10,
        FreeThrowMakes: 8,
        FreeThrowAttempts: 10,
        Drills: [new UpdateSession.DrillRequest("4x4", 150)]
    );

    private static UpdateSession.Response ValidResponse(int sessionId, int drillId)
    {
        var request = ValidRequest();

        var overallMakes = request.PaintMakes + request.MidrangeMakes +
                           request.ThreePointMakes + request.FreeThrowMakes;
        var overallAttempts = request.PaintAttempts + request.MidrangeAttempts +
                              request.ThreePointAttempts + request.FreeThrowAttempts;

        static int Percentage(int makes, int attempts) =>
            attempts != 0 ? (int)Math.Round((double)makes / attempts * 100) : 0;

        return new(
            Id: sessionId,
            Date: request.Date,
            PaintMakes: request.PaintMakes,
            PaintAttempts: request.PaintAttempts,
            PaintShotPercentage: Percentage(request.PaintMakes, request.PaintAttempts),
            MidrangeMakes: request.MidrangeMakes,
            MidrangeAttempts: request.MidrangeAttempts,
            MidrangeShotPercentage: Percentage(request.MidrangeMakes, request.MidrangeAttempts),
            ThreePointMakes: request.ThreePointMakes,
            ThreePointAttempts: request.ThreePointAttempts,
            ThreePointShotPercentage: Percentage(request.ThreePointMakes, request.ThreePointAttempts),
            FreeThrowMakes: request.FreeThrowMakes,
            FreeThrowAttempts: request.FreeThrowAttempts,
            FreeThrowShotPercentage: Percentage(request.FreeThrowMakes, request.FreeThrowAttempts),
            OverallMakes: overallMakes,
            OverallAttempts: overallAttempts,
            OverallShotPercentage: Percentage(overallMakes, overallAttempts),
            Drills:
            [
                new UpdateSession.DrillResponse(drillId, request.Drills[0].Name,
                    request.Drills[0].CompletionTimeInSeconds)
            ]
        );
    }

    private static Session ExistingSession(int playerId) => new()
    {
        PlayerId = playerId,
        Date = new DateOnly(2026, 1, 1),
        PaintMakes = 1,
        PaintAttempts = 4,
        MidrangeMakes = 1,
        MidrangeAttempts = 4,
        ThreePointMakes = 1,
        ThreePointAttempts = 4,
        FreeThrowMakes = 1,
        FreeThrowAttempts = 4,
        Drills = [new Drill { Name = "Old Drill", CompletionTimeInSeconds = 60 }]
    };

    [Fact]
    public async Task Returns_unauthorized_when_no_token()
    {
        var client = api.CreateClient();

        var response = await client.PutAsJsonAsync("/api/sessions/1", ValidRequest());

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Returns_not_found_when_no_matching_player()
    {
        var client = api.CreateClient().AuthenticateAs("clerk_unknown_user");

        var response = await client.PutAsJsonAsync("/api/sessions/1", ValidRequest());

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Returns_bad_request_when_body_is_invalid()
    {
        const string clerkUserId = "clerk_user_1";
        using var scope = api.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var player = new Player { ClerkUserId = clerkUserId };
        db.Players.Add(player);
        await db.SaveChangesAsync();

        var session = ExistingSession(player.Id);
        db.Sessions.Add(session);
        await db.SaveChangesAsync();

        var client = api.CreateClient().AuthenticateAs(clerkUserId);

        // PaintMakes > PaintAttempts is invalid
        var invalidRequest = ValidRequest() with { PaintMakes = 11, PaintAttempts = 10 };

        var response = await client.PutAsJsonAsync($"/api/sessions/{session.Id}", invalidRequest);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Returns_not_found_when_no_matching_session()
    {
        const string clerkUserId = "clerk_user_no_session";
        using var scope = api.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Players.Add(new Player { ClerkUserId = clerkUserId });
        await db.SaveChangesAsync();

        var client = api.CreateClient().AuthenticateAs(clerkUserId);

        var response = await client.PutAsJsonAsync("/api/sessions/999999", ValidRequest());

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Returns_ok_with_updated_data_and_drills_when_session_exists()
    {
        const string clerkUserId = "clerk_user_2";
        using var scope = api.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var player = new Player { ClerkUserId = clerkUserId };
        db.Players.Add(player);
        await db.SaveChangesAsync();

        var session = ExistingSession(player.Id);
        db.Sessions.Add(session);
        await db.SaveChangesAsync();

        var client = api.CreateClient().AuthenticateAs(clerkUserId);

        var request = ValidRequest();
        var response = await client.PutAsJsonAsync($"/api/sessions/{session.Id}", request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<UpdateSession.Response>();

        var expected = ValidResponse(session.Id, result!.Drills[0].Id);

        result.Should().NotBeNull();
        result.Id.Should().Be(expected.Id);
        result.Date.Should().Be(expected.Date);
        result.PaintMakes.Should().Be(expected.PaintMakes);
        result.PaintAttempts.Should().Be(expected.PaintAttempts);
        result.PaintShotPercentage.Should().Be(expected.PaintShotPercentage);
        result.MidrangeMakes.Should().Be(expected.MidrangeMakes);
        result.MidrangeAttempts.Should().Be(expected.MidrangeAttempts);
        result.MidrangeShotPercentage.Should().Be(expected.MidrangeShotPercentage);
        result.ThreePointMakes.Should().Be(expected.ThreePointMakes);
        result.ThreePointAttempts.Should().Be(expected.ThreePointAttempts);
        result.ThreePointShotPercentage.Should().Be(expected.ThreePointShotPercentage);
        result.FreeThrowMakes.Should().Be(expected.FreeThrowMakes);
        result.FreeThrowAttempts.Should().Be(expected.FreeThrowAttempts);
        result.FreeThrowShotPercentage.Should().Be(expected.FreeThrowShotPercentage);
        result.OverallMakes.Should().Be(expected.OverallMakes);
        result.OverallAttempts.Should().Be(expected.OverallAttempts);
        result.OverallShotPercentage.Should().Be(expected.OverallShotPercentage);

        result.Drills.Should().HaveCount(1);
        result.Drills.Should().Contain(d => d.Name == "4x4" && d.CompletionTimeInSeconds == 150);

        using var verifyScope = api.Services.CreateScope();
        var verifyDb = verifyScope.ServiceProvider.GetRequiredService<AppDbContext>();
        var persisted = await verifyDb.Sessions.Include(s => s.Drills)
            .FirstOrDefaultAsync(s => s.Id == session.Id);

        persisted.Should().NotBeNull();
        // old drill should be gone, not just appended
        persisted.Drills.Should().NotContain(d => d.Name == "Old Drill");
    }

    [Fact]
    public async Task Returns_not_found_when_session_belongs_to_different_player()
    {
        const string ownerClerkId = "clerk_owner";
        const string attackerClerkId = "clerk_attacker";

        using var scope = api.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var owner = new Player { ClerkUserId = ownerClerkId };
        var attacker = new Player { ClerkUserId = attackerClerkId };
        db.Players.AddRange(owner, attacker);
        await db.SaveChangesAsync();

        var session = ExistingSession(owner.Id);
        db.Sessions.Add(session);
        await db.SaveChangesAsync();

        var client = api.CreateClient().AuthenticateAs(attackerClerkId);

        var response = await client.PutAsJsonAsync($"/api/sessions/{session.Id}", ValidRequest());

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        using var verifyScope = api.Services.CreateScope();
        var verifyDb = verifyScope.ServiceProvider.GetRequiredService<AppDbContext>();
        var persisted = await verifyDb.Sessions.FirstAsync(s => s.Id == session.Id);
        persisted.Date.Should().Be(session.Date, "the attacker's update must not have been applied");
    }

    [Fact]
    public async Task Returns_bad_request_when_date_is_in_the_future()
    {
        const string clerkUserId = "clerk_user_future_date";
        using var scope = api.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var player = new Player { ClerkUserId = clerkUserId };
        db.Players.Add(player);
        await db.SaveChangesAsync();

        var session = ExistingSession(player.Id);
        db.Sessions.Add(session);
        await db.SaveChangesAsync();

        var client = api.CreateClient().AuthenticateAs(clerkUserId);

        var request = ValidRequest() with { Date = DateOnly.FromDateTime(DateTime.Today.AddDays(2)) };

        var response = await client.PutAsJsonAsync($"/api/sessions/{session.Id}", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Returns_bad_request_when_drill_name_is_empty()
    {
        const string clerkUserId = "clerk_user_empty_drill_name";
        using var scope = api.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var player = new Player { ClerkUserId = clerkUserId };
        db.Players.Add(player);
        await db.SaveChangesAsync();

        var session = ExistingSession(player.Id);
        db.Sessions.Add(session);
        await db.SaveChangesAsync();

        var client = api.CreateClient().AuthenticateAs(clerkUserId);

        var request = ValidRequest() with
        {
            Drills = [new UpdateSession.DrillRequest("", 150)]
        };

        var response = await client.PutAsJsonAsync($"/api/sessions/{session.Id}", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Returns_bad_request_when_drill_completion_time_is_zero_or_negative(int completionTime)
    {
        var clerkUserId = $"clerk_user_bad_drill_time_{completionTime}";
        using var scope = api.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var player = new Player { ClerkUserId = clerkUserId };
        db.Players.Add(player);
        await db.SaveChangesAsync();

        var session = ExistingSession(player.Id);
        db.Sessions.Add(session);
        await db.SaveChangesAsync();

        var client = api.CreateClient().AuthenticateAs(clerkUserId);

        var request = ValidRequest() with
        {
            Drills = [new UpdateSession.DrillRequest("4x4", completionTime)]
        };

        var response = await client.PutAsJsonAsync($"/api/sessions/{session.Id}", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Returns_ok_with_zero_percentages_when_all_attempts_are_zero()
    {
        const string clerkUserId = "clerk_user_zero";
        using var scope = api.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var player = new Player { ClerkUserId = clerkUserId };
        db.Players.Add(player);
        await db.SaveChangesAsync();

        var session = ExistingSession(player.Id);
        db.Sessions.Add(session);
        await db.SaveChangesAsync();

        var client = api.CreateClient().AuthenticateAs(clerkUserId);

        var request = ValidRequest() with
        {
            PaintMakes = 0,
            PaintAttempts = 0,
            MidrangeMakes = 0,
            MidrangeAttempts = 0,
            ThreePointMakes = 0,
            ThreePointAttempts = 0,
            FreeThrowMakes = 0,
            FreeThrowAttempts = 0
        };

        var response = await client.PutAsJsonAsync($"/api/sessions/{session.Id}", request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<UpdateSession.Response>();

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

    [Fact]
    public async Task Returns_ok_with_empty_drills_when_none_provided()
    {
        const string clerkUserId = "clerk_user_no_drills";
        using var scope = api.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var player = new Player { ClerkUserId = clerkUserId };
        db.Players.Add(player);
        await db.SaveChangesAsync();

        var session = ExistingSession(player.Id);
        db.Sessions.Add(session);
        await db.SaveChangesAsync();

        var client = api.CreateClient().AuthenticateAs(clerkUserId);

        var request = ValidRequest() with { Drills = [] };

        var response = await client.PutAsJsonAsync($"/api/sessions/{session.Id}", request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var rawJson = await response.Content.ReadAsStringAsync();
        using var document = JsonDocument.Parse(rawJson);
        var drillsElement = document.RootElement.GetProperty("drills");

        drillsElement.ValueKind.Should().Be(JsonValueKind.Array,
            "the API should serialize an empty drills collection as [], not null");
        drillsElement.GetArrayLength().Should().Be(0);

        var result = await response.Content.ReadFromJsonAsync<UpdateSession.Response>();
        result.Should().NotBeNull();
        result.Drills.Should().NotBeNull();
        result.Drills.Should().BeEmpty();

        using var verifyScope = api.Services.CreateScope();
        var verifyDb = verifyScope.ServiceProvider.GetRequiredService<AppDbContext>();
        var persisted = await verifyDb.Sessions.Include(s => s.Drills).FirstAsync(s => s.Id == session.Id);
        persisted.Drills.Should().BeEmpty("the old drill should have been removed, not left dangling");
    }

    [Fact]
    public async Task Removes_old_drills_from_database_when_replaced()
    {
        const string clerkUserId = "clerk_user_drill_replacement";
        using var scope = api.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var player = new Player { ClerkUserId = clerkUserId };
        db.Players.Add(player);
        await db.SaveChangesAsync();

        var session = ExistingSession(player.Id);
        db.Sessions.Add(session);
        await db.SaveChangesAsync();
        var oldDrillId = session.Drills.Single().Id;

        var client = api.CreateClient().AuthenticateAs(clerkUserId);

        var request = ValidRequest() with
        {
            Drills = [new UpdateSession.DrillRequest("New Drill", 90)]
        };

        var response = await client.PutAsJsonAsync($"/api/sessions/{session.Id}", request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        using var verifyScope = api.Services.CreateScope();
        var verifyDb = verifyScope.ServiceProvider.GetRequiredService<AppDbContext>();

        var oldDrillStillExists = await verifyDb.Drills.AnyAsync(d => d.Id == oldDrillId);
        oldDrillStillExists.Should().BeFalse("the old drill row should be deleted, not orphaned in the database");

        var persisted = await verifyDb.Sessions.Include(s => s.Drills).FirstAsync(s => s.Id == session.Id);
        persisted.Drills.Should().ContainSingle(d => d.Name == "New Drill" && d.CompletionTimeInSeconds == 90);
    }
}