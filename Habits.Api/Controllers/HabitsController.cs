using Habits.Application;
using Habits.Application.Handlers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Habits.Api.Controllers
{
    [Route("api/users/{username}/[controller]")]
    [ApiController]
    public class HabitsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public HabitsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("everyday")]
        public async Task<IActionResult> CreateEveryDayHabit(string username, [FromBody] CreateEveryDayHabit habit,
            CancellationToken cancellationToken)
        {
            var request = habit with {Username = username};
            await _mediator.Send(request, cancellationToken);
            return NoContent();
        }

        [HttpPost("everyweek")]
        public async Task<IActionResult> CreateEveryWeekHabit(string username, CreateEveryWeekHabit habit,
            CancellationToken cancellationToken)
        {
            var request = habit with {Username = username};
            await _mediator.Send(request, cancellationToken);
            return NoContent();
        }

        [HttpPost("everymonth")]
        public async Task<IActionResult> CreateEveryMonthHabit(string username, CreateEveryMonthHabit habit,
            CancellationToken cancellationToken)
        {
            var request = habit with {Username = username};
            await _mediator.Send(request, cancellationToken);
            return NoContent();
        }

        [HttpPost("everyselectweekdayshabit")]
        public async Task<IActionResult> CreateEverySelectWeekDaysHabit(string username,
            CreateEverySelectWeekDaysHabit habit, CancellationToken cancellationToken)
        {
            var request = habit with {Username = username};
            await _mediator.Send(request, cancellationToken);
            return NoContent();
        }

        [HttpPost("everyxdayshabit")]
        public async Task<IActionResult> CreateXDaysHabit(string username, CreateEveryXDaysHabit habit,
            CancellationToken cancellationToken)
        {
            var request = habit with {Username = username};
            await _mediator.Send(request, cancellationToken);
            return NoContent();
        }

        [HttpGet]
        public async Task<IActionResult> FetchHabitsForDate(string username, [FromQuery] DateTime date)
        {
            var request = new FetchHabitsHandler.FetchHabits(username, date);
            return Ok(await _mediator.Send(request));
        }

        [HttpDelete("{habitId}")]
        public async Task<IActionResult> DeleteHabit(string username, string habitId)
        {
            var request = new DeleteHabit(username, habitId);
            await _mediator.Send(request);
            return NoContent();
        }
    }
}