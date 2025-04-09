using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yukonda.Core.Models;

namespace Yukonda.Core.ViewModels.Windows
{
    public class CreateConnectionWindowViewModel : BindableBase
    {
        private MainWindowViewModel _mainWindowViewModel;

        public event Action<DbProviderModel, string[]> OnConnecting;

        public event EventHandler Show;
        public event EventHandler Close;
        public string Host { get; set; }
        public string Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Database { get; set; }
        public bool IsShown { get; set; }
        public DbProviderModel DbProvider { get; set; }
        public List<DbProviderModel> Providers { get; set; } = new List<DbProviderModel>()
        {
            new DbProviderModel("PostgreSQL", @"/Assets/svgs/postgresql-logo.svg"),
            new DbProviderModel("SQLite", @"/Assets/svgs/sqlite-original-logo.svg"),
        };
        public CreateConnectionWindowViewModel()
        {
            TryConnectCommand = new DelegateCommand(OnTryConnect);
            CancelConnectCommand = new DelegateCommand(OnCancelConnect);
            
        }

        public DelegateCommand TryConnectCommand { get; }
        public DelegateCommand CancelConnectCommand { get; }

        private void OnTryConnect()
        {
            try
            {
                OnConnecting?.Invoke(DbProvider, new string[] { Host, Port, Username, Password, Database });
                IsShown = false;
            }
            catch (Exception ex)
            {
                _mainWindowViewModel.RaiseError(ex);
            }

        }
        private void OnCancelConnect()
        {
            Host = "";
            Port = "";
            Username = "";
            Password = "";
            Database = "";
            DbProvider = null!;
            CloseWindow();
        }

        public void ShowWindow() => IsShown = true;
        public void CloseWindow() => IsShown = false;

        public void Init()
        {
            _mainWindowViewModel = IoC.Resolve<MainWindowViewModel>();
        }

    }
}
