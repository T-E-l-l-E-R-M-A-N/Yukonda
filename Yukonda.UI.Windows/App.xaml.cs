using System.Configuration;
using System.Data;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using Yukonda.Core;
using Yukonda.Core.ViewModels.Windows;
using Yukonda.UI.Windows.Windows;

namespace Yukonda.UI.Windows;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private CreateConnectionWindow _createConnectionWindow;
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        IoC.RegisterServices();

        var dataContext = IoC.Resolve<MainWindowViewModel>();

        _createConnectionWindow = new CreateConnectionWindow();
        

        var createconnectionVm = IoC.Resolve<CreateConnectionWindowViewModel>();
       
        createconnectionVm.Show += CreateconnectionVm_Show;
        createconnectionVm.Close += CreateconnectionVm_Close;
        createconnectionVm.Init();
        _createConnectionWindow.DataContext = createconnectionVm;

        dataContext.Init();

        var window = new MainWindow();
        window.DataContext = dataContext;
        window.Show();
    }

    private void CreateconnectionVm_Close(object s, EventArgs e)
    {
        _createConnectionWindow.Hide();
    }

    private void CreateconnectionVm_Show(object s, EventArgs e)
    {
        _createConnectionWindow.Show();
    }
}

internal class VisibilityConverter : MarkupExtension, IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool b)
            return b ? Visibility.Visible : Visibility.Collapsed;

        return false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return null!;
    }

    public override object ProvideValue(IServiceProvider serviceProvider) => this;


}
