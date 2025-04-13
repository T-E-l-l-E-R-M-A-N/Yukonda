using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Svg;
using Avalonia.Threading;
using SkiaSharp;
using SKMatrix = ShimSkiaSharp.SKMatrix;
using SKPaint = ShimSkiaSharp.SKPaint;
using SKPaintStyle = ShimSkiaSharp.SKPaintStyle;
using SKPath = ShimSkiaSharp.SKPath;
using SKPoint = ShimSkiaSharp.SKPoint;
using SKRect = ShimSkiaSharp.SKRect;

namespace Yukonda.Views;

public partial class ConnectionPageView : UserControl
{
    public ConnectionPageView()
    {
        InitializeComponent();
    }
    
    
}