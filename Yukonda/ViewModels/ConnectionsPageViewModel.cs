using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Yukonda.Models;

namespace Yukonda.ViewModels;

public partial class ConnectionsPageViewModel : PageViewModelBase
{
    [ObservableProperty] private ObservableCollection<ConnectionModel> _connections;
    public ConnectionsPageViewModel(MainViewModel mainViewModel) : base(mainViewModel, "Подключенные базы данных")
    {
    }

    public void Init()
    {
        Connections = new();
    }
    
    [RelayCommand]
    public void NewConnection()
    {
        MainViewModel.NewConnection();
    }
}