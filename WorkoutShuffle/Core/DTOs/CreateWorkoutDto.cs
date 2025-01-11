namespace WorkoutShuffle.Core.DTOs;

public class CreateWorkoutDto                                  
{                                                              
    public Guid Id { get; set; }                               
    public string CourseHeader { get; set; } = string.Empty;   
    public string Version { get; set; } = string.Empty;        
    public string Units { get; set; } = string.Empty;          
    public string Description { get; set; } = string.Empty;    
    public string FileName { get; set; } = string.Empty;       
    public string Ftp { get; set; } = string.Empty;            
    public string MinutesWatts { get; set; } = string.Empty;   
    public string EndCourseHeader { get; set; } = string.Empty;
    public List<WorkoutStep> WorkoutSteps { get; set; }        
    public string? CourseData { get; internal set; }            
    public string? EndCourseData { get; internal set; }         
}                                                              
                                                               
public class WorkoutStep                                       
{                                                              
    public string? Time { get; set; }                           
    public string? Power { get; set; }                          
}                                                              