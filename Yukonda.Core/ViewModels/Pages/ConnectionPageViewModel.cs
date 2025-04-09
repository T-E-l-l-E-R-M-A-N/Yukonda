using System.Data;
using Microsoft.Data.Sqlite;
using Npgsql;
using Yukonda.Core.Models;
using Yukonda.Core.ViewModels.Tabs;
using Yukonda.Core.ViewModels.Windows;

namespace Yukonda.Core.ViewModels.Pages
{
    public class ConnectionPageViewModel : PageViewModelBase
    {
        private MainWindowViewModel _mainWindowViewModel;
        private string _currentTable;
        public string[] ConnectionParams { get; set; }
        public ConnectionModel ConnectionModel { get; set; }
        public string ConnectionName { get; set; }
        
        public ConnectionPageViewModel() : base("Connection", PageType.Connection)
        {
            
        }

        public DelegateCommand DisconnectCommand { get; set; }
        public DelegateCommand<SchemaModel> OpenSchemaCommand { get; set; }

        public override void Init()
        {
            _mainWindowViewModel = IoC.Resolve<MainWindowViewModel>();

            DisconnectCommand = new DelegateCommand(OnDisconnect);
            OpenSchemaCommand = new DelegateCommand<SchemaModel>(OnOpenSchema);
        }

        private void OnOpenSchema(SchemaModel obj)
        {
            var tab = IoC.Resolve<TabViewModelBase>();
            tab.Name = $"Schema {ConnectionName} : {obj.SchemaName}";
            var page = IoC.Resolve<IEnumerable<IPage>>().OfType<SchemaPageViewModel>().FirstOrDefault();
            page.Init(ConnectionModel, obj, ConnectionName);
            tab.Page = page;
            _mainWindowViewModel.Tabs.Add(tab);
            _mainWindowViewModel.CurrentTab = tab;
        }

        private void OnDisconnect()
        {
            ConnectionModel?.CloseConnection();
            _mainWindowViewModel.Disconnect(this);
        }

        public void Init(DbProviderModel provider, string[] credentials)
        {
            this.Init();
            ConnectionModel cm = new ConnectionModel();
            cm.DbProvider = provider;
            try
            {
                if (cm != null) cm.CloseConnection();

                string connectionString = "";
                IDbConnection _conn = null!;
                switch (provider.Name.ToLower())
                {
                    case "postgresql":
                        connectionString = $"Host={credentials[0]};Port={credentials[1]};Username={credentials[2]};Password={credentials[3]};Database={credentials[4]}";
                        _conn = new NpgsqlConnection(connectionString);
                        break;
                    case "sqLite":
                        connectionString = $"Data Source={credentials[4]}.db";
                        _conn = new SqliteConnection(connectionString);
                        break;
                }
                ConnectionName = credentials[4].ToUpper();

                cm.Init(_conn);
                ConnectionModel = cm;

            }
            catch (Exception e)
            {
                _mainWindowViewModel.RaiseError(e);
            }
        }
    }
}
