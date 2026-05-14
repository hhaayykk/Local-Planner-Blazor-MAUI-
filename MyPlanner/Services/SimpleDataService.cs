using MyPlanner.Models;

namespace MyPlanner.Services
{
    public class SimpleDataService : IDataService
    {
        private List<Activity> _activities = GetTestData();
        private readonly List<DeletedActivityEntry> _deletedHistory = new();

        public Task<List<Activity>> GetDataAsync()
        {
            return Task.FromResult(_activities);
        }

        public Task<bool> InsertActivityAsync(Activity activity)
        {
            int newId = _activities.Max(a => a.Id) + 1;
            activity.Id = newId;
            _activities.Add(activity);
            return Task.FromResult(true);
        }

        public Task<bool> RemoveActivityAsync(int key)
        {
            var activity = _activities.FirstOrDefault(a => a.Id == key);
            if (activity == null)
                return Task.FromResult(false);

            _deletedHistory.Insert(0, new DeletedActivityEntry
            {
                Activity = DeletedActivityEntry.CloneActivity(activity),
                DeletedAt = DateTime.UtcNow
            });
            _activities.Remove(activity);
            return Task.FromResult(true);
        }

        public Task<IReadOnlyList<DeletedActivityEntry>> GetDeletedHistoryAsync() =>
            Task.FromResult<IReadOnlyList<DeletedActivityEntry>>(_deletedHistory.ToList());

        public Task<bool> ClearDeletedHistoryAsync()
        {
            _deletedHistory.Clear();
            return Task.FromResult(true);
        }

        public Task<bool> UpdateActivityAsync(int key, Activity newActivity)
        {
            var activity = _activities.FirstOrDefault(a => a.Id == key);
            if (activity != null)
            {
                PopulateActivity(activity, newActivity);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        private void PopulateActivity(Activity activity, Activity newActivity)
        {
            activity.Name = newActivity.Name;
            activity.Description = newActivity.Description;
            activity.Created = newActivity.Created;
            activity.Deadline = newActivity.Deadline;
            activity.Status = newActivity.Status;
            activity.Tags = newActivity.Tags ?? string.Empty;
        }
        private static List<Activity> GetTestData()
        {
            var today = DateTime.Today;
            return new List<Activity>
            {
                new()
                {
                    Id = 1,
                    Name = "Morning Jog",
                    Description = "Jog for 30 minutes in the park every morning.",
                    Created = today,
                    Deadline = today.AddDays(1),
                    Status = ActivityStatus.InProgress,
                    Tags = ""
                },
                new()
                {
                    Id = 2,
                    Name = "Work on Project",
                    Description = "Complete the project report and submit it by end of week.",
                    Created = today,
                    Deadline = today.AddDays(5),
                    Status = ActivityStatus.NotStarted,
                    Tags = "work"
                },
                new()
                {
                    Id = 3,
                    Name = "Grocery Shopping",
                    Description = "Buy groceries for the week including produce and dairy.",
                    Created = today,
                    Deadline = today.AddDays(2),
                    Status = ActivityStatus.Completed,
                    Tags = ""
                }
            };
        }
    }
}
