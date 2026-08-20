using ConverPro.Services;
using Microsoft.Extensions.Logging;

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
            fonts.AddFont("FontAwesome7FreeSolid.otf", "FontAwesomeSolid");
        });
        builder.Services.AddSingleton(new HttpClient { Timeout = TimeSpan.FromSeconds(15) });
        builder.Services.AddSingleton<ProcessRunner>();
        builder.Services.AddSingleton<ToolLocator>();
        builder.Services.AddSingleton<MediaService>();
        builder.Services.AddSingleton<UpdateService>();
        builder.Services.AddSingleton<MainPage>();
#if DEBUG
        builder.Logging.AddDebug();
#endif
        return builder.Build();
    }
}
