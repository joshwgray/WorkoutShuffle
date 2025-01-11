namespace WorkoutShuffle.Core.DTOs;

public class CreateWorkoutDto                                  
{                                                              
    public Guid Id { get; set; }                               
    public string CourseHeader { get; set; } = String.Empty;   
    public string Version { get; set; } = String.Empty;        
    public string Units { get; set; } = String.Empty;          
    public string Description { get; set; } = String.Empty;    
    public string FileName { get; set; } = String.Empty;       
    public string Ftp { get; set; } = String.Empty;            
    public string MinutesWatts { get; set; } = String.Empty;   
    public string EndCourseHeader { get; set; } = String.Empty;
    public List<WorkoutStep> WorkoutSteps { get; set; }        
    public string CourseData { get; internal set; }            
    public string EndCourseData { get; internal set; }         
}                                                              
                                                               
public class WorkoutStep                                       
{                                                              
    public string Time { get; set; }                           
    public string Power { get; set; }                          
}                                                              