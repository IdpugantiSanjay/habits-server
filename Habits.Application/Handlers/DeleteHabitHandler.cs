using Habits.Infrastructure;
using MediatR;

namespace Habits.Application.Handlers;

public record DeleteHabit(string Username, string HabitId): IRequest;

public class DeleteHabitHandler: IRequestHandler<DeleteHabit>
{
    private readonly HabitsService _service;

    public DeleteHabitHandler(HabitsService service)
    {
        _service = service;
    }
    
    public Task<Unit> Handle(DeleteHabit request, CancellationToken cancellationToken)
    {
        _service.RemoveHabit(request.Username, request.HabitId);
        return Unit.Task;
    }
}