using Backend.Data;
using Backend.Dtos;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class SessionService : ISessionService
{
    private readonly AppDbContext _db;

    public SessionService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<SessionResponse>> ListByPlayerAsync(int playerId)
    {
        return await _db.Sessions
            .AsNoTracking()
            .Where(s => s.PlayerId == playerId)
            .OrderByDescending(s => s.Date)
            .ThenByDescending(s => s.CreatedAt)
            .Select(s => new SessionResponse(
                s.Id,
                s.Date,
                s.PaintMakes,
                s.PaintAttempts,
                s.MidrangeMakes,
                s.MidrangeAttempts,
                s.ThreePointMakes,
                s.ThreePointAttempts,
                s.FreeThrowMakes,
                s.FreeThrowAttempts,
                s.CreatedAt
            ))
            .ToListAsync();
    }
}
