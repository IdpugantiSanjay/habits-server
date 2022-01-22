namespace Habits.Core.Periods;

public class EveryWeek : Period
{
    public override bool IsApplicableFor(DateTime dateOnly)
    {
        return true;
    }
}