using Backend.Dtos;

namespace backend.Services;

public interface ISessionService
{
    Task<IReadOnlyList<SessionResponse>> ListByPlayerAsync(int playerId);
}
