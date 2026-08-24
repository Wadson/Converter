using ConverPro.Services;
using Microsoft.Extensions.Logging;
using YoutubeExplode;
using ConverPro.ViewModels;

namespace ConverPro;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>().ConfigureFonts(fonts =>
        {
            fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
        });
        builder.Services.AddSingleton(new HttpClient { Timeout = Timeout.InfiniteTimeSpan });
        builder.Services.AddSingleton<ProcessRunner>();
        builder.Services.AddSingleton<ToolLocator>();
        builder.Services.AddSingleton<MediaService>();
        builder.Services.AddSingleton<YoutubeClient>();
        builder.Services.AddSingleton<YoutubeDownloadService>();
        builder.Services.AddSingleton<UpdateService>();
        builder.Services.AddSingleton<MainViewModel>();
        builder.Services.AddSingleton<MainPage>();
        builder.Services.AddSingleton<Pages.ConvertPage>();
        builder.Services.AddSingleton<Pages.CompressPage>();
        builder.Services.AddSingleton<Pages.ActivityPage>();
        builder.Services.AddSingleton<Pages.AboutPage>();
        builder.Services.AddSingleton<AppShell>();
#if DEBUG
        builder.Logging.AddDebug();
#endif
        return builder.Build();
    }
}
