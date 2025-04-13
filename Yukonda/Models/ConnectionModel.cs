using System.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Yukonda.ViewModels;

namespace Yukonda.Models;

public partial class ConnectionModel : ViewModelBase
{
    private MainViewModel _mainViewModel;
    [ObservableProperty] private IDbConnection _dbConnection;
    [ObservableProperty] private ProviderModel _provider;
    [ObservableProperty] private string _host;
    [ObservableProperty] private int _port;
    [ObservableProperty] private string _user;
    [ObservableProperty] private string _pass;
    [ObservableProperty] private string _db;

    public ConnectionModel(MainViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;
        
    }

    [RelayCommand]
    public void OpenConnection()
    {
        var page = new ConnectionPageViewModel(_mainViewModel);
        page.Init(this);
        page.Name = $"Обзор: {Db}";
        _mainViewModel.Pages.Add(page);
        _mainViewModel.CurrentPage = page;
        _mainViewModel.ReturnCommand.NotifyCanExecuteChanged();
    }
    [RelayCommand]
    public void Disconnect()
    {
        if(_dbConnection != null) _dbConnection.Close();
        _mainViewModel.Return();
        _mainViewModel.ConnectionsPageViewModel.Connections.Remove(this);
    }
}