using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Themes.Fluent;
using System.ComponentModel;
using ViedoAsyncAvalonia.Services;
using ViedoAsyncAvalonia.ViewModels;
using ViedoAsyncAvalonia.Views;

namespace ViedoAsyncAvalonia;

public partial class App : Application
{
    public FilesService FilesService { get; private set; }
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            FilesService = new FilesService(desktop.MainWindow);

            desktop.MainWindow = new MainWindow

            {
                DataContext = new MainViewModel((App)Application.Current)
            };

        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainView
            {
                DataContext = new MainViewModel((App)Application.Current)
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

}
