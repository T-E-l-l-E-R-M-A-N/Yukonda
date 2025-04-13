using System.Runtime.Versioning;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Browser;
using Avalonia.Media;
using Yukonda;

internal sealed partial class Program
{
    private static Task Main(string[] args) => BuildAvaloniaApp()
            .With(new FontManagerOptions
            {
                DefaultFamilyName = "avares://Yukonda/Assets/Fonts/OpenSans-Light.ttf#Open Sans Light",
            })
            .With(new FontManagerOptions
            {
                DefaultFamilyName = "avares://Yukonda/Assets/Fonts/OpenSans-Regular.ttf#Open Sans",
            })
            .With(new FontManagerOptions
            {
                DefaultFamilyName = "avares://Yukonda/Assets/Fonts/OpenSans-Bold.ttf#Open Sans",
            })
            .WithInterFont()
            .StartBrowserAppAsync("out");

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>();
}