namespace Habits.Core.Periods;

public class EveryYear : Period
{
    public override bool IsApplicableFor(DateTime dateOnly)
    {
        return true;
    }
}