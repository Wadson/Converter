namespace ConverPro;

public partial class App : Application
{
    private readonly MainPage _mainPage;
    public App(MainPage mainPage) { InitializeComponent(); _mainPage = mainPage; }

    protected override Window CreateWindow(IActivationState? activationState) => new(new NavigationPage(_mainPage))
    {
        Title = "ConverPro", Width = 1120, Height = 760, MinimumWidth = 900, MinimumHeight = 640
    };
}
