using Microsoft.AspNetCore.Components;
using MyPlanner.Models;
using MyPlanner.Services;

namespace MyPlanner.Components.Pages;

public partial class History : IDisposable
{
    [Inject] private IDataService DataService { get; set; } = default!;
    [Inject] private LocalizationService Loc { get; set; } = default!;

    private IReadOnlyList<DeletedActivityEntry> entries = Array.Empty<DeletedActivityEntry>();
    private bool isLoading = true;
    private bool busy;
    private string? loadError;
    private string? actionError;
    private bool clearConfirm;

    protected override async Task OnInitializedAsync()
    {
        Loc.LanguageChanged += OnLanguageChanged;
        await LoadAsync();
    }

    private void OnLanguageChanged() => _ = InvokeAsync(StateHasChanged);

    public void Dispose() => Loc.LanguageChanged -= OnLanguageChanged;

    private async Task LoadAsync()
    {
        isLoading = true;
        loadError = null;
        actionError = null;
        clearConfirm = false;
        try
        {
            entries = await DataService.GetDeletedHistoryAsync();
        }
        catch (Exception)
        {
            loadError = "err_hist_load";
            entries = Array.Empty<DeletedActivityEntry>();
        }
        finally
        {
            isLoading = false;
        }
    }

    private void BeginClearConfirm()
    {
        actionError = null;
        clearConfirm = true;
    }

    private void CancelClearConfirm() => clearConfirm = false;

    private async Task ConfirmClearAsync()
    {
        actionError = null;
        busy = true;
        try
        {
            var ok = await DataService.ClearDeletedHistoryAsync();
            if (!ok) actionError = "err_hist_clear";
            else await LoadAsync();
        }
        catch (Exception)
        {
            actionError = "err_hist_clear";
        }
        finally
        {
            busy = false;
        }
    }

    private static string Truncate(string value, int max)
    {
        if (string.IsNullOrEmpty(value) || value.Length <= max) return value;
        return value[..(max - 1)] + "…";
    }

    private string FormatDeletedAt(DateTime utc) =>
        utc.ToLocalTime().ToString("yyyy-MM-dd HH:mm", Loc.Culture);

    private static string StatusClass(ActivityStatus s) => s switch
    {
        ActivityStatus.NotStarted => "hist-tag--open",
        ActivityStatus.InProgress => "hist-tag--progress",
        ActivityStatus.Completed => "hist-tag--done",
        ActivityStatus.OnHold => "hist-tag--hold",
        ActivityStatus.Cancelled => "hist-tag--cancelled",
        _ => "hist-tag--open"
    };
}
