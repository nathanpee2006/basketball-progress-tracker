using Backend.Data;
using Microsoft.EntityFrameworkCore;

namespace backend.Features.Analytics;

public record ConsistencyDto(
    int CurrentStreakWeeks,
    int LongestStreakWeeks,
    int TotalSessions,
    double AvgSessionsPerWeek,
    double AvgSessionsPerMonth
);

public interface IConsistencyService
{
    Task<ConsistencyDto> GetConsistencyAsync(int playerId, CancellationToken ct = default);
}

public class ConsistencyService : IConsistencyService
{
    private readonly AppDbContext _db;

    public ConsistencyService(AppDbContext db) => _db = db;

    public async Task<ConsistencyDto> GetConsistencyAsync(int playerId, CancellationToken ct = default)
    {
        var player = await _db.Players.AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == playerId, ct);

        var sessions = await _db.Sessions.AsNoTracking()
            .Where(s => s.PlayerId == playerId)
            .Select(s => new { s.Date, s.DateAssignedAt })
            .ToListAsync(ct);

        if (sessions.Count == 0)
            return new ConsistencyDto(0, 0, 0, 0, 0);

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

        return new ConsistencyDto(
            currentStreak,
            longestStreak,
            totalSessions,
            totalSessions / weeksSpan,
            totalSessions / monthsSpan);
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
        return Math.Max(1, months + fraction + 1);
    }

    private static TimeZoneInfo ResolveTimeZone(string tzId)
    {
        try { return TimeZoneInfo.FindSystemTimeZoneById(tzId); }
        catch (TimeZoneNotFoundException) { return TimeZoneInfo.Utc; }
    }
}