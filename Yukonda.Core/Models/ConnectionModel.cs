using System.Collections.ObjectModel;
using System.Data;
using Dapper;
using Microsoft.Data.Sqlite;
using Npgsql;
using Yukonda.Core.ViewModels.Windows;

namespace Yukonda.Core.Models
{
    public class ConnectionModel : BindableBase
    {
        private readonly MainWindowViewModel _mainWindowViewModel;

        public IDbConnection Connection { get; set; }
        public string ConnectionName { get; set; }
        private string _currentTable;

        public bool Connected { get; set; }
        public ObservableCollection<string> Tables { get; set; } = new();
        public ObservableCollection<string> Columns { get; set; }
        public DataTable DataTable { get; set; }
        public string CurrentTable
        {
            get => _currentTable;
            set
            {
                _currentTable = value;
                SqlQuery = $"SELECT * FROM {_currentTable}";
                ExecuteQueryCommand.Execute();
            }
        }

        public string SqlQuery { get; set; }
        public DbProviderModel DbProvider { get; set; }

        public ConnectionModel(MainWindowViewModel mainWindowViewModel)
        {
            _mainWindowViewModel = mainWindowViewModel;

            
        }
        public DelegateCommand DisconnectCommand { get; set; }
        public DelegateCommand ExecuteQueryCommand { get; set; }
        private void OnDisconnect()
        {
            Connection.Close();
            _mainWindowViewModel.Disconnect(this);
        }

        private void OnExecuteQuery()
        {
            if (Connection == null) return;

            try
            {
                var result = Connection.Query(SqlQuery);
                var table = new DataTable();

                foreach (var row in result)
                {
                    if (table.Columns.Count == 0)
                    {
                        foreach (var column in ((IDictionary<string, object>)row).Keys)
                        {
                            table.Columns.Add(column);
                        }
                    }

                    var newRow = table.NewRow();
                    foreach (var column in ((IDictionary<string, object>)row).Keys)
                    {
                        newRow[column] = ((IDictionary<string, object>)row)[column];
                    }
                    table.Rows.Add(newRow);
                }

                DataTable = table;
            }
            catch (Exception ex)
            {
                _mainWindowViewModel.RaiseError(ex);
            }
        }

        private void OnLoadTables()
        {
            if (Connection == null) return;

            Tables.Clear();
            IEnumerable<string> tables = null!;
            switch (DbProvider.Name)
            {
                case "PostgreSQL":
                    tables = Connection.Query<string>("SELECT table_name FROM information_schema.tables WHERE table_schema='public' AND table_type='BASE TABLE';");
                    break;
                case "SQLite":
                    tables = Connection.Query<string>("SELECT name FROM sqlite_master WHERE type = \"table\";");
                    break;
            }
            foreach (var table in tables)
            {
                Tables.Add(table);
            }
        }

        public void Init(DbProviderModel provider, string[] credentials)
        {
            DisconnectCommand = new DelegateCommand(OnDisconnect);
            ExecuteQueryCommand = new DelegateCommand(OnExecuteQuery);
            DbProvider = provider;
            try
            {
                if (Connection != null) Connection.Close();

                string connectionString = "";

                switch (provider.Name.ToLower())
                {
                    case "postgresql":
                        ConnectionName = credentials[4];
                        connectionString = $"Host={credentials[0]};Port={credentials[1]};Username={credentials[2]};Password={credentials[3]};Database={credentials[4]}";
                        Connection = new NpgsqlConnection(connectionString);
                        break;
                    case "sqLite":
                        connectionString = $"Data Source={credentials[4]}.db";
                        Connection = new SqliteConnection(connectionString);
                        break;
                }
                ConnectionName = credentials[4].ToUpper();

                OnLoadTables();


            }
            catch (Exception e)
            {
                _mainWindowViewModel.RaiseError(e);
            }
        }
    }
}