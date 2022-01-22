namespace Habits.Core.Periods;

public class EveryMonth : Period
{
    public override bool IsApplicableFor(DateTime dateOnly)
    {
        return true;
    }
}