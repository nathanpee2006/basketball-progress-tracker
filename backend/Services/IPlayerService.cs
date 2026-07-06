using Backend.Models;

namespace backend.Services;

public interface IPlayerService
{
    Task<Player> GetOrCreateAsync(string clerkUserId, string? timeZoneId);
}