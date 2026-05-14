using Microsoft.AspNetCore.Components;
using MyPlanner.Models;
using MyPlanner.Services;

namespace MyPlanner.Components.Pages;

public partial class Home : IDisposable
{
    [Inject] private IDataService DataService { get; set; } = default!;
    [Inject] private ActivityStatsNotifier StatsNotifier { get; set; } = default!;
    [Inject] private LocalizationService Loc { get; set; } = default!;

    private enum ListFilter { All, Active, Done }

    private List<Activity> activities = new();
    private FormState newForm = null!;

    private string searchQuery = string.Empty;
    private ListFilter filter = ListFilter.All;
    private string filterStatusPick = "Any";
    private DateTime? filterDueFrom;
    private DateTime? filterDueTo;
    private bool filtersExpanded;

    private const int ListPageSize = 4;
    private int listPage = 1;

    private bool isLoading = true;
    private bool busy;
    private string? loadError;
    private string? actionError;
    private string? addFormError;
    private string? editError;

    private int? editingId;
    private FormState? editForm;

    private List<Activity> GetFilteredOrdered() =>
        activities
            .Where(MatchesScope)
            .Where(MatchesStatusPick)
            .Where(MatchesDeadlineRange)
            .Where(MatchesSearch)
            .OrderByDescending(a => a.Created)
            .ToList();

    private (IReadOnlyList<Activity> Items, int Total, int Page, int TotalPages) GetListPagingSnapshot()
    {
        var ordered = GetFilteredOrdered();
        var total = ordered.Count;
        if (total == 0)
            return (Array.Empty<Activity>(), 0, 1, 1);
        var totalPages = (total + ListPageSize - 1) / ListPageSize;
        var page = Math.Clamp(listPage, 1, totalPages);
        var items = ordered.Skip((page - 1) * ListPageSize).Take(ListPageSize).ToList();
        return (items, total, page, totalPages);
    }

    private void ClampListPage()
    {
        var ordered = GetFilteredOrdered();
        var total = ordered.Count;
        if (total == 0)
        {
            listPage = 1;
            return;
        }

        var totalPages = (total + ListPageSize - 1) / ListPageSize;
        listPage = Math.Clamp(listPage, 1, totalPages);
    }

    private void ResetListPaging() => listPage = 1;

    private void ListGoFirst() => listPage = 1;

    private void ListGoPrev()
    {
        var snap = GetListPagingSnapshot();
        if (snap.Total == 0) return;
        if (snap.Page > 1) listPage = snap.Page - 1;
    }

    private void ListGoNext()
    {
        var snap = GetListPagingSnapshot();
        if (snap.Total == 0) return;
        if (snap.Page < snap.TotalPages) listPage = snap.Page + 1;
    }

    private void ListGoLast()
    {
        var snap = GetListPagingSnapshot();
        if (snap.Total == 0) return;
        listPage = snap.TotalPages;
    }

    private bool MatchesScope(Activity a) => filter switch
    {
        ListFilter.Active => IsActiveBucket(a.Status),
        ListFilter.Done => a.Status == ActivityStatus.Completed,
        _ => true
    };

    private bool MatchesStatusPick(Activity a) =>
        filterStatusPick == "Any" || a.Status.ToString() == filterStatusPick;

    private bool MatchesDeadlineRange(Activity a)
    {
        if (filterDueFrom.HasValue && a.Deadline.Date < filterDueFrom.Value.Date)
            return false;
        if (filterDueTo.HasValue && a.Deadline.Date > filterDueTo.Value.Date)
            return false;
        return true;
    }

    private bool MatchesSearch(Activity a)
    {
        if (string.IsNullOrWhiteSpace(searchQuery)) return true;
        var q = searchQuery.Trim();
        return a.Name.Contains(q, StringComparison.OrdinalIgnoreCase)
               || (a.Description?.Contains(q, StringComparison.OrdinalIgnoreCase) ?? false)
               || (a.Tags?.Contains(q, StringComparison.OrdinalIgnoreCase) ?? false);
    }

    private int StatusCount(ActivityStatus s) => activities.Count(a => a.Status == s);

    private int DoneCount => StatusCount(ActivityStatus.Completed);

    private string TodayLabel => DateTime.Today.ToString("dddd, MMMM d", Loc.Culture);

    private int OverdueCount => activities.Count(IsOverdue);

    private IReadOnlyList<Activity> UpcomingSoon =>
        activities
            .Where(a => IsActiveBucket(a.Status))
            .OrderBy(a => a.Deadline)
            .Take(5)
            .ToList();

    private IReadOnlyList<Activity> OverdueList =>
        activities.Where(IsOverdue).OrderBy(a => a.Deadline).Take(5).ToList();

    private static string StatusBarSegClass(ActivityStatus s) => s switch
    {
        ActivityStatus.NotStarted => "status-bar__seg--open",
        ActivityStatus.InProgress => "status-bar__seg--progress",
        ActivityStatus.Completed => "status-bar__seg--done",
        ActivityStatus.OnHold => "status-bar__seg--hold",
        ActivityStatus.Cancelled => "status-bar__seg--cancelled",
        _ => "status-bar__seg--open"
    };

    private static bool IsActiveBucket(ActivityStatus s) =>
        s is ActivityStatus.NotStarted or ActivityStatus.InProgress or ActivityStatus.OnHold;

    private void SetFilter(ListFilter f)
    {
        filter = f;
        listPage = 1;
    }

    private void ClearFilters()
    {
        searchQuery = string.Empty;
        filterStatusPick = "Any";
        filterDueFrom = null;
        filterDueTo = null;
        filter = ListFilter.All;
        filtersExpanded = false;
        listPage = 1;
    }

    private void ToggleFiltersExpanded() => filtersExpanded = !filtersExpanded;

    private void CloseFiltersOverlay() => filtersExpanded = false;

    protected override async Task OnInitializedAsync()
    {
        Loc.LanguageChanged += OnLanguageChanged;
        newForm = FormState.Defaults(Loc.T("Form_DefaultNotes"));
        await LoadActivitiesAsync();
    }

    private void OnLanguageChanged() => _ = InvokeAsync(StateHasChanged);

    public void Dispose() => Loc.LanguageChanged -= OnLanguageChanged;

    protected override void OnAfterRender(bool firstRender)
    {
        base.OnAfterRender(firstRender);
        var snap = GetListPagingSnapshot();
        if (listPage != snap.Page)
        {
            listPage = snap.Page;
            StateHasChanged();
        }
    }

    private async Task LoadActivitiesAsync()
    {
        isLoading = true;
        loadError = null;
        actionError = null;
        try
        {
            activities = await DataService.GetDataAsync();
        }
        catch (Exception)
        {
            loadError = "err_load";
            activities = new List<Activity>();
        }
        finally
        {
            isLoading = false;
            ClampListPage();
            StatsNotifier.Notify();
        }
    }

    private void ResetNewForm()
    {
        addFormError = null;
        newForm = FormState.Defaults(Loc.T("Form_DefaultNotes"));
    }

    private async Task AddActivityAsync()
    {
        addFormError = null;
        actionError = null;
        var err = newForm.Validate();
        if (err != null)
        {
            addFormError = err;
            return;
        }

        var toSave = newForm.ToActivity(0);
        busy = true;
        try
        {
            var ok = await DataService.InsertActivityAsync(toSave);
            if (!ok) actionError = "err_save";
            else
            {
                ResetNewForm();
                await LoadActivitiesAsync();
            }
        }
        catch (Exception)
        {
            actionError = "err_save_io";
        }
        finally
        {
            busy = false;
        }
    }

    private async Task ToggleCompleteAsync(Activity activity, bool completed)
    {
        actionError = null;
        var updated = Clone(activity);
        updated.Status = completed ? ActivityStatus.Completed : ActivityStatus.InProgress;
        busy = true;
        try
        {
            var ok = await DataService.UpdateActivityAsync(activity.Id, updated);
            if (!ok) actionError = "err_update";
            else await LoadActivitiesAsync();
        }
        catch (Exception)
        {
            actionError = "err_update_generic";
        }
        finally
        {
            busy = false;
        }
    }

    private async Task DeleteActivityAsync(Activity activity)
    {
        actionError = null;
        busy = true;
        try
        {
            if (editingId == activity.Id) CancelEdit();
            var ok = await DataService.RemoveActivityAsync(activity.Id);
            if (!ok) actionError = "err_delete";
            else await LoadActivitiesAsync();
        }
        catch (Exception)
        {
            actionError = "err_delete_generic";
        }
        finally
        {
            busy = false;
        }
    }

    private async Task ClearCompletedAsync()
    {
        actionError = null;
        busy = true;
        try
        {
            foreach (var id in activities.Where(a => a.Status == ActivityStatus.Completed).Select(a => a.Id).ToList())
            {
                await DataService.RemoveActivityAsync(id);
            }
            CancelEdit();
            await LoadActivitiesAsync();
        }
        catch (Exception)
        {
            actionError = "err_clear";
        }
        finally
        {
            busy = false;
        }
    }

    private void StartEdit(Activity activity)
    {
        actionError = null;
        editError = null;
        editingId = activity.Id;
        editForm = FormState.FromActivity(activity);
    }

    private void CancelEdit()
    {
        editingId = null;
        editForm = null;
        editError = null;
    }

    private async Task SaveEditAsync()
    {
        if (editForm == null || editingId == null) return;
        editError = null;
        actionError = null;

        var err = editForm.Validate();
        if (err != null)
        {
            editError = err;
            return;
        }

        var updated = editForm.ToActivity(editingId.Value);
        busy = true;
        try
        {
            var ok = await DataService.UpdateActivityAsync(editingId.Value, updated);
            if (!ok) editError = "err_edit_save";
            else
            {
                CancelEdit();
                await LoadActivitiesAsync();
            }
        }
        catch (Exception)
        {
            editError = "err_edit_save";
        }
        finally
        {
            busy = false;
        }
    }

    private static Activity Clone(Activity a) => new()
    {
        Id = a.Id,
        Name = a.Name,
        Created = a.Created,
        Deadline = a.Deadline,
        Description = a.Description,
        Status = a.Status,
        Tags = a.Tags ?? string.Empty
    };

    private static bool IsOverdue(Activity a) =>
        a.Status != ActivityStatus.Completed
        && a.Status != ActivityStatus.Cancelled
        && a.Deadline.Date < DateTime.Today;

    private static string StatusPillClass(ActivityStatus s) => s switch
    {
        ActivityStatus.NotStarted => "tag--status-open",
        ActivityStatus.InProgress => "tag--status-progress",
        ActivityStatus.Completed => "tag--status-completed",
        ActivityStatus.OnHold => "tag--status-hold",
        ActivityStatus.Cancelled => "tag--status-cancelled",
        _ => "tag--status-open"
    };

    private static string Truncate(string value, int max)
    {
        if (string.IsNullOrEmpty(value) || value.Length <= max) return value;
        return value[..(max - 1)] + "…";
    }

    private string FormatRelative(DateTime created)
    {
        var local = created.Kind switch
        {
            DateTimeKind.Utc => created.ToLocalTime(),
            DateTimeKind.Local => created,
            _ => DateTime.SpecifyKind(created, DateTimeKind.Local)
        };
        var delta = DateTime.Now - local;
        if (delta.TotalSeconds < 45) return Loc.T("Rel_JustNow");
        if (delta.TotalMinutes < 60) return Loc.T("Rel_MinAgo", Math.Max(1, (int)delta.TotalMinutes));
        if (delta.TotalHours < 24) return Loc.T("Rel_HoursAgo", (int)delta.TotalHours);
        if (delta.TotalDays < 7) return Loc.T("Rel_DaysAgo", (int)delta.TotalDays);
        return created.ToLocalTime().ToString("MMM d, yyyy", Loc.Culture);
    }

    public sealed class FormState
    {
        public string Title { get; set; } = "";
        public string Notes { get; set; } = "";
        public DateTime Start { get; set; } = DateTime.Today;
        public DateTime Due { get; set; } = DateTime.Today.AddDays(7);
        public int Status { get; set; }
        public string Tags { get; set; } = "";

        public static FormState Defaults(string localizedDefaultNotes) => new()
        {
            Title = "",
            Notes = localizedDefaultNotes,
            Start = DateTime.Today,
            Due = DateTime.Today.AddDays(7),
            Status = 0,
            Tags = ""
        };

        public static FormState FromActivity(Activity a) => new()
        {
            Title = a.Name,
            Notes = a.Description,
            Start = a.Created.Date,
            Due = a.Deadline.Date,
            Status = (int)a.Status,
            Tags = a.Tags ?? string.Empty
        };

        public string? Validate()
        {
            var t = Title.Trim();
            if (t.Length < 3 || t.Length > 100) return "val_title";
            var n = Notes.Trim();
            if (n.Length < 10 || n.Length > 1000) return "val_notes";
            if (!Enum.IsDefined(typeof(ActivityStatus), Status)) return "val_status";
            var tags = Tags.Trim();
            if (tags.Length > 200) return "val_tags";
            return null;
        }

        public Activity ToActivity(int id)
        {
            var t = Title.Trim();
            var n = Notes.Trim();
            return new Activity
            {
                Id = id,
                Name = t,
                Description = n,
                Created = Start.Date,
                Deadline = Due.Date,
                Status = (ActivityStatus)Status,
                Tags = Tags.Trim()
            };
        }
    }
}
