using System.Globalization;

namespace backend.Features.Analytics;

public static class IsoWeekHelper
{
    public readonly record struct IsoWeekKey(int Year, int Week) : IComparable<IsoWeekKey>
    {
        public int CompareTo(IsoWeekKey other)
        {
            if (Year != other.Year) return Year.CompareTo(other.Year);
            return Week.CompareTo(other.Week);
        }

        private DateOnly ToMonday() =>
            DateOnly.FromDateTime(ISOWeek.ToDateTime(Year, Week, DayOfWeek.Monday));

        public IsoWeekKey PreviousWeek() => FromDate(ToMonday().AddDays(-7));
        public IsoWeekKey NextWeek() => FromDate(ToMonday().AddDays(7));

        public static IsoWeekKey FromDate(DateOnly date)
        {
            var dt = date.ToDateTime(TimeOnly.MinValue);
            return new IsoWeekKey(ISOWeek.GetYear(dt), ISOWeek.GetWeekOfYear(dt));
        }
    }

    public static DateOnly ToPlayerLocalDate(DateTime utcInstant, TimeZoneInfo tz)
    {
        var local = TimeZoneInfo.ConvertTimeFromUtc(
            DateTime.SpecifyKind(utcInstant, DateTimeKind.Utc), tz);
        return DateOnly.FromDateTime(local);
    }
}