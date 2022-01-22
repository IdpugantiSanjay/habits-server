using Habits.Core;
using Habits.Core.Periods;
using Habits.Infrastructure;
using MediatR;
using MongoDB.Bson;
using Habit = Habits.Infrastructure.Habit;

namespace Habits.Application.Handlers;

public class CreateHabitHandler : IRequestHandler<CreateEveryDayHabit>, IRequestHandler<CreateEveryWeekHabit>,
    IRequestHandler<CreateEveryMonthHabit>, IRequestHandler<CreateEverySelectWeekDaysHabit>,
    IRequestHandler<CreateEveryXDaysHabit>
{
    private readonly HabitsService _service;

    public CreateHabitHandler(HabitsService service)
    {
        _service = service;
    }

    private Habit CreateHabit(CreateHabit habitRequest, Period period)
    {
        return new Habit(habitRequest.Name, period, habitRequest.StartDate);
    }

    public Task<Unit> Handle(CreateEveryDayHabit request, CancellationToken cancellationToken)
    {
        _service.AddHabit(request.Username, CreateHabit(request, new EveryDay()));
        return Unit.Task;
    }

    public Task<Unit> Handle(CreateEveryWeekHabit request, CancellationToken cancellationToken)
    {
        _service.AddHabit(request.Username, CreateHabit(request, new EveryWeek()));
        return Unit.Task;
    }

    public Task<Unit> Handle(CreateEveryMonthHabit request, CancellationToken cancellationToken)
    {
        _service.AddHabit(request.Username, CreateHabit(request, new EveryMonth()));
        return Unit.Task;
    }

    public Task<Unit> Handle(CreateEverySelectWeekDaysHabit request, CancellationToken cancellationToken)
    {
        _service.AddHabit(request.Username, CreateHabit(request, new EverySelectWeekDays(request.SelectDays)));
        return Unit.Task;
    }

    public Task<Unit> Handle(CreateEveryXDaysHabit request, CancellationToken cancellationToken)
    {
        _service.AddHabit(request.Username, CreateHabit(request, new EveryXDays(request.X)));
        return Unit.Task;
    }
}