using Habits.Infrastructure;
using MediatR;

namespace Habits.Application.Handlers;

public class FetchHabitsHandler : IRequestHandler<FetchHabitsHandler.FetchHabits, FetchHabitsHandler.FetchHabitsResponse>
{
    private readonly HabitsService _service;

    public record HabitVm(string Id, string Name, dynamic Period, DateTime StartDate);


    public record FetchHabits(string Username, DateTime Date) : IRequest<FetchHabitsResponse>;

    public record FetchHabitsResponse(IEnumerable<HabitVm> Habits);

    public FetchHabitsHandler(HabitsService service)
    {
        _service = service;
    }

    public Task<FetchHabitsResponse> Handle(FetchHabits request, CancellationToken cancellationToken)
    {
        var result = _service.FetchUserHabitsForDate(request.Username, request.Date);
        return Task.FromResult(
            new FetchHabitsResponse(result.Select(r => new HabitVm(r.Id.ToString(), r.Name, r.Period, r.StartDate))));
    }
}