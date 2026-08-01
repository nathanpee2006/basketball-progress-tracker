using System.Security.Claims;
using Backend.Common.Endpoints;
using Backend.Common.Services;
using Backend.Data;
using backend.Features.Common;
using Microsoft.EntityFrameworkCore;

namespace backend.Features.Achievements;

public static class GetAchievements
{
    public record Achievement(
        string Key,
        string Name,
        string Description,
        string Trigger,
        DateTime? AchievedAt,
        int Progress
    );

    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("achievements", Handler)
                .WithTags("Achievements").Produces<List<Achievement>>()
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status404NotFound)
                .RequireAuthorization();
        }
    }

    public static async Task<IResult> Handler(
        AppDbContext context,
        IPlayerService playerService,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        var clerkUserId = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (clerkUserId is null)
        {
            return TypedResults.Unauthorized();
        }

        var player = await playerService.GetByClerkUserIdAsync(clerkUserId);
        if (player is null)
        {
            return TypedResults.NotFound();
        }

        var achievements = await context.Achievements
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var playerAchievements = await context.PlayerAchievements
            .AsNoTracking()
            .Where(pa => pa.PlayerId == player.Id)
            .ToDictionaryAsync(pa => pa.AchievementId, pa => pa.AchievedAt, cancellationToken);

        var totals = await context.Sessions
                         .AsNoTracking()
                         .Where(s => s.PlayerId == player.Id)
                         .GroupBy(s => 1)
                         .Select(g => new AchievementLogic.SessionTotals(
                             g.Sum(s => s.PaintMakes),
                             g.Sum(s => s.MidrangeMakes),
                             g.Sum(s => s.ThreePointMakes),
                             g.Sum(s => s.FreeThrowMakes),
                             g.Count()
                         ))
                         .FirstOrDefaultAsync(cancellationToken)
                     ?? new AchievementLogic.SessionTotals(0, 0, 0, 0, 0);

        var bestSingleSessionTotal = await context.Sessions
            .AsNoTracking()
            .Where(s => s.PlayerId == player.Id)
            .Select(s => s.PaintMakes + s.MidrangeMakes + s.ThreePointMakes + s.FreeThrowMakes)
            .OrderByDescending(total => total)
            .FirstOrDefaultAsync(cancellationToken);

        var response = achievements.Select(a =>
        {
            DateTime? achievedAt = playerAchievements.TryGetValue(a.Id, out var value) ? value : null;
            var currentValue = AchievementLogic.GetCurrentValue(a.Key, totals, bestSingleSessionTotal);
            var progress = achievedAt is not null
                ? 100
                : a.Threshold == 0
                    ? 0
                    : Math.Min(100, (int)Math.Round((double)currentValue / a.Threshold * 100));

            return new Achievement(a.Key, a.Name, a.Description, a.Trigger, achievedAt, progress);
        }).ToList();

        return TypedResults.Ok(response);
    }


}