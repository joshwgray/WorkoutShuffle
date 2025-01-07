using System.Globalization;
using System.Text;

namespace WorkoutGenerator.Console;

internal class Program
{
    public static double timeInZone { get; private set; }

    private static void Main(string[] args)
    {
        var r = new Random();
        const int ftp = 350;
        const double startTime = 0.00;
        var culture = CultureInfo.GetCultureInfo("en-US");
        var builder = new StringBuilder();
        var ergFileCreatedList = new List<string>();
        
        // Relative path to the "Output" folder in the project
        string outputDir = Path.Combine(AppContext.BaseDirectory, "Workouts");
        
        // Ensure the directory exists
        Directory.CreateDirectory(outputDir);

        for (int i = 0; i < 20; i++)
        {
            timeInZone = 0;
            var workoutType = GetRandomEnumValue<WorkoutTypes>(r);

            string ergFilePath = Path.Combine(outputDir, @$"Random-{workoutType}-{i}.erg");

            if (workoutType == WorkoutTypes.Threshold)
            {
                CreateWorkout(r, ftp, startTime, culture, builder, workoutType);

                if (timeInZone > 60)
                {
                    builder.Clear();
                    // Recreate workout
                    CreateWorkout(r, ftp, startTime, culture, builder, workoutType);
                }
                else if (timeInZone <= 60)
                {
                    ergFilePath = Path.Combine(outputDir, @$"Random-{workoutType}-{i}-{timeInZone}min.erg");
                    CommitWorkout(builder, ergFilePath);
                    ergFileCreatedList.Add(ergFilePath);
                }
            }
            else if (workoutType == WorkoutTypes.VO2)
            {
                CreateWorkout(r, ftp, startTime, culture, builder, workoutType);

                if (timeInZone > 30)
                {
                    builder.Clear();
                    // Recreate workout
                    CreateWorkout(r, ftp, startTime, culture, builder, workoutType);
                }
                else if (timeInZone <= 30)
                {
                    ergFilePath = Path.Combine(outputDir, @$"Random-{workoutType}-{i}-{timeInZone}min.erg");
                    CommitWorkout(builder, ergFilePath);
                    ergFileCreatedList.Add(ergFilePath);
                }

            }
            else if (workoutType == WorkoutTypes.Endurance)
            {
                CreateWorkout(r, ftp, startTime, culture, builder, workoutType);
                ergFilePath = Path.Combine(outputDir, @$"Random-{workoutType}-{i}-{timeInZone}min.erg");
                CommitWorkout(builder, ergFilePath);

                ergFileCreatedList.Add(ergFilePath);
            }
        }

        PrintListOfCreatedFiles(ergFileCreatedList);
    }

    private static void PrintListOfCreatedFiles(List<string> ergFileCreatedList)
    {
        ergFileCreatedList.Sort();
        foreach (var ergFilePath in ergFileCreatedList)
        {
            System.Console.WriteLine(ergFilePath);
        }
    }

    private static void CreateWorkout(Random r, int ftp, double startTime, CultureInfo culture, StringBuilder builder, WorkoutTypes? workoutType)
    {
        SetCourseHeaderDetails(ftp, workoutType, builder);
        SetCourseDataDetails(ftp, startTime, r, workoutType, builder, culture);
    }

    private static void CommitWorkout(StringBuilder builder, string ergFilePath)
    {
        var writer = new StreamWriter(ergFilePath);
        writer.Write(builder.ToString());
        builder.Clear();
        writer.Flush();
    }

    private static void SetCourseHeaderDetails(int ftp, WorkoutTypes? workoutType, StringBuilder builder)
    {
        builder.AppendLine("[COURSE HEADER]");
        builder.AppendLine("VERSION = 2");
        builder.AppendLine("UNITS = ENGLISH");
        builder.AppendLine($"DESCRIPTION = Random {workoutType} Workout");
        builder.AppendLine($"FILE NAME = Random_{workoutType}.erg");
        builder.AppendLine($"FTP = {ftp}");
        builder.AppendLine("MINUTES WATTS");
        builder.AppendLine("[END COURSE HEADER]");
    }

    private static void SetCourseDataDetails(int ftp, double startTime, Random r, WorkoutTypes? workoutType, StringBuilder builder, CultureInfo culture)
    {
        builder.AppendLine("[COURSE DATA]");

        double warmupEnd = AddWarmup(ftp, startTime, workoutType, builder, culture);
        double repStart = AddRepeats(ftp, warmupEnd, r, workoutType, builder, culture);

        AddCooldown(ftp, repStart, builder, culture);

        builder.AppendLine("[END COURSE DATA]");
    }

    private static double AddWarmup(int ftp, double startTime, WorkoutTypes? workoutType, StringBuilder builder, CultureInfo culture)
    {
        const double warmupEnd = 0;
        double warmupPercentFtp;
        double warmupDuration;

        switch (workoutType)
        {
            case WorkoutTypes.Endurance:
                warmupDuration = 10.00;
                warmupPercentFtp = 0.55;

                return SetWarmupValues(ftp, startTime, warmupPercentFtp, warmupDuration, builder, culture);
            case WorkoutTypes.Threshold:
                warmupDuration = 20.00;
                warmupPercentFtp = 0.5;

                return SetWarmupValues(ftp, startTime, warmupPercentFtp, warmupDuration, builder, culture);
            case WorkoutTypes.VO2:
                warmupDuration = 20.00;
                warmupPercentFtp = 0.5;

                return SetWarmupValues(ftp, startTime, warmupPercentFtp, warmupDuration, builder, culture);
        }

        return warmupEnd;
    }

    private static double SetWarmupValues(int ftp, double startTime,  double warmupPercentFtp, double warmupDuration, StringBuilder builder, CultureInfo culture)
    {
        double warmupEnd = SetEnd(startTime, warmupDuration);
        builder.AppendLine($"{startTime.ToString("0.00", culture)}\t{Math.Round(ftp * warmupPercentFtp)}");
        builder.AppendLine($"{warmupEnd.ToString("0.00", culture)}\t{Math.Round(ftp * warmupPercentFtp)}");

        return warmupEnd;
    }

    private static double AddRepeats(int ftp, double warmupEnd, Random r, WorkoutTypes? workoutType, StringBuilder builder, CultureInfo culture)
    {
        var repStart = warmupEnd;

        if(workoutType == WorkoutTypes.Endurance)
        {
            var reps = r.Next(25, 50);

            for (int i = 0; i < reps; i++)
            {
                var repDuration = Math.Round(GetRandomNumberInRange(r, 1, 5));
                timeInZone += repDuration;
                var repPercentFtp = GetRandomNumberInRange(r, 0.56, 0.75);

                var repFtp = Math.Round(ftp * repPercentFtp);
                var repEnd = SetEnd(repStart, repDuration);

                builder.AppendLine($"{repStart.ToString("0.00", culture)}\t{repFtp}");
                builder.AppendLine($"{repEnd.ToString("0.00", culture)}\t{repFtp}");

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
                timeInZone += repDuration;

                var repSegments = DivideRandomly(repDuration, numberOfParts);
                for (int j = 0; j < repSegments.Length; j++)
                {
                    var repPercentFtp = GetRandomNumberInRange(r, 0.98, 1.05);
                    var repFtp = Math.Round(ftp * repPercentFtp);
                    var repSegmentEnd = SetEnd(repStart, repSegments[j]);

                    builder.AppendLine($"{repStart.ToString("0.00", culture)}\t{repFtp}");
                    builder.AppendLine($"{repSegmentEnd.ToString("0.00", culture)}\t{repFtp}");

                    repStart = repSegmentEnd;
                }

                var restStart = repStart;
                var restEnd = SetEnd(restStart, restDuration);

                builder.AppendLine($"{restStart.ToString("0.00", culture)}\t{ftp * restPercentFtp}");
                builder.AppendLine($"{restEnd.ToString("0.00", culture)}\t{ftp * restPercentFtp}");

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
                timeInZone += repDuration;

                var repSegments = DivideRandomly(repDuration, numberOfParts);
                for (int j = 0; j < repSegments.Length; j++)
                {
                    var repPercentFtp = GetRandomNumberInRange(r, 1.06, 1.2);
                    var repFtp = Math.Round(ftp * repPercentFtp);
                    var repSegmentEnd = SetEnd(repStart, repSegments[j]);

                    builder.AppendLine($"{repStart.ToString("0.00", culture)}\t{repFtp}");
                    builder.AppendLine($"{repSegmentEnd.ToString("0.00", culture)}\t{repFtp}");

                    repStart = repSegmentEnd;
                }

                var restStart = repStart;
                var restEnd = SetEnd(restStart, restDuration);

                builder.AppendLine($"{restStart.ToString("0.00", culture)}\t{ftp * restPercentFtp}");
                builder.AppendLine($"{restEnd.ToString("0.00", culture)}\t{ftp * restPercentFtp}");

                repStart = restEnd;
            }
        }

        return repStart;
    }

    private static void AddCooldown(int ftp, double repStart, StringBuilder builder, CultureInfo culture)
    {
        const double cooldownDuration = 10.00;
        const double cooldownPercentFtp = 0.5;

        var cooldownStart = repStart;
        var cooldownEnd = SetEnd(cooldownStart, cooldownDuration);
        builder.AppendLine($"{cooldownStart.ToString("0.00", culture)}\t{ftp * cooldownPercentFtp}");
        builder.AppendLine($"{cooldownEnd.ToString("0.00", culture)}\t{ftp * cooldownPercentFtp}");
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
    /// Gets a random value of the specified enum type.
    /// </summary>
    /// <typeparam name="T">The enum type.</typeparam>
    /// <param name="random">The random number generator.</param>
    /// <returns>A random enum value of the specified type, or null if the enum type has no values.</returns>
    private static T? GetRandomEnumValue<T>(Random random) where T : struct, Enum
    {
        // Get all the values of the specified enum type.
        var values = Enum.GetValues(typeof(T));

        // Check if the array is null or empty.
        if (values == null || values.Length == 0)
        {
            // If so, return null.
            return null;
        }

        // Get a random value from the array.
        var randomValue = values.GetValue(random.Next(values.Length));

        // Use the null-coalescing operator to check for null before unboxing the random value.
        return randomValue != null ? (T)randomValue : null;
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