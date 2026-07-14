using Backend.Data.Models;

namespace Backend.Common.Services;

public interface IPlayerService
{
    Task<Player> GetOrCreateAsync(string clerkUserId, string? timeZoneId);
}