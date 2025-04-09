
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Dynamic;
using Dapper;
using Microsoft.Data.Sqlite;
using Npgsql;
using Yukonda.Core.Models;
using Yukonda.Core.ViewModels.Pages;
using Yukonda.Core.ViewModels.Tabs;

namespace Yukonda.Core.ViewModels.Windows
{

    public class MainWindowViewModel : BindableBase
    {
        private ConnectionPageViewModel _connection;
        public ObservableCollection<TabViewModelBase> Tabs { get; set; } = new();
        public TabViewModelBase CurrentTab { get; set; }
        public ObservableCollection<ConnectionPageViewModel> Connections { get; set; }

        public ConnectionPageViewModel Connection
        {
            get => _connection;
            set
            {
                _connection = value;
                if (_connection != null)
                {
                    if (Tabs.FirstOrDefault(x => x.Page == _connection) is TabViewModelBase tab)
                    {
                        CurrentTab = tab;
                    }
                    else
                    {
                        tab = IoC.Resolve<TabViewModelBase>();
                        tab.Name = $"Explore: {_connection.ConnectionName}";
                        tab.Page = _connection;
                        Tabs.Add(tab);
                        CurrentTab = tab;
                    }
                }
                RaisePropertyChanged();
            }
        }

        public CreateConnectionWindowViewModel CreateConnectionWindowViewModel { get; set; }
        public event EventHandler<Exception> Error;
        public string ErrorLog { get; set; }
        public MainWindowViewModel(CreateConnectionWindowViewModel createConnectionWindowViewModel)
        {
            CreateConnectionWindowViewModel = createConnectionWindowViewModel;
        }

        public void Init()
        {
            Error += OnError;
            CreateNewConnectionCommand = new DelegateCommand(OnCreateNewConnection);
            CreateConnectionWindowViewModel = IoC.Resolve<CreateConnectionWindowViewModel>();
            CloseCommand = new DelegateCommand<TabViewModelBase>(OnCloseTab);
            
            Connections = new ObservableCollection<ConnectionPageViewModel>();

            CreateConnectionWindowViewModel_OnConnecting(new DbProviderModel("PostgreSQL", ""), new string[] { "localhost", "5432", "postgres", "00001111", "postgres" });
        }

        private async void OnError(object? sender, Exception e)
        {
            ErrorLog += e.Message;
            if (e.InnerException != null) ErrorLog += e.InnerException.Message;
            await Task.Delay(5000);
            ErrorLog = "";
        }

        private void OnCloseTab(TabViewModelBase obj)
        {
            if (Connection == obj.Page)
                Connection = null!;
            Tabs.Remove(obj);
            CurrentTab = null!;
        }

        private void OnCreateNewConnection()
        {
            CreateConnectionWindowViewModel.ShowWindow();
            CreateConnectionWindowViewModel.OnConnecting += CreateConnectionWindowViewModel_OnConnecting;
        }

        private void CreateConnectionWindowViewModel_OnConnecting(DbProviderModel arg1, string[] arg2)
        {
            var connectionModel = IoC.Resolve<IEnumerable<IPage>>().OfType<ConnectionPageViewModel>().FirstOrDefault();
            connectionModel.Init(arg1, arg2);
            connectionModel.ConnectionParams = arg2;
            Connections.Add(connectionModel);
            Connection = connectionModel;

        }

        public DelegateCommand CreateNewConnectionCommand { get; set; }
        public DelegateCommand<TabViewModelBase> CloseCommand { get; set; }


        public void RaiseError(Exception ex)
        {
            Error?.Invoke(this, ex);
        }

        internal void Disconnect(ConnectionPageViewModel connectionModel)
        {
            
            Connections.Remove(connectionModel);
            var tab = Tabs.FirstOrDefault(x => x.Page == connectionModel);
            Tabs.Remove(tab);
            CurrentTab = null!;
        }
    }
}