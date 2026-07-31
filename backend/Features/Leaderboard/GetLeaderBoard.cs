using System.Security.Claims;
using Backend.Common.Endpoints;
using Backend.Data;
using Microsoft.EntityFrameworkCore;

namespace backend.Features.Leaderboard;

public static class GetLeaderBoard
{
    public record LeaderboardEntry(
        int PlayerId,
        int? Rank,
        string PlayerName,
        string? ImageUrl,
        int ShotPercentage,
        bool IsQualified
    );

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("leaderboard", Handler)
                .WithTags("Leaderboard")
                .Produces<IEnumerable<LeaderboardEntry>>()
                .Produces(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status404NotFound)
                .RequireAuthorization();
        }
    }

    public static async Task<IResult> Handler(string? zone, AppDbContext context,
        ClaimsPrincipal user)
    {
        var clerkUserId = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (clerkUserId is null)
        {
            return TypedResults.Unauthorized();
        }

        DateOnly startWeek = GetStartOfWeek(DateTime.UtcNow);
        DateOnly endWeek = GetEndOfWeek(DateTime.UtcNow);


        if (zone is not null)
        {
            // Process the zone-specific logic
            switch (zone.ToLower())
            {
                case "paint":

                    var paintWeeklyTotals = await context.Sessions
                        .Where(s => s.Date >= startWeek && s.Date < endWeek)
                        .GroupBy(s => s.PlayerId)
                        .Select(g => new
                        {
                            PlayerId = g.Key,
                            PlayerName = g.First().Player.FirstName + " " + g.First().Player.LastName,
                            g.First().Player.ImageUrl,
                            TotalPaintMakes = g.Sum(s => s.PaintMakes),
                            TotalPaintAttempts = g.Sum(s => s.PaintAttempts),
                        })
                        .ToListAsync();

                    var paintWeeklyShotPercentage = paintWeeklyTotals
                        .Select(r => new
                        {
                            r.PlayerId,
                            r.PlayerName,
                            r.ImageUrl,
                            ShotPercentage = (int)Math.Round((double)r.TotalPaintMakes / r.TotalPaintAttempts * 100),
                            IsQualified = r.TotalPaintAttempts >= 20
                        })
                        .OrderByDescending(r => r.IsQualified)
                        .ThenByDescending(r => r.ShotPercentage)
                        .ToList();

                    var paintRankedWeeklyShotPercentage = paintWeeklyShotPercentage
                        .Select((entry, index) => new LeaderboardEntry(
                            PlayerId: entry.PlayerId,
                            Rank: entry.IsQualified ? index + 1 : null,
                            PlayerName: entry.PlayerName,
                            ImageUrl: entry.ImageUrl,
                            ShotPercentage: entry.ShotPercentage,
                            IsQualified: entry.IsQualified
                        ))
                        .ToList();

                    return TypedResults.Ok(paintRankedWeeklyShotPercentage);

                case "midrange":

                    var midrangeWeeklyTotals = await context.Sessions
                        .Where(s => s.Date >= startWeek && s.Date < endWeek)
                        .GroupBy(s => s.PlayerId)
                        .Select(g => new
                        {
                            PlayerId = g.Key,
                            PlayerName = g.First().Player.FirstName + " " + g.First().Player.LastName,
                            g.First().Player.ImageUrl,
                            TotalMidrangeMakes = g.Sum(s => s.MidrangeMakes),
                            TotalMidrangeAttempts = g.Sum(s => s.MidrangeAttempts),
                        })
                        .ToListAsync();

                    var midrangeWeeklyShotPercentage = midrangeWeeklyTotals
                        .Select(r => new
                        {
                            r.PlayerId,
                            r.PlayerName,
                            r.ImageUrl,
                            ShotPercentage =
                                (int)Math.Round((double)r.TotalMidrangeMakes / r.TotalMidrangeAttempts * 100),
                            IsQualified = r.TotalMidrangeAttempts >= 20
                        })
                        .OrderByDescending(r => r.IsQualified)
                        .ThenByDescending(r => r.ShotPercentage)
                        .ToList();

                    var midrangeRankedWeeklyShotPercentage = midrangeWeeklyShotPercentage
                        .Select((entry, index) => new LeaderboardEntry(
                            PlayerId: entry.PlayerId,
                            Rank: entry.IsQualified ? index + 1 : null,
                            PlayerName: entry.PlayerName,
                            ImageUrl: entry.ImageUrl,
                            ShotPercentage: entry.ShotPercentage,
                            IsQualified: entry.IsQualified
                        ))
                        .ToList();

                    return TypedResults.Ok(midrangeRankedWeeklyShotPercentage);
                case "threepoint":

                    var threePointWeeklyTotals = await context.Sessions
                        .Where(s => s.Date >= startWeek && s.Date < endWeek)
                        .GroupBy(s => s.PlayerId)
                        .Select(g => new
                        {
                            PlayerId = g.Key,
                            PlayerName = g.First().Player.FirstName + " " + g.First().Player.LastName,
                            g.First().Player.ImageUrl,
                            TotalThreePointMakes = g.Sum(s => s.ThreePointMakes),
                            TotalThreePointAttempts = g.Sum(s => s.ThreePointAttempts),
                        })
                        .ToListAsync();

                    var threePointWeeklyShotPercentage = threePointWeeklyTotals
                        .Select(r => new
                        {
                            r.PlayerId,
                            r.PlayerName,
                            r.ImageUrl,
                            ShotPercentage =
                                (int)Math.Round((double)r.TotalThreePointMakes / r.TotalThreePointAttempts * 100),
                            IsQualified = r.TotalThreePointAttempts >= 20
                        })
                        .OrderByDescending(r => r.IsQualified)
                        .ThenByDescending(r => r.ShotPercentage)
                        .ToList();

                    var threePointRankedWeeklyShotPercentage = threePointWeeklyShotPercentage
                        .Select((entry, index) => new LeaderboardEntry(
                            PlayerId: entry.PlayerId,
                            Rank: entry.IsQualified ? index + 1 : null,
                            PlayerName: entry.PlayerName,
                            ImageUrl: entry.ImageUrl,
                            ShotPercentage: entry.ShotPercentage,
                            IsQualified: entry.IsQualified
                        ))
                        .ToList();

                    return TypedResults.Ok(threePointRankedWeeklyShotPercentage);

                case "freethrow":

                    var freeThrowWeeklyTotals = await context.Sessions
                        .Where(s => s.Date >= startWeek && s.Date < endWeek)
                        .GroupBy(s => s.PlayerId)
                        .Select(g => new
                        {
                            PlayerId = g.Key,
                            PlayerName = g.First().Player.FirstName + " " + g.First().Player.LastName,
                            g.First().Player.ImageUrl,
                            TotalFreeThrowMakes = g.Sum(s => s.FreeThrowMakes),
                            TotalFreeThrowAttempts = g.Sum(s => s.FreeThrowAttempts),
                        })
                        .ToListAsync();

                    var freeThrowWeeklyShotPercentage = freeThrowWeeklyTotals
                        .Select(r => new
                        {
                            r.PlayerId,
                            r.PlayerName,
                            r.ImageUrl,
                            ShotPercentage =
                                (int)Math.Round((double)r.TotalFreeThrowMakes / r.TotalFreeThrowAttempts * 100),
                            IsQualified = r.TotalFreeThrowAttempts >= 20
                        })
                        .OrderByDescending(r => r.IsQualified)
                        .ThenByDescending(r => r.ShotPercentage)
                        .ToList();

                    var freeThrowRankedWeeklyShotPercentage = freeThrowWeeklyShotPercentage
                        .Select((entry, index) => new LeaderboardEntry(
                            PlayerId: entry.PlayerId,
                            Rank: entry.IsQualified ? index + 1 : null,
                            PlayerName: entry.PlayerName,
                            ImageUrl: entry.ImageUrl,
                            ShotPercentage: entry.ShotPercentage,
                            IsQualified: entry.IsQualified
                        ))
                        .ToList();

                    return TypedResults.Ok(freeThrowRankedWeeklyShotPercentage);

                default:
                    return TypedResults.BadRequest("Invalid zone parameter.");
            }
        }

        // Default leaderboard logic (weekly overall shot percentage of all players)
        var weeklyTotals = await context.Sessions
            .Where(s => s.Date >= startWeek && s.Date < endWeek)
            .GroupBy(s => s.PlayerId)
            .Select(g => new
            {
                PlayerId = g.Key,
                PlayerName = g.First().Player.FirstName + " " + g.First().Player.LastName,
                g.First().Player.ImageUrl,
                TotalPaintMakes = g.Sum(s => s.PaintMakes),
                TotalPaintAttempts = g.Sum(s => s.PaintAttempts),
                TotalMidrangeMakes = g.Sum(s => s.MidrangeMakes),
                TotalMidrangeAttempts = g.Sum(s => s.MidrangeAttempts),
                TotalThreePointMakes = g.Sum(s => s.ThreePointMakes),
                TotalThreePointAttempts = g.Sum(s => s.ThreePointAttempts),
                TotalFreeThrowMakes = g.Sum(s => s.FreeThrowMakes),
                TotalFreeThrowAttempts = g.Sum(s => s.FreeThrowAttempts),
            })
            .ToListAsync();

        var weeklyOverallShotPercentage = weeklyTotals
            .Select(r => new
            {
                r.PlayerId,
                r.PlayerName,
                r.ImageUrl,
                ShotPercentage = (int)Math.Round((double)(r.TotalPaintMakes + r.TotalMidrangeMakes +
                                                          r.TotalThreePointMakes +
                                                          r.TotalFreeThrowMakes) /
                    (r.TotalPaintAttempts + r.TotalMidrangeAttempts +
                     r.TotalThreePointAttempts + r.TotalFreeThrowAttempts) * 100),
                IsQualified = (r.TotalPaintAttempts + r.TotalMidrangeAttempts + r.TotalThreePointAttempts +
                               r.TotalFreeThrowAttempts) >= 20
            })
            .OrderByDescending(r => r.IsQualified)
            .ThenByDescending(r => r.ShotPercentage)
            .ToList();

        var rankedWeeklyOverallShotPercentage = weeklyOverallShotPercentage
            .Select((entry, index) => new LeaderboardEntry(
                PlayerId: entry.PlayerId,
                Rank: entry.IsQualified ? index + 1 : null,
                PlayerName: entry.PlayerName,
                ImageUrl: entry.ImageUrl,
                ShotPercentage: entry.ShotPercentage,
                IsQualified: entry.IsQualified
            ))
            .ToList();
        
        return TypedResults.Ok(rankedWeeklyOverallShotPercentage);
    }


    // Returns Monday date of the current week based on the current date
    private static DateOnly GetStartOfWeek(DateTime date)
    {
        int daysSinceMonday = ((int)date.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
        DateTime currentWeekStartDate = date.AddDays(-daysSinceMonday);
        return DateOnly.FromDateTime(currentWeekStartDate);
    }

    // Returns Monday date of the next week based on the current date
    private static DateOnly GetEndOfWeek(DateTime date)
    {
        int daysTillNextWeekMonday = ((int)DayOfWeek.Monday - (int)date.DayOfWeek + 7) % 7;
        if (daysTillNextWeekMonday == 0)
            daysTillNextWeekMonday = 7;

        DateTime nextWeekStartDate = date.AddDays(daysTillNextWeekMonday);
        return DateOnly.FromDateTime(nextWeekStartDate);
    }
}