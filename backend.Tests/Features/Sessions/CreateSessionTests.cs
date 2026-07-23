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

public class CreateSessionTests(ApiFixture api) : IClassFixture<ApiFixture>, IAsyncLifetime
{
    public Task InitializeAsync() => api.ResetAsync();
    public Task DisposeAsync() => Task.CompletedTask;

    private static CreateSession.Request ValidRequest() => new(
        Date: new DateOnly(2026, 1, 10),
        PaintMakes: 5,
        PaintAttempts: 10,
        MidrangeMakes: 3,
        MidrangeAttempts: 10,
        ThreePointMakes: 2,
        ThreePointAttempts: 10,
        FreeThrowMakes: 8,
        FreeThrowAttempts: 10,
        Drills: [new CreateSession.DrillRequest("4x4", 150)]
    );

    [Fact]
    public async Task Returns_unauthorized_when_no_token()
    {
        var client = api.CreateClient();

        var response = await client.PostAsJsonAsync("/api/sessions", ValidRequest());

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Returns_not_found_when_no_matching_player()
    {
        var client = api.CreateClient().AuthenticateAs("clerk_unknown_user");

        var response = await client.PostAsJsonAsync("/api/sessions", ValidRequest());

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Returns_bad_request_when_body_is_invalid()
    {
        const string clerkUserId = "clerk_user_1";
        using var scope = api.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Players.Add(new Player { ClerkUserId = clerkUserId });
        await db.SaveChangesAsync();

        var client = api.CreateClient().AuthenticateAs(clerkUserId);

        // PaintMakes > PaintAttempts is invalid
        var invalidRequest = ValidRequest() with { PaintMakes = 11, PaintAttempts = 10 };

        var response = await client.PostAsJsonAsync("/api/sessions", invalidRequest);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Returns_created_with_expected_data_and_drills()
    {
        const string clerkUserId = "clerk_user_2";
        using var scope = api.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var player = new Player { ClerkUserId = clerkUserId };
        db.Players.Add(player);
        await db.SaveChangesAsync();

        var client = api.CreateClient().AuthenticateAs(clerkUserId);

        var request = ValidRequest();
        var response = await client.PostAsJsonAsync("/api/sessions", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();

        var result = await response.Content.ReadFromJsonAsync<CreateSession.Response>();

        result.Should().NotBeNull();
        response.Headers.Location.Should().Be(new Uri($"/api/sessions/{result.Id}", UriKind.Relative));
        result.Id.Should().BeGreaterThan(0);
        result.Date.Should().Be(request.Date);
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

        result.Drills.Should().HaveCount(1);
        result.Drills.Should().Contain(d => d.Name == "4x4" && d.CompletionTimeInSeconds == 150);

        using var verifyScope = api.Services.CreateScope();
        var verifyDb = verifyScope.ServiceProvider.GetRequiredService<AppDbContext>();
        var persisted = await verifyDb.Sessions.Include(s => s.Drills).FirstOrDefaultAsync(s => s.Id == result.Id);
        persisted.Should().NotBeNull();
        persisted.PlayerId.Should().Be(player.Id);
        persisted.Drills.Should().HaveCount(1);
    }

    [Fact]
    public async Task Returns_created_with_zero_percentages_when_all_attempts_are_zero()
    {
        const string clerkUserId = "clerk_user_zero";
        using var scope = api.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Players.Add(new Player { ClerkUserId = clerkUserId });
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

        var response = await client.PostAsJsonAsync("/api/sessions", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var result = await response.Content.ReadFromJsonAsync<CreateSession.Response>();

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
    public async Task Returns_created_with_empty_drills_when_none_provided()
    {
        const string clerkUserId = "clerk_user_no_drills";
        using var scope = api.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Players.Add(new Player { ClerkUserId = clerkUserId });
        await db.SaveChangesAsync();

        var client = api.CreateClient().AuthenticateAs(clerkUserId);

        var request = ValidRequest() with { Drills = [] };

        var response = await client.PostAsJsonAsync("/api/sessions", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var rawJson = await response.Content.ReadAsStringAsync();
        using var document = JsonDocument.Parse(rawJson);
        var drillsElement = document.RootElement.GetProperty("drills");

        drillsElement.ValueKind.Should().Be(JsonValueKind.Array,
            "the API should serialize an empty drills collection as [], not null");
        drillsElement.GetArrayLength().Should().Be(0);

        var result = await response.Content.ReadFromJsonAsync<CreateSession.Response>();
        result.Should().NotBeNull();
        result.Drills.Should().NotBeNull();
        result.Drills.Should().BeEmpty();
    }

    [Fact]
    public async Task Returns_bad_request_when_date_is_in_the_future()
    {
        const string clerkUserId = "clerk_user_future_date";
        using var scope = api.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Players.Add(new Player { ClerkUserId = clerkUserId });
        await db.SaveChangesAsync();

        var client = api.CreateClient().AuthenticateAs(clerkUserId);

        var request = ValidRequest() with { Date = DateOnly.FromDateTime(DateTime.Today.AddDays(2)) };

        var response = await client.PostAsJsonAsync("/api/sessions", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Returns_bad_request_when_drill_name_is_empty()
    {
        const string clerkUserId = "clerk_user_empty_drill_name";
        using var scope = api.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Players.Add(new Player { ClerkUserId = clerkUserId });
        await db.SaveChangesAsync();

        var client = api.CreateClient().AuthenticateAs(clerkUserId);

        var request = ValidRequest() with
        {
            Drills = [new CreateSession.DrillRequest("", 150)]
        };

        var response = await client.PostAsJsonAsync("/api/sessions", request);

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
        db.Players.Add(new Player { ClerkUserId = clerkUserId });
        await db.SaveChangesAsync();

        var client = api.CreateClient().AuthenticateAs(clerkUserId);

        var request = ValidRequest() with
        {
            Drills = [new CreateSession.DrillRequest("4x4", completionTime)]
        };

        var response = await client.PostAsJsonAsync("/api/sessions", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}