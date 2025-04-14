using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Npgsql;
using Yukonda.Models;

namespace Yukonda.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    
    [ObservableProperty] private ConnectionPopupViewModel _connectionPopup;
    [ObservableProperty] private ConnectionsPageViewModel _connectionsPageViewModel;
    [ObservableProperty] private ObservableCollection<PageViewModelBase> _pages;
    [ObservableProperty] private PageViewModelBase _currentPage;

    public MainViewModel()
    {
        Init();
        ConnectionsPageViewModel = _pages.FirstOrDefault() as ConnectionsPageViewModel;
    }
    
    public void Init()
    {
        
        Pages = new ObservableCollection<PageViewModelBase>();
        Pages.Add(new ConnectionsPageViewModel(this));
        ConnectionsPageViewModel = _pages.FirstOrDefault() as ConnectionsPageViewModel;
        ConnectionsPageViewModel.Init();
        CurrentPage = ConnectionsPageViewModel;
        ConnectionPopup = new ConnectionPopupViewModel(this, ConnectionsPageViewModel);

        if (File.Exists("connection.db"))
        {
            var connectionString = File.ReadAllLines("connection.db");
            ConnectionPopup.ConnectionModel = new ConnectionModel(this);
            ConnectionPopup.ConnectionModel.Provider = ConnectionPopup.Providers.FirstOrDefault();
            ConnectionPopup.ConnectionModel.Host = connectionString[0];
            ConnectionPopup.ConnectionModel.Port = Convert.ToInt32(connectionString[1]);
            ConnectionPopup.ConnectionModel.User = connectionString[2];
            ConnectionPopup.ConnectionModel.Pass = connectionString[3];
            ConnectionPopup.ConnectionModel.Db = connectionString[4];
            ConnectionPopup.Accept();
        }

    }

    public void DisconnectAll()
    {
        foreach (var connection in _connectionsPageViewModel.Connections)
        {
            connection.Disconnect();
        }
    }

    [RelayCommand(CanExecute = nameof(CanReturn))]
    public void Return()
    {
        Pages.RemoveAt(Pages.IndexOf(Pages.Last()));
        CurrentPage = Pages.LastOrDefault();
        ReturnCommand.NotifyCanExecuteChanged();
    }

    [RelayCommand]
    public void NewConnection()
    {
        _connectionPopup.ShowDialog("новое подключение", "", true);
    }

    [RelayCommand]
    public void CloseAllTabs()
    {
        _pages.Clear();
    }
    
    private bool CanReturn() => Pages.Count != 1;
}