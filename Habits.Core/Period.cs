namespace Habits.Core;

public abstract class Period
{
    public abstract bool IsApplicableFor(DateTime dateTime);
}