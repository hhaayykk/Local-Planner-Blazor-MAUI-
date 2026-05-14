using Microsoft.Extensions.Logging;
using MyPlanner.Services;

namespace MyPlanner;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder.UseMauiApp<App>();

		builder.Services.AddMauiBlazorWebView();

		builder.Services.AddScoped<IDataService, FileDataService>();
		builder.Services.AddScoped<ActivityStatsNotifier>();
		builder.Services.AddScoped<LocalizationService>();
		builder.Services.AddScoped<ThemeService>();

#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
