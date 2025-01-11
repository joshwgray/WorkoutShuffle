using MediatR;
using WorkoutShuffle.Core.DTOs;

namespace WorkoutShuffle.Core.Workouts.Queries;

public class GetWorkoutDetails : IRequest<WorkoutDetailDto>
{
    public class Handler : IRequestHandler<GetWorkoutDetails, WorkoutDetailDto>
    {
        private readonly ILogger<GetWorkoutDetails> _logger;
        public Handler(ILogger<GetWorkoutDetails> logger)
        {
            _logger = logger;
        }

        public Task<WorkoutDetailDto> Handle(GetWorkoutDetails request, CancellationToken cancellationToken)
        {

            throw new NotImplementedException();
        }
    }
}