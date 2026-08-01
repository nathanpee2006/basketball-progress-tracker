using Backend.Data;
using Backend.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace backend;

public static class AchievementSeeder
{
    private static readonly List<Achievement> Achievements =
    [
        // Volume — Paint
        new() { Key = "paint-1", Name = "Paint Starter", Description = "Make 50 shots in the paint", Trigger = "metric", Threshold = 50 },
        new() { Key = "paint-2", Name = "Paint Regular", Description = "Make 250 shots in the paint", Trigger = "metric", Threshold = 250 },
        new() { Key = "paint-3", Name = "Paint Beast", Description = "Make 1,000 shots in the paint", Trigger = "metric", Threshold = 1000 },

        // Volume — Midrange
        new() { Key = "midrange-1", Name = "Midrange Starter", Description = "Make 50 midrange shots", Trigger = "metric", Threshold = 50 },
        new() { Key = "midrange-2", Name = "Midrange Regular", Description = "Make 250 midrange shots", Trigger = "metric", Threshold = 250 },
        new() { Key = "midrange-3", Name = "Midrange Beast", Description = "Make 1,000 midrange shots", Trigger = "metric", Threshold = 1000 },

        // Volume — Three-point
        new() { Key = "three-1", Name = "Three Starter", Description = "Make 50 three-pointers", Trigger = "metric", Threshold = 50 },
        new() { Key = "three-2", Name = "Three Regular", Description = "Make 250 three-pointers", Trigger = "metric", Threshold = 250 },
        new() { Key = "three-3", Name = "Three Beast", Description = "Make 1,000 three-pointers", Trigger = "metric", Threshold = 1000 },

        // Volume — Free throw
        new() { Key = "ft-1", Name = "Free Throw Starter", Description = "Make 100 free throws", Trigger = "metric", Threshold = 100 },
        new() { Key = "ft-2", Name = "Free Throw Regular", Description = "Make 500 free throws", Trigger = "metric", Threshold = 500 },
        new() { Key = "ft-3", Name = "Free Throw Beast", Description = "Make 2,000 free throws", Trigger = "metric", Threshold = 2000 },

        // Volume — All-around
        new() { Key = "all-around-1", Name = "All-Around Baller", Description = "Make 100 total shots across all zones in a single session", Trigger = "metric", Threshold = 100 },

        // Session count
        new() { Key = "sessions-1", Name = "First Session", Description = "Log your first session", Trigger = "metric", Threshold = 1 },
        new() { Key = "sessions-10", Name = "10 Sessions", Description = "Log 10 total sessions", Trigger = "metric", Threshold = 10 },
        new() { Key = "sessions-50", Name = "50 Sessions", Description = "Log 50 total sessions", Trigger = "metric", Threshold = 50 },
        new() { Key = "sessions-100", Name = "100 Sessions", Description = "Log 100 total sessions", Trigger = "metric", Threshold = 100 },
    ];

    public static async Task SeedAsync(AppDbContext context, CancellationToken cancellationToken = default)
    {
        var existingKeys = await context.Achievements
            .Select(a => a.Key)
            .ToListAsync(cancellationToken);

        var toInsert = Achievements
            .Where(a => !existingKeys.Contains(a.Key))
            .ToList();

        if (toInsert.Count > 0)
        {
            context.Achievements.AddRange(toInsert);
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}