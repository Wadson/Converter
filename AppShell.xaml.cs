using ConverPro.Pages;

namespace ConverPro;

public partial class AppShell : Shell
{
    public AppShell(MainPage download, ConvertPage convert, CompressPage compress, ActivityPage activity, AboutPage about)
    {
        InitializeComponent();
        VersionLabel.Text = $"Versão {AppInfo.Current.VersionString}";
        Items.Add(Item("Baixar do YouTube", "nav_download.png", "download", download));
        Items.Add(Item("Converter vídeos", "nav_convert.png", "convert", convert));
        Items.Add(Item("Comprimir MP3", "nav_compress.png", "compress", compress));
        Items.Add(Item("Atividades e falhas", "nav_activity.png", "activity", activity));
        Items.Add(Item("Sobre", "nav_info.png", "about", about));
    }

    private static FlyoutItem Item(string title, string icon, string route, Page page) => new()
    {
        Title = title, Icon = icon, FlyoutIcon = icon, Route = route,
        Items = { new ShellContent { Title = title, Icon = icon, FlyoutIcon = icon, Content = page } }
    };
}
