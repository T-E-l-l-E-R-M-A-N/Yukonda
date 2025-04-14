using System;
using System.ComponentModel;
using System.Globalization;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Styling;
using Yukonda.ViewModels;

namespace Yukonda.Views;

public partial class MainView : UserControl
{
    private Animation animation;
    public MainView()
    {
        InitializeComponent();
        animation = new Animation();
        animation.Easing = new ExponentialEaseOut();
        animation.Duration = TimeSpan.Parse("0:0:0.9");
        animation.Children.Add(new KeyFrame()
        {
            Cue = new Cue(0.0),
            Setters = { new Setter(Grid.MarginProperty, Thickness.Parse("200 0 -200 0")) }
        });
        animation.Children.Add(new KeyFrame()
        {
            Cue = new Cue(1.0),
            Setters = { new Setter(Grid.MarginProperty, Thickness.Parse("0 0 0 0")) }
        });
        
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        if (change.Property == DataContextProperty)
        {
            if(DataContext != null)
                (DataContext as MainViewModel).PropertyChanged += PropertyChanged;
        }
        
    }

    private void PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == "CurrentPage")
        {
            animation.RunAsync(ContentGrid);
        }
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        animation.RunAsync(ContentGrid);
    }
}