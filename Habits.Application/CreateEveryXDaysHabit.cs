using MediatR;

namespace Habits.Application;

public record CreateEveryXDaysHabit(string? Username, string Name, DateTime StartDate, int X) : CreateHabit(Username, Name, StartDate),
    IRequest;
