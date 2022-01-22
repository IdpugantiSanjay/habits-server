using MediatR;

namespace Habits.Application;

public record CreateEverySelectWeekDaysHabit
    (string? Username, string Name, DateTime StartDate, DayOfWeek[] SelectDays) : CreateHabit(Username, Name, StartDate), IRequest;
