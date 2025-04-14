using System;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Browser;
using Avalonia.Media;
using Yukonda;

internal sealed partial class Program
{
    private static Task Main(string[] args)
    { 
        try
        {
            return BuildAvaloniaApp()
                .WithInterFont()
                .StartBrowserAppAsync("out");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>();
}