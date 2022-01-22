using System.Globalization;

namespace Habits.Core;

public static class DateTimExtensions
{
    private static int Week(this DateTime date)
    {
        return CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(
            date, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
    }

    public static bool IsSameWeek(this DateTime thisDate, DateTime otherDate)
    {
        return (thisDate.Year, Week(thisDate)) == (otherDate.Year, Week(otherDate));
    }

    public static bool IsSameMonth(this DateTime thisDate, DateTime otherDate)
    {
        return (thisDate.Year, thisDate.Month) == (otherDate.Year, otherDate.Month);
    }

    public static (DateTime WeekStart, DateTime WeekEnd) WeekStartAndEndDates(this DateTime thisDateTime)
    {
        return (thisDateTime.AddDays(-(int)thisDateTime.DayOfWeek),
            thisDateTime.AddDays(6 - (int)thisDateTime.DayOfWeek));
    }

    public static (DateTime MonthStart, DateTime MonthEnd) MonthStartAndEndDates(this DateTime thisDateTime)
    {
        return (new DateTime(thisDateTime.Year, thisDateTime.Month, 1),
            new DateTime(thisDateTime.Year, thisDateTime.Month,
                DateTime.DaysInMonth(thisDateTime.Day, thisDateTime.Month)));
    }

    public static (DateTime MonthStart, DateTime MonthEnd) YearStartAndEndDates(this DateTime thisDateTime)
    {
        return (new DateTime(thisDateTime.Year, 1, 1), new DateTime(thisDateTime.Year, 12, 31));
    }
}