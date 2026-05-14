using Microsoft.AspNetCore.Components;
using MyPlanner.Models;
using MyPlanner.Services;

namespace MyPlanner.Components.Layout;

public partial class MainLayout : IDisposable
{
    [Inject] private IDataService DataService { get; set; } = default!;
    [Inject] private ActivityStatsNotifier StatsNotifier { get; set; } = default!;
    [Inject] private LocalizationService Loc { get; set; } = default!;
    [Inject] private ThemeService Theme { get; set; } = default!;

    private List<Activity> navActivities = new();
    private bool _storageLoaded;

    protected override async Task OnInitializedAsync()
    {
        StatsNotifier.Changed += OnStatsChanged;
        Loc.LanguageChanged += OnLanguageChanged;
        Theme.ThemeChanged += OnThemeChanged;
        await ReloadNavActivitiesAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && !_storageLoaded)
        {
            _storageLoaded = true;
            await Loc.LoadFromStorageAsync();
            await Theme.LoadFromStorageAsync();
            await InvokeAsync(StateHasChanged);
        }
    }

    private void OnLanguageChanged() => InvokeAsync(StateHasChanged);

    private void OnThemeChanged() => InvokeAsync(StateHasChanged);

    private async Task ToggleThemeAsync()
    {
        await Theme.ToggleAsync();
        StateHasChanged();
    }

    private async Task SetLangAsync(UiLanguage lang)
    {
        if (Loc.Current == lang) return;
        await Loc.SetLanguageAsync(lang);
        StateHasChanged();
    }

    private void OnStatsChanged()
    {
        _ = RefreshFromNotifierAsync();
    }

    private async Task RefreshFromNotifierAsync()
    {
        await ReloadNavActivitiesAsync();
        await InvokeAsync(StateHasChanged);
    }

    private async Task ReloadNavActivitiesAsync()
    {
        try
        {
            navActivities = await DataService.GetDataAsync();
        }
        catch
        {
            navActivities = new List<Activity>();
        }
    }

    private int NavCount(ActivityStatus s) => navActivities.Count(a => a.Status == s);

    private static string NavStatClass(ActivityStatus s) => s switch
    {
        ActivityStatus.NotStarted => "nav-stat--not-started",
        ActivityStatus.InProgress => "nav-stat--in-progress",
        ActivityStatus.Completed => "nav-stat--completed",
        ActivityStatus.OnHold => "nav-stat--on-hold",
        ActivityStatus.Cancelled => "nav-stat--cancelled",
        _ => "nav-stat--not-started"
    };

    public void Dispose()
    {
        StatsNotifier.Changed -= OnStatsChanged;
        Loc.LanguageChanged -= OnLanguageChanged;
        Theme.ThemeChanged -= OnThemeChanged;
    }
}
