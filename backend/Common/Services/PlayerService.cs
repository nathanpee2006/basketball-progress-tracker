using Backend.Data;
using Backend.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Common.Services;

public class PlayerService : IPlayerService
{
    private readonly AppDbContext _db;

    public PlayerService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Player> GetOrCreateAsync(string clerkUserId, string? timeZoneId)
    {
        var validatedTimeZone = ValidateTimeZone(timeZoneId);

        var player = await _db.Players
            .FirstOrDefaultAsync(u => u.ClerkUserId == clerkUserId);

        if (player is not null)
        {
            if (validatedTimeZone is not null && player.TimeZone != validatedTimeZone)
            {
                player.TimeZone = validatedTimeZone;
                await _db.SaveChangesAsync();
            }

            return player;
        }

        player = new Player
        {
            ClerkUserId = clerkUserId,
            TimeZone = validatedTimeZone ?? "UTC"
        };
        _db.Players.Add(player);

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            player = await _db.Players
                .FirstAsync(u => u.ClerkUserId == clerkUserId);
        }

        return player;
    }

    private static string? ValidateTimeZone(string? timeZoneId)
    {
        if (string.IsNullOrWhiteSpace(timeZoneId)) return null;
        try
        {
            TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            return timeZoneId;
        }
        catch (TimeZoneNotFoundException)
        {
            return null;
        }
        catch (InvalidTimeZoneException)
        {
            return null;
        }
    }
}