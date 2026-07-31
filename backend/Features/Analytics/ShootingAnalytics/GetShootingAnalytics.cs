using System.Security.Claims;
using Backend.Common.Endpoints;
using Backend.Common.Services;
using Backend.Data;
using Backend.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Features.Analytics.ShootingAnalytics;

public static class GetShootingAnalytics
{
    public record ZoneStat(string Zone, int Attempts, int Makes, int Percentage);

    public record SessionZoneStats(int SessionId, DateOnly Date, IReadOnlyList<ZoneStat> Zones);

    public record FreeThrowTrendPoint(DateOnly Date, int Attempts, int Makes, int Percentage);

    public record SessionZoneData(
        int Id,
        DateOnly Date,
        int PaintMakes,
        int PaintAttempts,
        int MidrangeMakes,
        int MidrangeAttempts,
        int ThreePointMakes,
        int ThreePointAttempts,
        int FreeThrowMakes,
        int FreeThrowAttempts
    );

    public record ShootingAnalyticsResponse(
        IReadOnlyList<ZoneStat> ShootingByZone,
        IReadOnlyList<SessionZoneStats> ShootingByZonePerSession,
        string WeakestShootingZone,
        int FreeThrowPercentage,
        IReadOnlyList<FreeThrowTrendPoint> FreeThrowTrend,
        DateOnly? FromDate,
        DateOnly? ToDate
    );

    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("analytics/shooting", Handler)
                .WithTags("Analytics")
                .Produces<ShootingAnalyticsResponse>()
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status404NotFound)
                .RequireAuthorization();
        }
    }

    public static async Task<IResult> Handler(DateOnly? from, DateOnly? to, AppDbContext context,
        IPlayerService playerService, ClaimsPrincipal user,
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

        var sessions = await context.Sessions.AsNoTracking()
            .Where(s => s.PlayerId == player.Id
                        && (from == null || s.Date >= from) // no filter if null lower bound
                        && (to == null || s.Date <= to)) // no filter if null upper bound
            .OrderBy(s => s.Date)
            .Select(s => new SessionZoneData(
                s.Id, s.Date,
                s.PaintMakes, s.PaintAttempts,
                s.MidrangeMakes, s.MidrangeAttempts,
                s.ThreePointMakes, s.ThreePointAttempts,
                s.FreeThrowMakes, s.FreeThrowAttempts))
            .ToListAsync(cancellationToken);

        if (sessions.Count == 0)
        {
            return TypedResults.Ok(new ShootingAnalyticsResponse(
                ShootingByZone: [],
                ShootingByZonePerSession: [],
                WeakestShootingZone: string.Empty,
                FreeThrowPercentage: 0,
                FreeThrowTrend: [],
                FromDate: from,
                ToDate: to
            ));
        }

        var shootingByZone = ComputeShootingByZone(sessions);

        return TypedResults.Ok(new ShootingAnalyticsResponse(
            ShootingByZone: shootingByZone,
            ShootingByZonePerSession: ComputeShootingByZonePerSession(sessions),
            WeakestShootingZone: DetermineWeakestShootingZone(shootingByZone),
            FreeThrowPercentage: CalculateFreeThrowPercentage(sessions),
            FreeThrowTrend: ComputeFreeThrowTrend(sessions),
            FromDate: from,
            ToDate: to
        ));
    }

    private static IReadOnlyList<ZoneStat> ComputeShootingByZone(
        IEnumerable<SessionZoneData> sessions)
    {
        var list = sessions.ToList();

        int paintMakes = list.Sum(s => s.PaintMakes), paintAttempts = list.Sum(s => s.PaintAttempts);
        int midMakes = list.Sum(s => s.MidrangeMakes), midAttempts = list.Sum(s => s.MidrangeAttempts);
        int threeMakes = list.Sum(s => s.ThreePointMakes), threeAttempts = list.Sum(s => s.ThreePointAttempts);

        var zone = new List<ZoneStat>
        {
            new("paint", paintAttempts, paintMakes, Session.CalculateZoneShotPercentage(paintMakes, paintAttempts)),
            new("midrange", midAttempts, midMakes, Session.CalculateZoneShotPercentage(midMakes, midAttempts)),
            new("threePoint", threeAttempts, threeMakes, Session.CalculateZoneShotPercentage(threeMakes, threeAttempts))
        };
        return zone;
    }

    private static IReadOnlyList<SessionZoneStats> ComputeShootingByZonePerSession(
        IEnumerable<SessionZoneData> sessions)
    {
        return
        [
            .. sessions
                .Select(s => new SessionZoneStats(
                    s.Id,
                    s.Date,
                    new List<ZoneStat>
                    {
                        new("paint", s.PaintAttempts, s.PaintMakes,
                            Session.CalculateZoneShotPercentage(s.PaintMakes, s.PaintAttempts)),
                        new("midrange", s.MidrangeAttempts, s.MidrangeMakes,
                            Session.CalculateZoneShotPercentage(s.MidrangeMakes, s.MidrangeAttempts)),
                        new("threePoint", s.ThreePointAttempts, s.ThreePointMakes,
                            Session.CalculateZoneShotPercentage(s.ThreePointMakes, s.ThreePointAttempts)),
                    }))
        ];
    }

    private static string DetermineWeakestShootingZone(IReadOnlyList<ZoneStat> shootingByZone)
    {
        if (shootingByZone.Count == 0)
            return string.Empty;

        return shootingByZone 
            .OrderBy(z => z.Percentage)
            .ThenByDescending(z => z.Attempts) // tie-break: prefer the zone with more data
            .First()
            .Zone;
    }

    private static int CalculateFreeThrowPercentage(IEnumerable<SessionZoneData> sessions)
    {
        var sessionZoneDatas = sessions.ToList();
        var attempts = sessionZoneDatas.Sum(s => s.FreeThrowAttempts);
        var makes = sessionZoneDatas.Sum(s => s.FreeThrowMakes);
        return Session.CalculateZoneShotPercentage(makes, attempts);
    }

    private static IReadOnlyList<FreeThrowTrendPoint> ComputeFreeThrowTrend(
        IEnumerable<SessionZoneData> sessions)
    {
        return
        [
            .. sessions
                .Where(s => s.FreeThrowAttempts > 0) // exclude sessions with no FT attempts
                .Select(s => new FreeThrowTrendPoint(
                    s.Date,
                    s.FreeThrowAttempts,
                    s.FreeThrowMakes,
                    Session.CalculateZoneShotPercentage(s.FreeThrowMakes, s.FreeThrowAttempts)))
                .OrderBy(p => p.Date)
        ];
    }
}

  