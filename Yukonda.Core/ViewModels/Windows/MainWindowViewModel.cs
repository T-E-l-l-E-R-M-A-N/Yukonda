
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Dynamic;
using Dapper;
using Microsoft.Data.Sqlite;
using Npgsql;
using Yukonda.Core.Models;

namespace Yukonda.Core.ViewModels.Windows
{

    public class MainWindowViewModel : BindableBase
    {
        
        public ConnectionModel Connection { get; set; }
        public ObservableCollection<ConnectionModel> Connections { get; set; }
        public CreateConnectionWindowViewModel CreateConnectionWindowViewModel { get; set; }
        

        public event EventHandler<Exception> Error;
        public MainWindowViewModel(CreateConnectionWindowViewModel createConnectionWindowViewModel)
        {
            CreateConnectionWindowViewModel = createConnectionWindowViewModel;
        }

        public void Init()
        {
            CreateNewConnectionCommand = new DelegateCommand(OnCreateNewConnection);
            CreateConnectionWindowViewModel = IoC.Resolve<CreateConnectionWindowViewModel>();
            
            Connections = new ObservableCollection<ConnectionModel>();
        }

        private void OnCreateNewConnection()
        {
            CreateConnectionWindowViewModel.ShowWindow();
            CreateConnectionWindowViewModel.OnConnecting += CreateConnectionWindowViewModel_OnConnecting;
        }

        private void CreateConnectionWindowViewModel_OnConnecting(DbProviderModel arg1, string[] arg2)
        {
            var connectionModel = new ConnectionModel(this);
            connectionModel.Init(arg1, arg2);
            Connections.Add(connectionModel);
            Connection = connectionModel;
            CreateConnectionWindowViewModel.CloseWindow();

        }

        public DelegateCommand CreateNewConnectionCommand { get; set; }


        public void RaiseError(Exception ex)
        {
            Error?.Invoke(this, ex);
        }

        internal void Disconnect(ConnectionModel connectionModel)
        {
            
            Connections.Remove(connectionModel);
        }
    }
}