using System.Globalization;
using Microsoft.JSInterop;
using MyPlanner.Models;

namespace MyPlanner.Services;

public sealed class LocalizationService
{
    private readonly IJSRuntime _js;
    private UiLanguage _current = UiLanguage.En;

    public LocalizationService(IJSRuntime js) => _js = js;

    public UiLanguage Current => _current;

    public CultureInfo Culture => _current switch
    {
        UiLanguage.Ru => CultureInfo.GetCultureInfo("ru-RU"),
        UiLanguage.Hy => CultureInfo.GetCultureInfo("hy-AM"),
        _ => CultureInfo.GetCultureInfo("en-US")
    };

    public event Action? LanguageChanged;

    public string T(string key, params object[] args)
    {
        var template = UiStrings.Get(key, _current);
        if (args is { Length: > 0 })
            return string.Format(Culture, template, args);
        return template;
    }

    public string StatusLabel(ActivityStatus s) => T($"Status_{s}");

    public async Task LoadFromStorageAsync()
    {
        try
        {
            var code = await _js.InvokeAsync<string>("__myplanner.getLang");
            var lang = UiLanguageExtensions.FromStorageCode(code);
            if (lang != _current)
            {
                _current = lang;
                LanguageChanged?.Invoke();
            }

            await _js.InvokeVoidAsync("__myplanner.setHtmlLang", _current.ToStorageCode());
        }
        catch
        {
        }
    }

    public async Task SetLanguageAsync(UiLanguage lang)
    {
        _current = lang;
        try
        {
            await _js.InvokeVoidAsync("__myplanner.setLang", lang.ToStorageCode());
            await _js.InvokeVoidAsync("__myplanner.setHtmlLang", lang.ToStorageCode());
        }
        catch
        {
        }

        LanguageChanged?.Invoke();
    }
}
