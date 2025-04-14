using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using Dapper;
using Npgsql;
using Yukonda.Models;

namespace Yukonda.ViewModels;

public partial class ConnectionPopupViewModel : PopupViewModelBase
{
    readonly MainViewModel _mainViewModel;
    private ConnectionsPageViewModel _connectionsPageView;
    [ObservableProperty] private ConnectionModel _connectionModel = null!;

    [ObservableProperty] private List<ProviderModel> _providers;
    [ObservableProperty] private string _exMessage;
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
            if(ConnectionModel.Port == 0)
                ConnectionModel.DbConnection = new NpgsqlConnection(
                    $"Host={ConnectionModel.Host};Username={ConnectionModel.User};Password={ConnectionModel.Pass};Database={ConnectionModel.Db}");
            else
                ConnectionModel.DbConnection = new NpgsqlConnection(
                $"Host={ConnectionModel.Host};Port={ConnectionModel.Port};Username={ConnectionModel.User};Password={ConnectionModel.Pass};Database={ConnectionModel.Db}");
            _connectionsPageView.Connections.Add(ConnectionModel);
            
            
            
            File.WriteAllLines("connection.db", new string[] {ConnectionModel.Host, ConnectionModel.Port.ToString(), ConnectionModel.User, ConnectionModel.Pass, ConnectionModel.Db});
            
            ConnectionModel.OpenConnection();
            Cancel();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            ExMessage += "Data base connection is not in open state\n";
            ExMessage += e;
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