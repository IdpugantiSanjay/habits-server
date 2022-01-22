namespace Habits.Core.Periods;

public class EveryXDays : Period
{
    public int X { get; }

    public EveryXDays(int x)
    {
        X = x;
    }

    public override bool IsApplicableFor(DateTime dateOnly)
    {
        return true;
    }
}