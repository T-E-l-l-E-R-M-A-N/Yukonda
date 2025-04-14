using System;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using Aspose.Imaging;
using Aspose.Imaging.ImageFilters.FilterOptions;
using Aspose.Imaging.Sources;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Microsoft.Win32;

namespace Yukonda.ValueConverters;

public class DesktopImageToWindowBackgroundConverter : MarkupExtension, IValueConverter
{
    public override object ProvideValue(IServiceProvider serviceProvider) => this;

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            var desktopImagePath = Registry.CurrentUser.OpenSubKey("Control Panel").OpenSubKey("Desktop").GetValue("Wallpaper");
            using (Image image = Image.Load(desktopImagePath.ToString()))
            {
                // Преобразовать изображение в RasterImage
                RasterImage rasterImage = (RasterImage)image;

                // Применить эффект размытия
                rasterImage.Filter(rasterImage.Bounds, new GaussianBlurFilterOptions(30,90));

                // Сохранить размытое изображение
                rasterImage.Save("background.bmp");
                return new Bitmap("background.bmp");
            }
        }

        return null!;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class HalfOfWidthConverter : MarkupExtension, IValueConverter
{
    public override object ProvideValue(IServiceProvider serviceProvider) => this;

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is double d)
        {
            return d / 2;
        }

        return null!;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}