using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using Npgsql;
using Yukonda.Models;

namespace Yukonda.ViewModels;

public partial class ConnectionPopupViewModel : PopupViewModelBase
{
    readonly MainViewModel _mainViewModel;
    private ConnectionsPageViewModel _connectionsPageView;
    [ObservableProperty] private ConnectionModel _connectionModel = null!;

    [ObservableProperty] private List<ProviderModel> _providers;

    public ConnectionPopupViewModel(MainViewModel mainViewModel, ConnectionsPageViewModel connectionsPageView)
    {
        _mainViewModel = mainViewModel;
        _connectionsPageView = connectionsPageView;
        Providers = new List<ProviderModel>()
        {
            new ProviderModel("PostgreSQL"),
            new ProviderModel("SQLite"),
        };
    }

    public override void ShowDialog(string name, string description, bool isShow)
    {
        Header = name;
        Description = description;
        ConnectionModel = new ConnectionModel(_mainViewModel);
        IsShown = isShow;
    }

    public override void Accept()
    {
        try
        {
            ConnectionModel.DbConnection = new NpgsqlConnection(
                $"Host={ConnectionModel.Host};Port={ConnectionModel.Port};Username={ConnectionModel.User};Password={ConnectionModel.Pass};Database={ConnectionModel.Db}");
            _connectionsPageView.Connections.Add(ConnectionModel);
            
            
            File.WriteAllLines("connection.db", new string[] {ConnectionModel.Host, ConnectionModel.Port.ToString(), ConnectionModel.User, ConnectionModel.Pass, ConnectionModel.Db});
            
            Cancel();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            Cancel();
        }
    }

    public override void Cancel()
    {
        Header = "";
        Description = "";
        IsShown = false;
        ConnectionModel = null!;
    }
}