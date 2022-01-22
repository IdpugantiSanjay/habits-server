using MediatR;

namespace Habits.Application;

public record CreateEveryMonthHabit(string? Username, string Name, DateTime StartDate) : CreateHabit(Username, Name, StartDate), IRequest;
