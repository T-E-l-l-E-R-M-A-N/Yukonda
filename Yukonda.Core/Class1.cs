
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using Dapper;
using Microsoft.Data.Sqlite;
using Npgsql;

namespace Yukonda.Core;

public class MainWindowViewModel : BindableBase
{
    private DbConnection _connection;
    public ConnectionViewModel ConnectionView { get; set; }
    public DataTable DataTable { get; set; }
    public ObservableCollection<string> Tables { get; set; } = new();

    public string SqlQuery {get;set;}

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
        if(_connection == null) return;

        try
        {
            var result = _connection.Query(SqlQuery);
            var table = new DataTable();

            foreach(var row in result)
            {
                if(table.Columns.Count == 0)
                {
                    foreach(var column in ((IDictionary<string, object>)row).Keys)
                    {
                        table.Columns.Add(column);
                    }
                }

                var newRow = table.NewRow();
                foreach(var column in ((IDictionary<string, object>)row).Keys)
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
        if(_connection == null) return;

        Tables.Clear();
        IEnumerable<string> tables = null!;
        switch (ConnectionView.DbProvider)
        {
            case "PostgreSQL":
                tables = _connection.Query<string>("SELECT table_name FROM information.schema.tables WHERE table_schema = \"public\";");
                break;
            case "SQLite":
                tables = _connection.Query<string>("SELECT name FROM sqlite_master WHERE type = \"table\";");
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
            if (_connection != null) _connection.Close();

            string connectionString;

            switch (ConnectionView.DbProvider)
            {
                case "PostgreSQL":
                    connectionString = $"Host={ConnectionView.Host};Port={ConnectionView.Port};Username={ConnectionView.Username};Password={ConnectionView.Password};Database={ConnectionView.Database}";
                    _connection = new NpgsqlConnection(connectionString);
                    break;
                case "SQLite":
                    connectionString = $"Data Source={ConnectionView.Database}.db";
                    _connection = new SqliteConnection(connectionString);
                    break;
            }
        }
        catch (Exception e)
        {
            Error?.Invoke(this, e);
        }
    }
}

public class ConnectionViewModel : BindableBase
{
    public string Host { get; set; }
    public string Port { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Database { get; set; }
    public string DbProvider { get; set; }
}