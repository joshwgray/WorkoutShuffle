using System.Globalization;
using MediatR;
using WorkoutShuffle.Core.DTOs;

namespace WorkoutShuffle.Core.Workouts.Commands;

public class CreateWorkouts : IRequest<List<CreateWorkoutDto>>
{
    /// <summary>
    /// The calculate time in zone metric for workouts
    /// </summary>
    public static double TimeInZone { get; private set; }

    /// <summary>
    /// The name of the workout.
    /// </summary>
    public string Name { get; set; } = String.Empty;

    /// <summary>
    /// The duration of the workout, in minutes.
    /// </summary>
    public int Duration { get; set; }

    /// <summary>
    /// The FTP value to use for the workout.
    /// </summary>
    public int Ftp { get; set; }

    /// <summary>
    /// The maximum time that the athlete should spend in each training zone.
    /// </summary>
    public int TimeInZoneLimit { get; set; }

    /// <summary>
    /// The amount of workouts to create.
    /// </summary>
    public int WorkoutCreateCount { get; set; }

    public WorkoutTypes WorkoutType { get; set; }

    public static IFormatProvider? Culture { get; set; } = CultureInfo.GetCultureInfo("en-US");

    public class Handler : IRequestHandler<CreateWorkouts, List<CreateWorkoutDto>>
    {
        private readonly ILogger<CreateWorkouts> _logger;

        public Handler(ILogger<CreateWorkouts> logger)
        {
            _logger = logger;
        }
        
        public Task<List<CreateWorkoutDto>> Handle(CreateWorkouts request, CancellationToken cancellationToken)
        {
            var workoutList = new List<CreateWorkoutDto>();
            for (int i = 0; i < request.WorkoutCreateCount; i++)
            {
                var r = new Random();
                const double startTime = 0.00;

                var workout = new CreateWorkoutDto
                {
                    Id = Guid.NewGuid(),
                    WorkoutSteps = new List<WorkoutStep>()
                };

                switch (request.WorkoutType)
                {
                    case WorkoutTypes.Endurance:
                        CreateWorkout(r, startTime, request, workout, i);
                        CommitWorkoutData(workout, workoutList);
                        break;
                    case WorkoutTypes.Threshold:
                        GenerateWorkoutRecursive(r, startTime, request, workout, workoutList, i);
                        break;
                    case WorkoutTypes.VO2:
                        GenerateWorkoutRecursive(r, startTime, request, workout, workoutList, i);
                        break;
                    default:
                        break;
                }
            }

            return Task.FromResult(workoutList);
        }

        private static Task<List<CreateWorkoutDto>> GenerateWorkoutRecursive(Random r, double startTime, CreateWorkouts request, CreateWorkoutDto workout, List<CreateWorkoutDto> workoutList, int workoutNumber)
        {
            TimeInZone = 0;
            // create the workout
            CreateWorkout(r, startTime, request, workout, workoutNumber);

            if (TimeInZone > request.TimeInZoneLimit)
            {
                return GenerateWorkoutRecursive(r, startTime, request, workout, workoutList, workoutNumber);
            }
            else
            {
                // if time in zone is within limit, commit the workout data and return the result
                var workoutDetail = CommitWorkoutData(workout, workoutList);
                return workoutDetail;
            }
        }

        private static Task<List<CreateWorkoutDto>> CommitWorkoutData(CreateWorkoutDto workout, List<CreateWorkoutDto> workoutList)
        {
            workoutList.Add(workout);
            return Task.FromResult(workoutList);
        }

        private static void CreateWorkout(Random r, double startTime, CreateWorkouts request, CreateWorkoutDto workout, int workoutNumber)
        {
            SetCourseHeaderDetails(request.Ftp, request.WorkoutType, workout, workoutNumber);
            SetCourseDataDetails(request.Ftp, startTime, r, request.WorkoutType, workout);
        }

        private static void SetCourseHeaderDetails(int ftp, WorkoutTypes? workoutType, CreateWorkoutDto workout, int workoutNumber)
        {
            workout.CourseHeader = "[COURSE HEADER]";
            workout.Version = "VERSION = 2";
            workout.Units = "UNITS = ENGLISH";
            workout.Description = $"DESCRIPTION = Random {workoutType} Workout";
            workout.FileName = $"FILE NAME = {workout.FileName}_{workoutType}_{workoutNumber}.erg";
            workout.Ftp = $"FTP = {ftp}";
            workout.MinutesWatts = "MINUTES WATTS";
            workout.EndCourseHeader = "[END COURSE HEADER]";
        }

        private static void SetCourseDataDetails(int ftp, double startTime, Random r, WorkoutTypes? workoutType, CreateWorkoutDto workout)
        {
            workout.CourseData = "[COURSE DATA]";

            double warmupEnd = AddWarmup(ftp, startTime, workoutType, workout);
            double repStart = AddRepeats(ftp, warmupEnd, r, workoutType, workout);
            AddCooldown(ftp, repStart, workout);

            workout.EndCourseData = "[END COURSE DATA]";
        }

        private static double AddWarmup(int ftp, double startTime, WorkoutTypes? workoutType, CreateWorkoutDto workout)
        {
            const double warmupEnd = 0;
            double warmupPercentFtp;
            double warmupDuration;

            switch (workoutType)
            {
                case WorkoutTypes.Endurance:
                    warmupDuration = 10.00;
                    warmupPercentFtp = 0.55;

                    return SetWarmupValues(ftp, startTime, warmupPercentFtp, warmupDuration, workout);
                case WorkoutTypes.Threshold:
                    warmupDuration = 20.00;
                    warmupPercentFtp = 0.5;

                    return SetWarmupValues(ftp, startTime, warmupPercentFtp, warmupDuration, workout);
                case WorkoutTypes.VO2:
                    warmupDuration = 20.00;
                    warmupPercentFtp = 0.5;

                    return SetWarmupValues(ftp, startTime, warmupPercentFtp, warmupDuration, workout);
            }

            return warmupEnd;
        }

        private static double SetWarmupValues(int ftp, double startTime, double warmupPercentFtp, double warmupDuration, CreateWorkoutDto workout)
        {
            double warmupEnd = SetEnd(startTime, warmupDuration);

            BuildWorkoutStepData(ftp, startTime, warmupPercentFtp, workout);
            BuildWorkoutStepData(ftp, warmupEnd, warmupPercentFtp, workout);

            return warmupEnd;
        }

        private static void BuildWorkoutStepData(int ftp, double time, double percentFtp, CreateWorkoutDto workout)
        {
            var workoutStep = new WorkoutStep
            {
                Time = $"{time.ToString("0.00", Culture)}",
                Power = Math.Round(ftp * percentFtp).ToString()
            };

            workout.WorkoutSteps.Add(workoutStep);
        }

        private static double AddRepeats(int ftp, double warmupEnd, Random r, WorkoutTypes? workoutType, CreateWorkoutDto workout)
        {
            var repStart = warmupEnd;

            if (workoutType == WorkoutTypes.Endurance)
            {
                var reps = r.Next(25, 50);

                for (int i = 0; i < reps; i++)
                {
                    var repDuration = Math.Round(GetRandomNumberInRange(r, 1, 5));
                    TimeInZone += repDuration;
                    var repPercentFtp = GetRandomNumberInRange(r, 0.56, 0.75);
                    var repEnd = SetEnd(repStart, repDuration);

                    BuildWorkoutStepData(ftp, repStart, repPercentFtp, workout);
                    BuildWorkoutStepData(ftp, repEnd, repPercentFtp, workout);

                    repStart = repEnd;
                }
            }
            else if (workoutType == WorkoutTypes.Threshold)
            {
                var reps = GetRandomNumberInRange(r, 2, 7);
                var restDuration = Math.Round(GetRandomNumberInRange(r, 5, 10));
                const double restPercentFtp = 0.5;
                const int numberOfParts = 6;

                for (int i = 0; i < reps; i++)
                {
                    var repDuration = Math.Round(GetRandomNumberInRange(r, 8, 15));
                    TimeInZone += repDuration;

                    var repSegments = DivideRandomly(repDuration, numberOfParts);
                    for (int j = 0; j < repSegments.Length; j++)
                    {
                        var repPercentFtp = GetRandomNumberInRange(r, 0.98, 1.05);
                        var repSegmentEnd = SetEnd(repStart, repSegments[j]);

                        BuildWorkoutStepData(ftp, repStart, repPercentFtp, workout);
                        BuildWorkoutStepData(ftp, repSegmentEnd, repPercentFtp, workout);

                        repStart = repSegmentEnd;
                    }

                    var restStart = repStart;
                    var restEnd = SetEnd(restStart, restDuration);

                    BuildWorkoutStepData(ftp, restStart, restPercentFtp, workout);
                    BuildWorkoutStepData(ftp, restEnd, restPercentFtp, workout);

                    repStart = restEnd;
                }
            }
            else if (workoutType == WorkoutTypes.VO2)
            {
                var reps = GetRandomNumberInRange(r, 4, 8);
                var restDuration = Math.Round(GetRandomNumberInRange(r, 5, 10));
                const double restPercentFtp = 0.5;
                const int numberOfParts = 8;

                for (int i = 0; i < reps; i++)
                {
                    var repDuration = Math.Round(GetRandomNumberInRange(r, 4, 8));
                    TimeInZone += repDuration;

                    var repSegments = DivideRandomly(repDuration, numberOfParts);
                    for (int j = 0; j < repSegments.Length; j++)
                    {
                        var repPercentFtp = GetRandomNumberInRange(r, 1.06, 1.2);
                        var repSegmentEnd = SetEnd(repStart, repSegments[j]);

                        BuildWorkoutStepData(ftp, repStart, repPercentFtp, workout);
                        BuildWorkoutStepData(ftp, repSegmentEnd, repPercentFtp, workout);

                        repStart = repSegmentEnd;
                    }

                    var restStart = repStart;
                    var restEnd = SetEnd(restStart, restDuration);

                    BuildWorkoutStepData(ftp, restStart, restPercentFtp, workout);
                    BuildWorkoutStepData(ftp, restEnd, restPercentFtp, workout);

                    repStart = restEnd;
                }
            }

            return repStart;
        }

        private static void AddCooldown(int ftp, double repStart, CreateWorkoutDto workout)
        {
            const double cooldownDuration = 10.00;
            const double cooldownPercentFtp = 0.5;

            var cooldownStart = repStart;
            var cooldownEnd = SetEnd(cooldownStart, cooldownDuration);

            BuildWorkoutStepData(ftp, cooldownStart, cooldownPercentFtp, workout);

            var workoutStep = new WorkoutStep
            {
                Time = $"{cooldownEnd.ToString("0.00", Culture)}",
                Power = Math.Round(ftp * cooldownPercentFtp).ToString()
            };

            workout.WorkoutSteps.Add(workoutStep);
        }

        private static double SetEnd(double startTime, double duration)
        {
            return (double)(startTime + duration);
        }

        /// <summary>
        /// Generates a random number within a specified range.
        /// </summary>
        /// <param name="random">The random number generator.</param>
        /// <param name="minNumber">The minimum number in the range.</param>
        /// <param name="maxNumber">The maximum number in the range.</param>
        /// <returns>A random number within the specified range.</returns>
        private static double GetRandomNumberInRange(Random random, double minNumber, double maxNumber)
        {
            // Generate a random number between 0 and 1 using the NextDouble method.
            // Multiply this by the difference between maxNumber and minNumber to get a random value in the range.
            // Finally, add the minNumber to the random value to shift it into the correct range.
            return (random.NextDouble() * (maxNumber - minNumber)) + minNumber;
        }

        /// <summary>
        /// Divides the specified value randomly into the specified number of parts.
        /// </summary>
        /// <param name="value">The value to be divided.</param>
        /// <param name="numberOfParts">The number of parts to divide the value into.</param>
        /// <returns>An array of doubles containing the parts of the divided value.</returns>
        private static double[] DivideRandomly(double value, int numberOfParts)
        {
            // Check that the number of parts is greater than or equal to 1.
            if (numberOfParts < 1)
            {
                throw new ArgumentException("The number of parts must be greater than or equal to 1.");
            }

            // Create a new array to hold the parts of the divided value.
            double[] parts = new double[numberOfParts];

            // Set the initial remaining value to the specified value.
            double remainingValue = value;

            // Divide the value into random parts, except for the last part.
            for (int i = 0; i < numberOfParts - 1; i++)
            {
                // Set the random part to exactly 50% of the remaining value.
                double randomPart = remainingValue * 0.5;
                parts[i] = randomPart;

                // Subtract the random part from the remaining value.
                remainingValue -= randomPart;
            }

            // Assign the remaining value to the last part.
            parts[numberOfParts - 1] = remainingValue;

            // Return the array of parts.
            return parts;
        }
    }
}