using MediatR;

namespace Habits.Application;

public record CreateEveryWeekHabit(string? Username, string Name, DateTime StartDate) : CreateHabit(Username, Name, StartDate), IRequest;
