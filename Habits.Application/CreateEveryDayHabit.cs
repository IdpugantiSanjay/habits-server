using MediatR;

namespace Habits.Application;

public record CreateEveryDayHabit(string? Username, string Name, DateTime StartDate) : CreateHabit(Username, Name, StartDate), IRequest;
