using Microsoft.JSInterop;

namespace MyPlanner.Services;

public sealed class ThemeService
{
    private readonly IJSRuntime _js;
    private UiTheme _current = UiTheme.Light;

    public ThemeService(IJSRuntime js) => _js = js;

    public UiTheme Current => _current;

    public bool IsDark => _current == UiTheme.Dark;

    public event Action? ThemeChanged;

    public async Task LoadFromStorageAsync()
    {
        try
        {
            var code = await _js.InvokeAsync<string>("__myplanner.getTheme");
            _current = code == "dark" ? UiTheme.Dark : UiTheme.Light;
        }
        catch
        {
            _current = UiTheme.Light;
        }

        ThemeChanged?.Invoke();
    }

    public async Task SetThemeAsync(UiTheme theme)
    {
        if (_current == theme) return;
        _current = theme;
        try
        {
            await _js.InvokeVoidAsync("__myplanner.setTheme", theme == UiTheme.Dark ? "dark" : "light");
        }
        catch
        {
        }

        ThemeChanged?.Invoke();
    }

    public Task ToggleAsync() =>
        SetThemeAsync(_current == UiTheme.Light ? UiTheme.Dark : UiTheme.Light);
}
