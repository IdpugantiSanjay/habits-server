namespace Habits.Core.Periods;

public class EverySelectWeekDays : Period
{
    public DayOfWeek[] SelectDays { get; }

    public EverySelectWeekDays(DayOfWeek[] selectDays)
    {
        SelectDays = selectDays;
    }

    public override bool IsApplicableFor(DateTime dateOnly)
    {
        return SelectDays.Contains(dateOnly.DayOfWeek);
    }
}