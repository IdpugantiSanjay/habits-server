// using System;
// using System.Linq;
// using Habits.Core.Periods;
// using Xunit;
// using FluentAssertions;
//
// namespace Habits.Core.UnitTests;
//
// public class HabitServiceTests
// {
//     private readonly HabitService _service;
//
//     private const string Username = "sanjay";
//
//     public HabitServiceTests()
//     {
//         _service = new HabitService();
//     }
//
//     [Fact]
//     public void ShouldReturnInListWhenAdded()
//     {
//         var everyDay = new EveryDay();
//         _service.AddHabit(Username, new Habit(Guid.NewGuid(), "Wake Up at 9 AM", everyDay, DateTime.Now));
//         Assert.Single(_service.FetchUserHabitsForDate(Username, DateTime.Now));
//     }
//
//     [Fact]
//     public void ShouldBeCompletedForNextXDays()
//     {
//         var period = new EveryXDays(3);
//         var habitId = Guid.NewGuid();
//         var date = DateTime.Now.Date;
//         _service.AddHabit(Username, new Habit(habitId, "Wake Up at 9 AM", period, date));
//         Assert.False(_service.IsHabitCompletedForDate(habitId, date));
//         _service.MarkCompleted(habitId, date);
//
//         foreach (var i in Enumerable.Range(0, 3))
//             Assert.True(_service.IsHabitCompletedForDate(habitId, date.AddDays(i)));
//         Assert.False(_service.IsHabitCompletedForDate(habitId, date.AddDays(3)));
//     }
//
//     [Fact]
//     public void ShouldBeCompletedWhenEveryDayHabitMarkedAsCompleted()
//     {
//         var everyDay = new EveryDay();
//         var habitId = Guid.NewGuid();
//         var date = DateTime.Now.Date;
//         _service.AddHabit(Username, new Habit(habitId, "Wake Up at 9 AM", everyDay, date));
//         Assert.False(_service.IsHabitCompletedForDate(habitId, date));
//         _service.MarkCompleted(habitId, date);
//         Assert.True(_service.IsHabitCompletedForDate(habitId, date));
//     }
//
//     [Fact]
//     public void ShouldBeCompletedWhenEveryWeekHabitMarkedAsCompleted()
//     {
//         var everyDay = new EveryWeek();
//         var habitId = Guid.NewGuid();
//         var date = new DateTime(2022, 1, 18);
//
//         _service.AddHabit(Username, new Habit(habitId, "Wake Up at 9 AM", everyDay, date));
//         Assert.False(_service.IsHabitCompletedForDate(habitId, date));
//         _service.MarkCompleted(habitId, date);
//         Assert.True(_service.IsHabitCompletedForDate(habitId, date));
//
//         var nextWeekDate = new DateTime(2022, 1, 25);
//         Assert.False(_service.IsHabitCompletedForDate(habitId, nextWeekDate));
//     }
//
//     [Fact]
//     public void ShouldBeCompletedWhenEveryMonthHabitMarkedAsCompleted()
//     {
//         var everyDay = new EveryMonth();
//         var habitId = Guid.NewGuid();
//         var date = new DateTime(2022, 1, 18);
//
//         _service.AddHabit(Username, new Habit(habitId, "Wake Up at 9 AM", everyDay, date));
//         Assert.False(_service.IsHabitCompletedForDate(habitId, date));
//         _service.MarkCompleted(habitId, date);
//         Assert.True(_service.IsHabitCompletedForDate(habitId, date));
//
//         var nextMonth = new DateTime(2022, 2, 1);
//         Assert.False(_service.IsHabitCompletedForDate(habitId, nextMonth));
//     }
//
//     [Fact]
//     public void ShouldBeCompletedWhenEveryYearHabitMarkedAsCompleted()
//     {
//         var everyDay = new EveryYear();
//         var habitId = Guid.NewGuid();
//         var date = new DateTime(2022, 1, 18);
//
//         _service.AddHabit(Username, new Habit(habitId, "Wake Up at 9 AM", everyDay, date));
//         Assert.False(_service.IsHabitCompletedForDate(habitId, date));
//         _service.MarkCompleted(habitId, date);
//         Assert.True(_service.IsHabitCompletedForDate(habitId, date));
//
//         var sameYear = new DateTime(2022, 12, 31);
//         Assert.True(_service.IsHabitCompletedForDate(habitId, sameYear));
//
//         var nextYear = new DateTime(2023, 1, 1);
//         Assert.False(_service.IsHabitCompletedForDate(habitId, nextYear));
//     }
//
//     [Fact]
//     public void ShouldBeCompletedWhenEverySelectDaysHabitMarkedAsCompleted()
//     {
//         var everyDay = new EverySelectWeekDays(new[] {DayOfWeek.Monday, DayOfWeek.Wednesday, DayOfWeek.Friday});
//         var habitId = Guid.NewGuid();
//         var date = new DateTime(2022, 1, 17); // monday
//
//         _service.AddHabit(Username, new Habit(habitId, "Wake Up at 9 AM", everyDay, date));
//         Assert.False(_service.IsHabitCompletedForDate(habitId, date));
//         _service.MarkCompleted(habitId, date);
//         Assert.True(_service.IsHabitCompletedForDate(habitId, date));
//
//         var nextSelectDay = new DateTime(2022, 1, 19); // wednesday
//         Assert.False(_service.IsHabitCompletedForDate(habitId, nextSelectDay));
//     }
//
//     [Fact]
//     public void ShouldOnlyBeVisibleOnSelectDays()
//     {
//         var selectWeekDays = new[] {DayOfWeek.Monday, DayOfWeek.Wednesday, DayOfWeek.Friday};
//         var period = new EverySelectWeekDays(selectWeekDays);
//         var habitId = Guid.NewGuid();
//         var date = new DateTime(2022, 1, 17); // monday
//
//         var habit = new Habit(habitId, "Wake Up at 9 AM", period, date);
//         _service.AddHabit(Username, habit);
//         var habits = _service.FetchUserHabitsForDate(Username, date);
//         habits.Should().Equal(habit);
//
//         var current = date;
//         while (current.Year == date.Year)
//         {
//             if (selectWeekDays.Contains(current.DayOfWeek))
//                 _service.FetchUserHabitsForDate(Username, current).Should().Equal(habit);
//             else
//                 _service.FetchUserHabitsForDate(Username, current).Should().BeEmpty();
//             current = current.AddDays(1);
//         }
//     }
//
//     [Fact]
//     public void ShouldBeVisibleEveryDay()
//     {
//         var everyDay = new EveryDay();
//         var habitId = Guid.NewGuid();
//         var startDate = new DateTime(2022, 1, 17); // monday
//         var habit = new Habit(habitId, "Wake Up at 9 AM", everyDay, startDate);
//
//         _service.AddHabit(Username, habit);
//
//         var current = startDate;
//         while (current.Year == startDate.Year)
//         {
//             _service.FetchUserHabitsForDate(Username, current).Should().Equal(habit);
//             current = current.AddDays(1);
//         }
//     }
//
//     [Fact]
//     public void ShouldBeVisibleEveryWeek()
//     {
//         var period = new EveryWeek();
//         var habitId = Guid.NewGuid();
//         var startDate = new DateTime(2022, 1, 17); // monday
//         var habit = new Habit(habitId, "Wake Up at 9 AM", period, startDate);
//
//         _service.AddHabit(Username, habit);
//
//         var current = startDate;
//         while (current.Year == startDate.Year)
//         {
//             _service.FetchUserHabitsForDate(Username, current).Should().Equal(habit);
//             current = current.AddDays(7);
//         }
//     }
//
//     [Fact]
//     public void ShouldBeVisibleEveryMonth()
//     {
//         var period = new EveryMonth();
//         var habitId = Guid.NewGuid();
//         var startDate = new DateTime(2022, 1, 17); // monday
//         var habit = new Habit(habitId, "Wake Up at 9 AM", period, startDate);
//
//         _service.AddHabit(Username, habit);
//
//         var current = startDate;
//         while (current.Year == startDate.Year)
//         {
//             _service.FetchUserHabitsForDate(Username, current).Should().Equal(habit);
//             current = current.AddMonths(1);
//         }
//     }
//
//     [Fact]
//     public void ShouldBeVisibleEveryYear()
//     {
//         var period = new EveryMonth();
//         var habitId = Guid.NewGuid();
//         var startDate = new DateTime(2022, 1, 17); // monday
//         var habit = new Habit(habitId, "Wake Up at 9 AM", period, startDate);
//
//         _service.AddHabit(Username, habit);
//
//         var current = startDate;
//         while (current.Year <= startDate.Year + 10)
//         {
//             _service.FetchUserHabitsForDate(Username, current).Should().Equal(habit);
//             current = current.AddYears(1);
//         }
//     }
//
//     [Fact]
//     public void ShouldBeVisibleEveryDayAndEverySelectWeekDays()
//     {
//         var everyDay = new EveryMonth();
//         var selectWeekDays = new[] {DayOfWeek.Wednesday, DayOfWeek.Saturday};
//         var everySelectWeekDays = new EverySelectWeekDays(selectWeekDays);
//
//         var habitId = Guid.NewGuid();
//         var startDate = new DateTime(2022, 1, 17); // monday
//
//         var habit1 = new Habit(habitId, "Wake Up at 9 AM", everyDay, startDate);
//         var habit2 = new Habit(habitId, "Wake Up at 10 AM", everySelectWeekDays, startDate);
//
//         _service.AddHabit(Username, habit1);
//         _service.AddHabit(Username, habit2);
//
//         var current = startDate;
//         while (current.Year == startDate.Year)
//         {
//             if (selectWeekDays.Contains(current.DayOfWeek))
//                 _service.FetchUserHabitsForDate(Username, current).Should().Equal(habit1, habit2);
//             else
//                 _service.FetchUserHabitsForDate(Username, current).Should().Equal(habit1);
//
//             current = current.AddDays(1);
//         }
//     }
//
//     [Fact]
//     public void ShouldBeVisibleEveryDayAndEveryAllWeekDays()
//     {
//         var everyDay = new EveryMonth();
//         var selectWeekDays = new[]
//         {
//             DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday,
//             DayOfWeek.Saturday, DayOfWeek.Sunday
//         };
//         var everySelectWeekDays = new EverySelectWeekDays(selectWeekDays);
//
//         var habitId = Guid.NewGuid();
//         var startDate = new DateTime(2022, 1, 17); // monday
//
//         var habit1 = new Habit(habitId, "Wake Up at 9 AM", everyDay, startDate);
//         var habit2 = new Habit(habitId, "Wake Up at 10 AM", everySelectWeekDays, startDate);
//
//         _service.AddHabit(Username, habit1);
//         _service.AddHabit(Username, habit2);
//
//         var current = startDate;
//         while (current.Year == startDate.Year)
//         {
//             _service.FetchUserHabitsForDate(Username, current).Should().Equal(habit1, habit2);
//             current = current.AddDays(1);
//         }
//     }
//
//     [Fact]
//     public void ShouldBeCompletedAfterXTimesPerWeek()
//     {
//         var period = new EveryXTimesPerY(3, Y.Week);
//         var habitId = Guid.NewGuid();
//         var startDate = new DateTime(2022, 1, 17); // monday
//         var habit = new Habit(habitId, "Wake Up at 9 AM", period, startDate);
//
//         _service.AddHabit(Username, habit);
//         _service.FetchUserHabitsForDate(Username, startDate).Should().Equal(habit);
//
//         foreach (var inc in Enumerable.Range(0, 3))
//             _service.MarkCompleted(habitId, startDate.AddDays(inc));
//
//         var afterXDays = new DateTime(2022, 1, 20); // thursday
//         foreach (var inc in Enumerable.Range(0, 3))
//             _service.IsHabitCompletedForDate(habitId, afterXDays.AddDays(inc)).Should().BeTrue();
//     }
//
//     [Fact]
//     public void ShouldBeCompletedAfterXTimesPerMonth()
//     {
//         var period = new EveryXTimesPerY(3, Y.Month);
//         var habitId = Guid.NewGuid();
//         var startDate = new DateTime(2022, 1, 17); // monday
//         var habit = new Habit(habitId, "Wake Up at 9 AM", period, startDate);
//
//         _service.AddHabit(Username, habit);
//
//         _service.FetchUserHabitsForDate(Username, startDate).Should().Equal(habit);
//         _service.IsHabitCompletedForDate(habitId, startDate.AddDays(1)).Should().BeFalse();
//         _service.MarkCompleted(habitId, startDate.AddDays(2));
//         _service.IsHabitCompletedForDate(habitId, startDate.AddDays(2)).Should().BeTrue();
//         _service.IsHabitCompletedForDate(habitId, startDate.AddDays(3)).Should().BeFalse();
//         _service.IsHabitCompletedForDate(habitId, startDate.AddDays(4)).Should().BeFalse();
//         _service.MarkCompleted(habitId, startDate.AddDays(5));
//         _service.IsHabitCompletedForDate(habitId, startDate.AddDays(5)).Should().BeTrue();
//         _service.IsHabitCompletedForDate(habitId, startDate.AddDays(6)).Should().BeFalse();
//         _service.MarkCompleted(habitId, startDate.AddDays(7));
//         _service.IsHabitCompletedForDate(habitId, startDate.AddDays(7)).Should().BeTrue();
//
//
//         var afterXDays = new DateTime(2022, 1, 25); // thursday
//         var current = afterXDays;
//         while (current.Month == afterXDays.Month)
//         {
//             _service.IsHabitCompletedForDate(habitId, current).Should().BeTrue();
//             current = current.AddDays(1);
//         }
//
//         _service.IsHabitCompletedForDate(habitId, startDate.AddMonths(1)).Should().BeFalse();
//     }
//
//     [Fact]
//     public void ShouldBeCompletedAfterXTimesPerYear()
//     {
//         var period = new EveryXTimesPerY(4, Y.Year);
//         var habitId = Guid.NewGuid();
//         var startDate = new DateTime(2022, 1, 17); // monday
//         var habit = new Habit(habitId, "Wake Up at 9 AM", period, startDate);
//
//         _service.AddHabit(Username, habit);
//
//         var completedDates = new[]
//         {
//             startDate.AddMonths(1), startDate.AddMonths(2),
//             startDate.AddMonths(3), startDate.AddMonths(4),
//         };
//
//         var current = startDate;
//         while (current <= startDate.AddMonths(4))
//         {
//             if (completedDates.Contains(current))
//                 _service.MarkCompleted(habitId, current);
//
//             var isCompleted = _service.IsHabitCompletedForDate(habitId, current);
//
//             if (completedDates.Contains(current)) isCompleted.Should().BeTrue();
//             else isCompleted.Should().BeFalse();
//
//             current = current.AddDays(1);
//         }
//
//         current = startDate.AddMonths(4).AddDays(1);
//         while (current.Year == startDate.Year)
//         {
//             _service.IsHabitCompletedForDate(habitId, current).Should().BeTrue();
//             current = current.AddDays(1);
//         }
//     }
// }