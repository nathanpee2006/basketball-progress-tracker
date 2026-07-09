using Backend.Common.Services;
using Backend.Data;
using Backend.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Backend.Tests.Services;

public class PlayerServiceTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly AppDbContext _db;

    public PlayerServiceTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;

        _db = new AppDbContext(options);
        _db.Database.EnsureCreated();
    }

    public void Dispose()
    {
        _db.Dispose();
        _connection.Dispose();
    }

    [Fact]
    public async Task GetOrCreateAsync_NewUser_CreatesPlayer()
    {
        var service = new PlayerService(_db);

        var player = await service.GetOrCreateAsync("clerk_abc123", "Pacific/Auckland");

        Assert.Equal("clerk_abc123", player.ClerkUserId);
        Assert.Equal("Pacific/Auckland", player.TimeZone);
        Assert.Equal(1, await _db.Players.CountAsync());
    }

    [Fact]
    public async Task GetOrCreateAsync_ExistingUser_ReturnsExistingUser()
    {
        _db.Players.Add(new Player { ClerkUserId = "clerk_abc123", TimeZone = "UTC" });
        await _db.SaveChangesAsync();

        var service = new PlayerService(_db);
        var player = await service.GetOrCreateAsync("clerk_abc123", "UTC");

        Assert.Equal("clerk_abc123", player.ClerkUserId);
        Assert.Equal(1, await _db.Players.CountAsync()); // this is the part that actually matters
    }

    [Fact]
    public async Task GetOrCreateAsync_ExistingPlayerWithDifferentTimeZone_UpdatesTimeZone()
    {
        _db.Players.Add(new Player { ClerkUserId = "clerk_abc123", TimeZone = "Pacific/Auckland" });
        await _db.SaveChangesAsync();

        var service = new PlayerService(_db);
        var player = await service.GetOrCreateAsync("clerk_abc123", "America/Los_Angeles");

        Assert.Equal("America/Los_Angeles", player.TimeZone);
        Assert.Equal(1, await _db.Players.CountAsync());
    }

    [Fact]
    public async Task GetOrCreateAsync_ExistingPlayerNullTimeZone_KeepsExistingTimeZone()
    {
        _db.Players.Add(new Player { ClerkUserId = "clerk_abc123", TimeZone = "Pacific/Auckland" });
        await _db.SaveChangesAsync();

        var service = new PlayerService(_db);
        var player = await service.GetOrCreateAsync("clerk_abc123", null);

        Assert.Equal("Pacific/Auckland", player.TimeZone);
    }

    [Fact]
    public async Task GetOrCreateAsync_NewUserNullTimeZone_DefaultsToUtc()
    {
        var service = new PlayerService(_db);
        var player = await service.GetOrCreateAsync("clerk_abc123", null);

        Assert.Equal("UTC", player.TimeZone);
    }
}
