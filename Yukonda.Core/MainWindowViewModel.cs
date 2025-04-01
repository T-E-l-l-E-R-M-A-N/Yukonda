
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using Dapper;
using Microsoft.Data.Sqlite;
using Npgsql;

namespace Yukonda.Core
{
    public abstract class DbProvider
    {
        public string Name {get;set;}
    }
    public class MainWindowViewModel : BindableBase
    {
        public DbConnection Connection { get; set; }
        public ConnectionViewModel ConnectionView { get; set; }
        public DataTable DataTable { get; set; }
        public ObservableCollection<string> Tables { get; set; } = new();

        public string SqlQuery { get; set; }

        public event EventHandler<Exception> Error;
        public MainWindowViewModel()
        {
            ConnectCommand = new DelegateCommand(OnConnect);
            LoadTablesCommand = new DelegateCommand(OnLoadTables);
            ExecuteQueryCommand = new DelegateCommand(OnExecuteQuery);

            ConnectionView = new ConnectionViewModel();
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
                    tables = Connection.Query<string>("SELECT table_name FROM information.schema.tables WHERE table_schema = \"public\";");
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
                        var result = Connection.Query("SELECT name FROM Customers WHERE id < 10;");
                        Console.WriteLine(result);
                        break;
                    case "SQLite":
                        connectionString = $"Data Source={ConnectionView.Database}.db";
                        Connection = new SqliteConnection(connectionString);
                        break;
                }

                
            }
            catch (Exception e)
            {
                Error?.Invoke(this, e);
            }
        }
    }

}