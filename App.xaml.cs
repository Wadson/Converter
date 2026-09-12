namespace ConverPro;

public partial class App : Application
{
    private readonly AppShell _shell;
    public App(IServiceProvider services)
    {
        InitializeComponent();
        _shell = services.GetRequiredService<AppShell>();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var window = new Window(_shell)
        {
            Title = "ConverPro", Width = 1120, Height = 760, MinimumWidth = 900, MinimumHeight = 640
        };
        window.Created += (_, _) => CenterWindow(window);
        return window;
    }

    private static void CenterWindow(Window window)
    {
        if (window.Handler?.PlatformView is not Microsoft.UI.Xaml.Window nativeWindow) return;

        var handle = WinRT.Interop.WindowNative.GetWindowHandle(nativeWindow);
        var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(handle);
        var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);
        var display = Microsoft.UI.Windowing.DisplayArea.GetFromWindowId(
            windowId, Microsoft.UI.Windowing.DisplayAreaFallback.Primary);
        var workArea = display.WorkArea;
        var width = Math.Min(1120, workArea.Width);
        var height = Math.Min(760, workArea.Height);
        var x = workArea.X + (workArea.Width - width) / 2;
        var y = workArea.Y + (workArea.Height - height) / 2;
        appWindow.MoveAndResize(new Windows.Graphics.RectInt32(x, y, width, height));
    }
}
