using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using WinRT.Interop;

namespace MyPlanner.WinUI;

public partial class App : MauiWinUIApplication
{
    public App()
    {
        this.InitializeComponent();
    }

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        base.OnLaunched(args);

        var mauiWindow = Microsoft.Maui.Controls.Application.Current.Windows[0];

        var nativeWindow = mauiWindow.Handler.PlatformView
            as Microsoft.UI.Xaml.Window;

        if (nativeWindow != null)
        {
            IntPtr hwnd = WindowNative.GetWindowHandle(nativeWindow);
            WindowId windowId = Win32Interop.GetWindowIdFromWindow(hwnd);

            AppWindow appWindow = AppWindow.GetFromWindowId(windowId);

            // Максимизация окна
            if (appWindow.Presenter is OverlappedPresenter presenter)
            {
                presenter.Maximize();
            }

            // Настоящий fullscreen:
            // appWindow.SetPresenter(AppWindowPresenterKind.FullScreen);
        }
    }
}