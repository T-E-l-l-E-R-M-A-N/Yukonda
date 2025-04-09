using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Yukonda.Core;
using Yukonda.Core.ViewModels.Windows;

namespace Yukonda.UI;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);

        IoC.RegisterServices();
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var dataContext = IoC.Resolve<MainWindowViewModel>();
        dataContext.Init();
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow()
            {
                DataContext = dataContext
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}