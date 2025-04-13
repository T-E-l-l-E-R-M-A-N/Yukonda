using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Yukonda.ViewModels;

public abstract partial class PageViewModelBase : ViewModelBase
{
    [ObservableProperty] private MainViewModel _mainViewModel;
    [ObservableProperty] private string _name;

    protected PageViewModelBase(MainViewModel mainViewModel, string name)
    {
        _mainViewModel = mainViewModel;
        Name = name;
    }

    [RelayCommand]
    public void CloseTab()
    {
        if (this is not ConnectionsPageViewModel)
        {
            _mainViewModel.CurrentPage = null!;
            _mainViewModel.Pages.Remove(this);
            _mainViewModel.ReturnCommand.NotifyCanExecuteChanged();
        }
    }
}