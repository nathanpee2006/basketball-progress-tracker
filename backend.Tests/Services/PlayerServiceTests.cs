using backend.Services;
using Backend.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace backend.Tests;

public class PlayerServiceTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly AppDbContext _db;

    public PlayerServiceTests()
    {
        // In-memory SQLite requires an open connection to persist the DB for the test's lifetime
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;

        _db = new AppDbContext(options);
        _db.Database.EnsureCreated(); // builds schema from current model, including unique indexes
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
}
