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

    public async Task<Player?> GetByClerkUserIdAsync(string clerkUserId)
    {
        var player = await _db.Players.FirstOrDefaultAsync(u => u.ClerkUserId == clerkUserId);
        return player;
    }
}