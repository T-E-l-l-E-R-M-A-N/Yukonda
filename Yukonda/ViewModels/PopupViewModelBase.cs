using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Yukonda.ViewModels;

public abstract partial class PopupViewModelBase : ViewModelBase
{
    [ObservableProperty] private string _header;
    [ObservableProperty] private string _description;
    [ObservableProperty] private bool _isShown;

    public abstract void ShowDialog(string name, string description, bool isShow);

    [RelayCommand]
    public abstract void Accept();

    [RelayCommand]
    public abstract void Cancel();
}