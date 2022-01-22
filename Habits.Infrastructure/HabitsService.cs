using Habits.Core;
using Habits.Core.Periods;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Habits.Infrastructure;

public class HabitsService
{
    private readonly IMongoCollection<UserHabits> _habits;
    private readonly IMongoCollection<HabitsCompleted> _completedHabits;

    public HabitsService(IMongoCollection<UserHabits> habits, IMongoCollection<HabitsCompleted> completedHabits)
    {
        _habits = habits;
        _completedHabits = completedHabits;
    }

    public IEnumerable<Habit> FetchUserHabitsForDate(string username, DateTime date)
    {
        var builder = Builders<UserHabits>.Filter;
        var filter = builder.Empty;

        filter &= builder.Eq(nameof(UserHabits.Username), username);

        return _habits.Find(filter).Project(us => us.Habits).ToList().SelectMany(h => h)
            .Where(h => date >= h.StartDate.Date && h.Period.IsApplicableFor(date));
    }

    public void AddHabit(string username, Habit habit)
    {
        _habits.FindOneAndUpdate(Builders<UserHabits>.Filter.Eq(nameof(UserHabits.Username), username),
            Builders<UserHabits>.Update.Push("Habits", habit),
            new FindOneAndUpdateOptions<UserHabits>() {IsUpsert = true});
    }

    public bool IsHabitCompletedForDate(string username, string habitId, DateTime date)
    {
        var builder = Builders<UserHabits>.Filter;
        var filter = builder.Empty;

        filter &= builder.Eq(nameof(UserHabits.Username), username);

        var userHabits = _habits.Find(filter).FirstOrDefault();

        if (userHabits is null) return false;

        var habit = userHabits.Habits.First(h => h.Id == ObjectId.Parse(habitId));

        if (habit.Period is EveryXTimesPerY xTimesPerY)
        {
            if (xTimesPerY.Y == Y.Month)
            {
                var (start, end) = date.MonthStartAndEndDates();
                var completedDates = FetchHabitDatesCompletedBetween(habitId, start, end).ToList();
                var completedOnDate = completedDates.Any(cd => cd.Date == date.Date);
                return completedOnDate || completedDates.Count >= xTimesPerY.X;
            }

            if (xTimesPerY.Y == Y.Week)
            {
                var (start, end) = date.WeekStartAndEndDates();
                var completedDates = FetchHabitDatesCompletedBetween(habitId, start, end).ToList();
                var completedOnDate = completedDates.Any(cd => cd.Date == date.Date);
                return completedOnDate || completedDates.Count >= xTimesPerY.X;
            }

            if (xTimesPerY.Y == Y.Year)
            {
                var (start, end) = date.YearStartAndEndDates();
                var completedDates = FetchHabitDatesCompletedBetween(habitId, start, end).ToList();
                var completedOnDate = completedDates.Any(cd => cd.Date == date.Date);
                return completedOnDate || completedDates.Count >= xTimesPerY.X;
            }

            throw new InvalidOperationException("EveryXTimesPerY -> Y Not Implemented");
        }

        var lastCompletedDate = FetchHabitLastCompletedDate(habitId);
        if (lastCompletedDate.HasValue is false)
        {
            return false;
        }

        return habit.Period switch
        {
            EveryDay => date == lastCompletedDate,
            EveryWeek => date.IsSameWeek(lastCompletedDate.Value),
            EveryMonth => date.IsSameMonth(lastCompletedDate.Value),
            EveryYear => date.Year == lastCompletedDate.Value.Year,
            EverySelectWeekDays => (date.Year, DateOnly.FromDateTime(date).DayNumber) ==
                                   (lastCompletedDate.Value.Year,
                                       DateOnly.FromDateTime(lastCompletedDate.Value).DayNumber),
            EveryXDays xDays => date.Subtract(lastCompletedDate.Value).TotalDays < xDays.X,
            _ => false
        };
    }

    private DateTime? FetchHabitLastCompletedDate(string habitId)
    {
        var lastCompleted = _completedHabits.Find(hc => hc.HabitId == ObjectId.Parse(habitId))
            .SortByDescending(hc => hc.CompletedDate)
            .Limit(1)
            .ToList();
        return lastCompleted.FirstOrDefault()?.CompletedDate.Date;
    }

    private IEnumerable<DateTime> FetchHabitDatesCompletedBetween(string habitId, DateTime from, DateTime to)
    {
        var completedInInterval = _completedHabits
            .Find(h => h.HabitId == ObjectId.Parse(habitId) && h.CompletedDate <= to && h.CompletedDate >= from)
            .ToList();
        return completedInInterval.Select(ci => ci.CompletedDate);
    }

    public void MarkCompleted(string habitId, DateTime date)
    {
        _completedHabits.InsertOne(new HabitsCompleted {CompletedDate = date, HabitId = ObjectId.Parse(habitId)});
    }

    public void RemoveHabit(string username, string habitId)
    {
        var habitObjectId = ObjectId.Parse(habitId);
        var filter = Builders<UserHabits>.Filter.Eq(nameof(UserHabits.Username), username);
        var update = Builders<UserHabits>.Update.PullFilter(us => us.Habits, h => h.Id == habitObjectId);
        _habits.UpdateOne(filter, update);
    }
}