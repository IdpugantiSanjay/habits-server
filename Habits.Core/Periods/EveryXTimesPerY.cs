namespace Habits.Core.Periods;


public enum Y
{
    Week,
    Month,
    Year
}

public class EveryXTimesPerY : Period
{
    public Y Y { get; }
    public int X { get; }

    public EveryXTimesPerY(int x, Y y)
    {
        Y = y;
        X = x;
    }

    public override bool IsApplicableFor(DateTime dateOnly)
    {
        return true;
    }
}