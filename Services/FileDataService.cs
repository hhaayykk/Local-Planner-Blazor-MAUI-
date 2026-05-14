using Microsoft.Maui.Storage;
using MyPlanner.Models;
using System.Text.Json;

namespace MyPlanner.Services;

public class FileDataService : IDataService
{
    public static readonly string DefaultPath = "Data/items.txt";
    public static readonly string DefaultHistoryPath = "Data/deleted-history.json";

    private readonly string _filePath;
    private readonly string _historyPath;

    public FileDataService()
    {
        var root = FileSystem.AppDataDirectory;
        _filePath = Path.Combine(root, DefaultPath);
        _historyPath = Path.Combine(root, DefaultHistoryPath);
    }

    public async Task<List<Activity>> GetDataAsync() => await ReadCollectionFromFile();

    public async Task<bool> InsertActivityAsync(Activity activity)
    {
        var data = await ReadCollectionFromFile();

        int newId = data.Count == 0 ? 1 : data.Max(a => a.Id) + 1;
        activity.Id = newId;
        data.Add(activity);
        await WriteAllAsync(data);

        return true;
    }

    public async Task<bool> RemoveActivityAsync(int key)
    {
        var data = await ReadCollectionFromFile();

        var activity = data.FirstOrDefault(a => a.Id == key);
        if (activity == null)
            return false;

        var snapshot = DeletedActivityEntry.CloneActivity(activity);
        var history = await ReadDeletedHistoryAsync();
        history.Insert(0, new DeletedActivityEntry
        {
            Activity = snapshot,
            DeletedAt = DateTime.UtcNow
        });
        await WriteDeletedHistoryAsync(history);

        data.Remove(activity);
        await WriteAllAsync(data);
        return true;
    }

    public async Task<IReadOnlyList<DeletedActivityEntry>> GetDeletedHistoryAsync()
    {
        var list = await ReadDeletedHistoryAsync();
        return list;
    }

    public async Task<bool> ClearDeletedHistoryAsync()
    {
        try
        {
            await WriteDeletedHistoryAsync(new List<DeletedActivityEntry>());
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> UpdateActivityAsync(int key, Activity newActivity)
    {
        var data = await ReadCollectionFromFile();
        var activity = data.FirstOrDefault(a => a.Id == key);
        if (activity != null)
        {
            PopulateActivity(activity, newActivity);
            await WriteAllAsync(data);
            return true;
        }
        return false;
    }

    private void EnsureDataDirectory()
    {
        var dir = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrEmpty(dir))
            Directory.CreateDirectory(dir);
    }

    private async Task WriteAllAsync(List<Activity> data)
    {
        EnsureDataDirectory();
        await File.WriteAllTextAsync(_filePath, JsonSerializer.Serialize(data));
    }

    private void PopulateActivity(Activity activity, Activity newActivity)
    {
        activity.Name = newActivity.Name;
        activity.Description = newActivity.Description;
        activity.Created = newActivity.Created;
        activity.Deadline = newActivity.Deadline;
        activity.Status = newActivity.Status;
        activity.Tags = newActivity.Tags;
    }

    private async Task<List<Activity>> ReadCollectionFromFile()
    {
        if (!File.Exists(_filePath))
            return new List<Activity>();

        var json = await File.ReadAllTextAsync(_filePath);

        if (string.IsNullOrWhiteSpace(json))
            return new List<Activity>();

        return JsonSerializer.Deserialize<List<Activity>>(json)
               ?? new List<Activity>();
    }

    private async Task<List<DeletedActivityEntry>> ReadDeletedHistoryAsync()
    {
        if (!File.Exists(_historyPath))
            return new List<DeletedActivityEntry>();

        try
        {
            var json = await File.ReadAllTextAsync(_historyPath);
            if (string.IsNullOrWhiteSpace(json))
                return new List<DeletedActivityEntry>();

            return JsonSerializer.Deserialize<List<DeletedActivityEntry>>(json)
                   ?? new List<DeletedActivityEntry>();
        }
        catch (JsonException)
        {
            return new List<DeletedActivityEntry>();
        }
        catch (IOException)
        {
            return new List<DeletedActivityEntry>();
        }
    }

    private async Task WriteDeletedHistoryAsync(List<DeletedActivityEntry> data)
    {
        EnsureHistoryDirectory();
        await File.WriteAllTextAsync(_historyPath, JsonSerializer.Serialize(data));
    }

    private void EnsureHistoryDirectory()
    {
        var dir = Path.GetDirectoryName(_historyPath);
        if (!string.IsNullOrEmpty(dir))
            Directory.CreateDirectory(dir);
    }
}
