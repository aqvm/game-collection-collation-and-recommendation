namespace GameLibrary.Desktop;

public sealed partial class App : Microsoft.UI.Xaml.Application
{
    private Window? _window;

    public App()
    {
        InitializeComponent();
    }

    protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
    {
        Resources.Build(resources => resources.Merged(new XamlControlsResources()));
        _window ??= new Window();
        _window.Content ??= new MainPage();
        _window.SetWindowIcon();
        _window.Activate();
    }
}
