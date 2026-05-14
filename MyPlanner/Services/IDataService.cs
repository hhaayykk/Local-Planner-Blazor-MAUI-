using MyPlanner.Models;

namespace MyPlanner.Services;

public interface IDataService
{
    Task<List<Activity>> GetDataAsync();
    Task<bool> InsertActivityAsync(Activity activity);
    Task<bool> RemoveActivityAsync(int key);
    Task<bool> UpdateActivityAsync(int key, Activity newActivity);
    Task<IReadOnlyList<DeletedActivityEntry>> GetDeletedHistoryAsync();
    Task<bool> ClearDeletedHistoryAsync();
}
