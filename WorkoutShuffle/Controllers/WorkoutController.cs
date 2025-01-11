using MediatR;
using Microsoft.AspNetCore.Mvc;
using WorkoutShuffle.Core.Workouts.Commands;
using WorkoutShuffle.Core.Workouts.Queries;

namespace WorkoutShuffle.Controllers;

[ApiController]
[Route("[controller]")]
public class WorkoutController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<WorkoutController> _logger;

    public WorkoutController(IMediator mediator, ILogger<WorkoutController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    [Route("")]
    public async Task<IActionResult> Get()
    {
        var result = await _mediator.Send(new GetWorkoutDetails());

        return Ok(result);
    }

    [HttpPost]
    [Route("CreateWorkout")]
    public async Task<IActionResult> CreateWorkout(CreateWorkoutRequest request)
    {
        var config = await _mediator.Send(new CreateWorkout()
        {
            WorkoutType = request.WorkoutType,
            Name = request.Name,
            Ftp = request.Ftp,
            TimeInZoneLimit = request.TimeInZoneLimit,
            WorkoutCreateCount = 1

        });

        return Ok(config);
    }

    [HttpPost]
    [Route("CreateWorkouts")]
    public async Task<IActionResult> CreateWorkouts(CreateWorkoutRequest request)
    {
        var config = await _mediator.Send(new CreateWorkouts()
        {
            WorkoutType = request.WorkoutType,
            Name = request.Name,
            Ftp = request.Ftp,
            TimeInZoneLimit = request.TimeInZoneLimit,
            WorkoutCreateCount = request.WorkoutCreateCount
        });

        return Ok(config);
    }
}