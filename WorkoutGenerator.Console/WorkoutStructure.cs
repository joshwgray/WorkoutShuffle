namespace WorkoutGenerator.Console
{
    public class Rootobject
    {
        public Structure[] Structure { get; set; }
    }

    public class Structure
    {
        public string IntensityClass { get; set; }
        public string Name { get; set; }
        public Length Length { get; set; }
        public string Type { get; set; }
        public Intensitytarget IntensityTarget { get; set; }
        public Step[] Steps { get; set; }
        public bool OpenDuration { get; set; }
    }

    public class Length
    {
        public string Unit { get; set; }
        public int Value { get; set; }
    }

    public class Intensitytarget
    {
        public string Unit { get; set; }
        public int Value { get; set; }
        public int MinValue { get; set; }
        public int MaxValue { get; set; }
    }

    public class Step
    {
        public string IntensityClass { get; set; }
        public string Name { get; set; }
        public Length1 Length { get; set; }
        public string Type { get; set; }
        public Intensitytarget1 IntensityTarget { get; set; }
        public Cadencetarget CadenceTarget { get; set; }
    }

    public class Length1
    {
        public string Unit { get; set; }
        public int Value { get; set; }
    }

    public class Intensitytarget1
    {
        public string Unit { get; set; }
        public int Value { get; set; }
    }

    public class Cadencetarget
    {
        public string Unit { get; set; }
        public int MinValue { get; set; }
        public int MaxValue { get; set; }
    }
}