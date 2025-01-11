namespace WorkoutShuffle.Core.Workouts.Commands;

/// <summary>
/// Represents the data required to create a new workout.
/// </summary>
public class CreateWorkoutRequest
{
    /// <summary>
    /// The type of workout to be created.
    /// </summary>
    public WorkoutTypes WorkoutType { get; set; }

    /// <summary>
    /// The name of the workout.
    /// </summary>
    public string Name { get; set; }

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
}

/// <summary>
/// The types of workouts that can be created
/// </summary>
public enum WorkoutTypes
{
    Endurance,
    Threshold,
    VO2
}