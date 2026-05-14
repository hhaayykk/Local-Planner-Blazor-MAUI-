namespace MyPlanner.Services;

public enum UiLanguage
{
    En,
    Ru,
    Hy
}

public static class UiLanguageExtensions
{
    public static string ToStorageCode(this UiLanguage lang) =>
        lang switch
        {
            UiLanguage.Ru => "ru",
            UiLanguage.Hy => "hy",
            _ => "en"
        };

    public static UiLanguage FromStorageCode(string? code) =>
        code?.Trim().ToLowerInvariant() switch
        {
            "ru" => UiLanguage.Ru,
            "hy" => UiLanguage.Hy,
            _ => UiLanguage.En
        };
}
