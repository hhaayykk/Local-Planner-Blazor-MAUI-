namespace MyPlanner.Models;

public class DeletedActivityEntry
{
    public Activity Activity { get; set; } = null!;

    public DateTime DeletedAt { get; set; }

    public static Activity CloneActivity(Activity a) => new()
    {
        Id = a.Id,
        Name = a.Name,
        Created = a.Created,
        Deadline = a.Deadline,
        Description = a.Description,
        Status = a.Status,
        Tags = a.Tags ?? string.Empty
    };
}
