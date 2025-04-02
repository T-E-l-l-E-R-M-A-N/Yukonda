
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Dynamic;
using Dapper;
using Microsoft.Data.Sqlite;
using Npgsql;

namespace Yukonda.Core
{
    public class DbProvider
    {
        public string Name {get;set;}
    }
    public class MainWindowViewModel : BindableBase
    {
        private string _currentTable;

        public bool Connected {get;set;}
        public DbConnection Connection { get; set; }
        public ConnectionViewModel ConnectionView { get; set; }
        public ObservableCollection<string> Tables { get; set; } = new();
        public ObservableCollection<string> Columns {get;set;}
        public ObservableCollection<KeyValuePair<string,object>> Data {get;set;}
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

        public event EventHandler<Exception> Error;
        public MainWindowViewModel()
        {
            ConnectCommand = new DelegateCommand(OnConnect);
            LoadTablesCommand = new DelegateCommand(OnLoadTables);
            ExecuteQueryCommand = new DelegateCommand(OnExecuteQuery);

            ConnectionView = new ConnectionViewModel();

            if(System.IO.File.Exists("connection.db"))
            {
                Connection = new NpgsqlConnection(System.IO.File.ReadAllText("connection.db"));
                if(Connection != null)
                {
                    Connected = true;
                    ConnectionView.DbProvider = new DbProvider() {Name = "PostgreSQL"};
                    LoadTablesCommand.Execute();
                    CurrentTable = Tables[0];
                }
            }
        }

        private void OnExecuteQuery()
        {
            if (Connection == null) return;

            try
            {
                var result = Connection.Query(SqlQuery).ToList();
                if (!result.Any()) return;

                Data = new ObservableCollection<KeyValuePair<string,object>>();

                foreach(var p in result)
                {
                    foreach (var kp in p)
                    {
                        Data.Add(kp);
                    }
                }
                
            }
            catch (Exception e)
            {
                Error?.Invoke(this, e);
            }
        }

        private void OnLoadTables()
        {
            if (Connection == null) return;

            Tables.Clear();
            IEnumerable<string> tables = null!;
            switch (ConnectionView.DbProvider.Name)
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

        public DelegateCommand ConnectCommand { get; }
        public DelegateCommand LoadTablesCommand { get; }
        public DelegateCommand ExecuteQueryCommand { get; set; }


        private void OnConnect()
        {
            try
            {
                if (Connection != null) Connection.Close();

                string connectionString;

                switch (ConnectionView.DbProvider.Name)
                {
                    case "PostgreSQL":
                    
                        connectionString = $"Host={ConnectionView.Host};Port={ConnectionView.Port};Username={ConnectionView.Username};Password={ConnectionView.Password};Database={ConnectionView.Database}";
                        Connection = new NpgsqlConnection(connectionString);
                        if(Connection != null)
                        {
                            System.IO.File.WriteAllText("connection.db", connectionString);
                            Connected = true;
                        }
                        var result = Connection.Query("SELECT * FROM Customers WHERE id < 10;");
                        Console.WriteLine(result);
                        break;
                    case "SQLite":
                        connectionString = $"Data Source={ConnectionView.Database}.db";
                        Connection = new SqliteConnection(connectionString);
                        break;
                }

                LoadTablesCommand.Execute();

                
            }
            catch (Exception e)
            {
                Error?.Invoke(this, e);
            }
        }
    }


}