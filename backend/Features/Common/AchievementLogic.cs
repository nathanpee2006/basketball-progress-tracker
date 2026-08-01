namespace backend.Features.Common;

internal static class AchievementLogic
{
    public record SessionTotals(
        int PaintMakes,
        int MidrangeMakes,
        int ThreePointMakes,
        int FreeThrowMakes,
        int SessionCount
    );

    public static int GetCurrentValue(string key, SessionTotals totals, int bestSingleSessionTotal) => key switch
    {
        "paint-1" or "paint-2" or "paint-3" => totals.PaintMakes,
        "midrange-1" or "midrange-2" or "midrange-3" => totals.MidrangeMakes,
        "three-1" or "three-2" or "three-3" => totals.ThreePointMakes,
        "ft-1" or "ft-2" or "ft-3" => totals.FreeThrowMakes,
        "all-around-1" => bestSingleSessionTotal,
        "sessions-1" or "sessions-10" or "sessions-50" or "sessions-100" => totals.SessionCount,
        _ => 0
    };
}