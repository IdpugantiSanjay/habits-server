using System;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using Habits.Core.Periods;
using MongoDB.Driver;
using Xunit;

// ReSharper disable once CheckNamespace
namespace Habits.Infrastructure.Tests;

public class HabitsServiceUnitTests : IDisposable
{
    private readonly HabitsService _service;
    private readonly IMongoCollection<HabitsCompleted> _completedHabits;
    private readonly IMongoCollection<UserHabits> _userHabits;
    private readonly string _username = new Fixture().Create<string>();

    public HabitsServiceUnitTests()
    {
        var client = new MongoClient("mongodb://127.0.0.1:27018");
        var db = client.GetDatabase("habits");

        _userHabits = db.GetCollection<UserHabits>("userHabits");
        _completedHabits = db.GetCollection<HabitsCompleted>("completedHabits");
        _service = new HabitsService(_userHabits, _completedHabits);
    }

    [Fact]
    public void ShouldReturnInListWhenAdded()
    {
        var everyDay = new EveryDay();
        _service.AddHabit(_username,
            new Habit("Wake Up at 9 AM", everyDay, DateTime.Now));
        Assert.Single(_service.FetchUserHabitsForDate(_username, DateTime.Now));
    }


    [Fact]
    public void ShouldNotReturnDeletedHabit()
    {
        var everyDay = new EveryDay();
        var wakeUpHabit = new Habit("Wake Up at 9 AM", everyDay, DateTime.UtcNow);
        var sleepHabit = new Habit("Sleep at 11 PM", everyDay, DateTime.UtcNow);

        _service.AddHabit(_username, wakeUpHabit);
        _service.AddHabit(_username, sleepHabit);
        _service.FetchUserHabitsForDate(_username, DateTime.UtcNow).Should()
            .BeEquivalentTo(new[] {wakeUpHabit, sleepHabit});
        _service.RemoveHabit(_username, sleepHabit.Id.ToString());
        _service.FetchUserHabitsForDate(_username, DateTime.UtcNow).Should()
            .BeEquivalentTo(new[] {wakeUpHabit});
        _service.RemoveHabit(_username, wakeUpHabit.Id.ToString());
        _service.FetchUserHabitsForDate(_username, DateTime.UtcNow).Should().BeEmpty();
    }

    [Fact]
    public void ShouldBeCompletedForNextXDays()
    {
        var period = new EveryXDays(3);
        var date = DateTime.UtcNow.Date;
        var habit = new Habit("Wake Up at 9 AM", period, date);
        var habitId = habit.Id.ToString();
        _service.AddHabit(_username, habit);
        Assert.False(_service.IsHabitCompletedForDate(_username, habitId, date));
        _service.MarkCompleted(habitId, date);

        foreach (var i in Enumerable.Range(0, 3))
            Assert.True(_service.IsHabitCompletedForDate(_username, habitId, date.AddDays(i)));
        Assert.False(_service.IsHabitCompletedForDate(_username, habitId, date.AddDays(3)));
    }

    [Fact]
    public void ShouldBeCompletedWhenEveryDayHabitMarkedAsCompleted()
    {
        var everyDay = new EveryDay();
        var date = DateTime.UtcNow.Date;
        var habit = new Habit("Wake Up at 9 AM", everyDay, date);
        var habitId = habit.Id.ToString();
        _service.AddHabit(_username, habit);
        Assert.False(_service.IsHabitCompletedForDate(_username, habitId, date));
        _service.MarkCompleted(habitId, date);
        Assert.True(_service.IsHabitCompletedForDate(_username, habitId, date));
    }

    [Fact]
    public void ShouldBeCompletedWhenEveryWeekHabitMarkedAsCompleted()
    {
        var everyDay = new EveryWeek();
        var date = new DateTime(2022, 1, 18);

        var habit = new Habit("Wake Up at 9 AM", everyDay, date);
        var habitId = habit.Id.ToString();
        _service.AddHabit(_username, habit);
        Assert.False(_service.IsHabitCompletedForDate(_username, habitId, date));
        _service.MarkCompleted(habitId, date);
        Assert.True(_service.IsHabitCompletedForDate(_username, habitId, date));

        var nextWeekDate = new DateTime(2022, 1, 25);
        Assert.False(_service.IsHabitCompletedForDate(_username, habitId, nextWeekDate));
    }

    [Fact]
    public void ShouldBeCompletedWhenEveryMonthHabitMarkedAsCompleted()
    {
        var everyDay = new EveryMonth();
        var date = new DateTime(2022, 1, 18);

        var habit = new Habit("Wake Up at 9 AM", everyDay, date);
        var habitId = habit.Id.ToString();
        _service.AddHabit(_username, habit);
        Assert.False(_service.IsHabitCompletedForDate(_username, habitId, date));
        _service.MarkCompleted(habitId, date);
        Assert.True(_service.IsHabitCompletedForDate(_username, habitId, date));

        var nextMonth = new DateTime(2022, 2, 1);
        Assert.False(_service.IsHabitCompletedForDate(_username, habitId, nextMonth));
    }

    [Fact]
    public void ShouldBeCompletedWhenEveryYearHabitMarkedAsCompleted()
    {
        var everyDay = new EveryYear();
        var date = new DateTime(2022, 1, 18);

        var habit = new Habit("Wake Up at 9 AM", everyDay, date);
        var habitId = habit.Id.ToString();
        _service.AddHabit(_username, habit);
        Assert.False(_service.IsHabitCompletedForDate(_username, habitId, date));
        _service.MarkCompleted(habitId, date);
        Assert.True(_service.IsHabitCompletedForDate(_username, habitId, date));

        var sameYear = new DateTime(2022, 12, 30);
        Assert.True(_service.IsHabitCompletedForDate(_username, habitId, sameYear));

        var nextYear = new DateTime(2023, 1, 1);
        Assert.False(_service.IsHabitCompletedForDate(_username, habitId, nextYear));
    }

    [Fact]
    public void ShouldBeCompletedWhenEverySelectDaysHabitMarkedAsCompleted()
    {
        var everyDay = new EverySelectWeekDays(new[] {DayOfWeek.Monday, DayOfWeek.Wednesday, DayOfWeek.Friday});
        var date = new DateTime(2022, 1, 17, 0, 0, 0, DateTimeKind.Utc); // monday

        var habit = new Habit("Wake Up at 9 AM", everyDay, date);
        var habitId = habit.Id.ToString();
        _service.AddHabit(_username, habit);
        Assert.False(_service.IsHabitCompletedForDate(_username, habitId, date));
        _service.MarkCompleted(habitId, date);
        Assert.True(_service.IsHabitCompletedForDate(_username, habitId, date));

        var nextSelectDay = new DateTime(2022, 1, 19); // wednesday
        Assert.False(_service.IsHabitCompletedForDate(_username, habitId, nextSelectDay));
    }

    [Fact]
    public void ShouldOnlyBeVisibleOnSelectDays()
    {
        var selectWeekDays = new[] {DayOfWeek.Monday, DayOfWeek.Wednesday, DayOfWeek.Friday};
        var period = new EverySelectWeekDays(selectWeekDays);
        var date = new DateTime(2022, 1, 17, 0, 0, 0, DateTimeKind.Utc); // monday

        var habit = new Habit("Wake Up at 9 AM", period, date);
        _service.AddHabit(_username, habit);
        var habits = _service.FetchUserHabitsForDate(_username, date);

        habits.Should().Equal(habit);

        var current = date;
        while (current.Year == date.Year)
        {
            if (selectWeekDays.Contains(current.DayOfWeek))
                _service.FetchUserHabitsForDate(_username, current).Should().Equal(habit);
            else
                _service.FetchUserHabitsForDate(_username, current).Should().BeEmpty();
            current = current.AddDays(1);
        }
    }

    [Fact]
    public void ShouldBeVisibleEveryDay()
    {
        var everyDay = new EveryDay();
        var startDate = new DateTime(2022, 1, 17, 0, 0, 0, DateTimeKind.Utc); // monday
        var habit = new Habit("Wake Up at 9 AM", everyDay, startDate);

        _service.AddHabit(_username, habit);

        var current = startDate;
        while (current.Year == startDate.Year)
        {
            _service.FetchUserHabitsForDate(_username, current).Should().Equal(habit);
            current = current.AddDays(1);
        }
    }

    [Fact]
    public void ShouldBeVisibleEveryWeek()
    {
        var period = new EveryWeek();

        var startDate = new DateTime(2022, 1, 17); // monday
        var habit = new Habit("Wake Up at 9 AM", period, startDate);

        _service.AddHabit(_username, habit);

        var current = startDate;
        while (current.Year == startDate.Year)
        {
            _service.FetchUserHabitsForDate(_username, current).Should().Equal(habit);
            current = current.AddDays(7);
        }
    }

    [Fact]
    public void ShouldBeVisibleEveryMonth()
    {
        var period = new EveryMonth();
        var startDate = new DateTime(2022, 1, 17); // monday
        var habit = new Habit("Wake Up at 9 AM", period, startDate);

        _service.AddHabit(_username, habit);

        var current = startDate;
        while (current.Year == startDate.Year)
        {
            _service.FetchUserHabitsForDate(_username, current).Should().Equal(habit);
            current = current.AddMonths(1);
        }
    }

    [Fact]
    public void ShouldBeVisibleEveryYear()
    {
        var period = new EveryMonth();
        var startDate = new DateTime(2022, 1, 17); // monday
        var habit = new Habit("Wake Up at 9 AM", period, startDate);

        _service.AddHabit(_username, habit);

        var current = startDate;
        while (current.Year <= startDate.Year + 10)
        {
            _service.FetchUserHabitsForDate(_username, current).Should().Equal(habit);
            current = current.AddYears(1);
        }
    }

    [Fact]
    public void ShouldBeVisibleEveryDayAndEverySelectWeekDays()
    {
        var everyDay = new EveryMonth();
        var selectWeekDays = new[] {DayOfWeek.Wednesday, DayOfWeek.Saturday};
        var everySelectWeekDays = new EverySelectWeekDays(selectWeekDays);

        var startDate = new DateTime(2022, 1, 17); // monday

        var habit1 = new Habit("Wake Up at 9 AM", everyDay, startDate);
        var habit2 = new Habit("Wake Up at 10 AM", everySelectWeekDays, startDate);

        _service.AddHabit(_username, habit1);
        _service.AddHabit(_username, habit2);

        var current = startDate;
        while (current.Year == startDate.Year)
        {
            if (selectWeekDays.Contains(current.DayOfWeek))
                _service.FetchUserHabitsForDate(_username, current).Should().Equal(habit1, habit2);
            else
                _service.FetchUserHabitsForDate(_username, current).Should().Equal(habit1);

            current = current.AddDays(1);
        }
    }

    [Fact]
    public void ShouldBeVisibleEveryDayAndEveryAllWeekDays()
    {
        var everyDay = new EveryMonth();
        var selectWeekDays = new[]
        {
            DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday,
            DayOfWeek.Saturday, DayOfWeek.Sunday
        };
        var everySelectWeekDays = new EverySelectWeekDays(selectWeekDays);

        var startDate = new DateTime(2022, 1, 17); // monday

        var habit1 = new Habit("Wake Up at 9 AM", everyDay, startDate);
        var habit2 = new Habit("Wake Up at 10 AM", everySelectWeekDays, startDate);

        _service.AddHabit(_username, habit1);
        _service.AddHabit(_username, habit2);

        var current = startDate;
        while (current.Year == startDate.Year)
        {
            _service.FetchUserHabitsForDate(_username, current).Should().Equal(habit1, habit2);
            current = current.AddDays(1);
        }
    }

    [Fact]
    public void ShouldBeCompletedAfterXTimesPerWeek()
    {
        var period = new EveryXTimesPerY(3, Y.Week);
        var startDate = new DateTime(2022, 1, 17); // monday
        var habit = new Habit("Wake Up at 9 AM", period, startDate);
        var habitId = habit.Id.ToString();

        _service.AddHabit(_username, habit);
        _service.FetchUserHabitsForDate(_username, startDate).Should().Equal(habit);

        foreach (var inc in Enumerable.Range(0, 3))
            _service.MarkCompleted(habitId, startDate.AddDays(inc));

        var afterXDays = new DateTime(2022, 1, 20); // thursday
        foreach (var inc in Enumerable.Range(0, 3))
            _service.IsHabitCompletedForDate(_username, habitId, afterXDays.AddDays(inc)).Should().BeTrue();
    }

    [Fact]
    public void ShouldBeCompletedAfterXTimesPerMonth()
    {
        var period = new EveryXTimesPerY(3, Y.Month);

        var startDate = new DateTime(2022, 1, 17, 0, 0, 0, DateTimeKind.Utc); // monday
        var habit = new Habit("Wake Up at 9 AM", period, startDate);
        var habitId = habit.Id.ToString();

        _service.AddHabit(_username, habit);

        _service.FetchUserHabitsForDate(_username, startDate).Should().Equal(habit);
        _service.IsHabitCompletedForDate(_username, habitId, startDate.AddDays(1)).Should().BeFalse();
        _service.MarkCompleted(habitId, startDate.AddDays(2));
        _service.IsHabitCompletedForDate(_username, habitId, startDate.AddDays(2)).Should().BeTrue();
        _service.IsHabitCompletedForDate(_username, habitId, startDate.AddDays(3)).Should().BeFalse();
        _service.IsHabitCompletedForDate(_username, habitId, startDate.AddDays(4)).Should().BeFalse();
        _service.MarkCompleted(habitId, startDate.AddDays(5));
        _service.IsHabitCompletedForDate(_username, habitId, startDate.AddDays(5)).Should().BeTrue();
        _service.IsHabitCompletedForDate(_username, habitId, startDate.AddDays(6)).Should().BeFalse();
        _service.MarkCompleted(habitId, startDate.AddDays(7));
        _service.IsHabitCompletedForDate(_username, habitId, startDate.AddDays(7)).Should().BeTrue();


        var afterXDays = new DateTime(2022, 1, 25); // thursday
        var current = afterXDays;
        while (current.Month == afterXDays.Month)
        {
            _service.IsHabitCompletedForDate(_username, habitId, current).Should().BeTrue();
            current = current.AddDays(1);
        }

        _service.IsHabitCompletedForDate(_username, habitId, startDate.AddMonths(1)).Should().BeFalse();
    }

    [Fact]
    public void ShouldBeCompletedAfterXTimesPerYear()
    {
        var period = new EveryXTimesPerY(4, Y.Year);
        var startDate = new DateTime(2022, 1, 17, 0, 0, 0, DateTimeKind.Utc); // monday
        var habit = new Habit("Wake Up at 9 AM", period, startDate);
        var habitId = habit.Id.ToString();

        _service.AddHabit(_username, habit);

        var completedDates = new[]
        {
            startDate.AddMonths(1), startDate.AddMonths(2),
            startDate.AddMonths(3), startDate.AddMonths(4),
        };

        var current = startDate;
        while (current <= startDate.AddMonths(4))
        {
            if (completedDates.Contains(current))
                _service.MarkCompleted(habitId, current);

            var isCompleted = _service.IsHabitCompletedForDate(_username, habitId, current);

            if (completedDates.Contains(current)) isCompleted.Should().BeTrue();
            else isCompleted.Should().BeFalse();

            current = current.AddDays(1);
        }

        current = startDate.AddMonths(4).AddDays(1);
        while (current.Year == startDate.Year)
        {
            _service.IsHabitCompletedForDate(_username, habitId, current).Should().BeTrue();
            current = current.AddDays(1);
        }
    }

    public void Dispose()
    {
        _userHabits.DeleteMany(Builders<UserHabits>.Filter.Empty);
        _completedHabits.DeleteMany(Builders<HabitsCompleted>.Filter.Empty);
    }
}