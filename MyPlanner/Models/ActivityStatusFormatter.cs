namespace MyPlanner.Models;

public static class ActivityStatusFormatter
{
    public static string Label(ActivityStatus s) => s switch
    {
        ActivityStatus.NotStarted => "Not started",
        ActivityStatus.InProgress => "In progress",
        ActivityStatus.Completed => "Completed",
        ActivityStatus.OnHold => "On hold",
        ActivityStatus.Cancelled => "Cancelled",
        _ => s.ToString()
    };
}
