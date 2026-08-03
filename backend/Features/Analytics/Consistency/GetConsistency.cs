using System.Security.Claims;
using Backend.Common.Endpoints;
using Backend.Common.Services;
using Backend.Data;
using Microsoft.EntityFrameworkCore;

namespace backend.Features.Analytics.Consistency;

public static class GetConsistency
{
    public record ConsistencyResponse(
        int CurrentStreakWeeks,
        int LongestStreakWeeks,
        int TotalSessions,
        double AvgSessionsPerWeek,
        double AvgSessionsPerMonth
    );

    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("analytics/consistency", Handler)
                .WithTags("Analytics")
                .Produces<ConsistencyResponse>()
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status404NotFound)
                .RequireAuthorization();
        }
    }

    public static async Task<IResult> Handler(
        AppDbContext context, IPlayerService playerService, 
        ClaimsPrincipal user, CancellationToken cancellationToken)
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

        var sessions = await context.Sessions
            .AsNoTracking()
            .Where(s => s.PlayerId == player.Id)
            .Select(s => new { s.Date, s.DateAssignedAt })
            .ToListAsync(cancellationToken);

        if (sessions.Count == 0)
            return TypedResults.Ok(new ConsistencyResponse(0, 0, 0, 0, 0));

        var tz = ResolveTimeZone(player.TimeZone);

        var liveWeeks = new HashSet<IsoWeekHelper.IsoWeekKey>();
        foreach (var s in sessions)
        {
            var assignedLocalDate = IsoWeekHelper.ToPlayerLocalDate(s.DateAssignedAt, tz);
            var dateWeek = IsoWeekHelper.IsoWeekKey.FromDate(s.Date);
            var assignedWeek = IsoWeekHelper.IsoWeekKey.FromDate(assignedLocalDate);

            if (dateWeek.Equals(assignedWeek))
                liveWeeks.Add(dateWeek);
        }

        var nowLocal = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz);
        var currentWeek = IsoWeekHelper.IsoWeekKey.FromDate(DateOnly.FromDateTime(nowLocal));

        int currentStreak = ComputeCurrentStreak(liveWeeks, currentWeek);
        int longestStreak = ComputeLongestStreak(liveWeeks);

        int totalSessions = sessions.Count;
        var firstDate = sessions.Min(s => s.Date);
        var today = DateOnly.FromDateTime(nowLocal);

        double weeksSpan = Math.Max(1,
            (today.ToDateTime(TimeOnly.MinValue) - firstDate.ToDateTime(TimeOnly.MinValue)).TotalDays / 7.0);
        double monthsSpan = Math.Max(1, MonthsBetween(firstDate, today));

        return TypedResults.Ok(new ConsistencyResponse(
            currentStreak,
            longestStreak,
            totalSessions,
            totalSessions / weeksSpan,
            totalSessions / monthsSpan));
    }

    private static int ComputeCurrentStreak(
        HashSet<IsoWeekHelper.IsoWeekKey> liveWeeks, IsoWeekHelper.IsoWeekKey currentWeek)
    {
        var startWeek = liveWeeks.Contains(currentWeek) ? currentWeek : currentWeek.PreviousWeek();
        if (!liveWeeks.Contains(startWeek)) return 0;

        int count = 0;
        var w = startWeek;
        while (liveWeeks.Contains(w))
        {
            count++;
            w = w.PreviousWeek();
        }

        return count;
    }

    private static int ComputeLongestStreak(HashSet<IsoWeekHelper.IsoWeekKey> liveWeeks)
    {
        int longest = 0;
        foreach (var week in liveWeeks)
        {
            if (liveWeeks.Contains(week.PreviousWeek())) continue;

            int runLength = 0;
            var w = week;
            while (liveWeeks.Contains(w))
            {
                runLength++;
                w = w.NextWeek();
            }

            longest = Math.Max(longest, runLength);
        }

        return longest;
    }

    private static double MonthsBetween(DateOnly from, DateOnly to)
    {
        int months = (to.Year - from.Year) * 12 + (to.Month - from.Month);
        double fraction = (to.Day - from.Day) / 30.0;
        return Math.Max(1, months + fraction);
    }

    private static TimeZoneInfo ResolveTimeZone(string tzId)
    {
        try
        {
            return TimeZoneInfo.FindSystemTimeZoneById(tzId);
        }
        catch (TimeZoneNotFoundException)
        {
            return TimeZoneInfo.Utc;
        }
        catch (InvalidTimeZoneException)
        {
            return TimeZoneInfo.Utc;
        }
    }
}