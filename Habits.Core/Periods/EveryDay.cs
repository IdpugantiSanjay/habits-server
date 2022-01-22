namespace Habits.Core.Periods;

public class EveryDay : Period
{
    public override bool IsApplicableFor(DateTime dateOnly)
    {
        return true;
    }
}