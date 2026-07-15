using Backend.Data.Models;

namespace Backend.Common.Services;

public interface IPlayerService
{
    Task<Player?> GetByClerkUserIdAsync(string clerkUserId);
}