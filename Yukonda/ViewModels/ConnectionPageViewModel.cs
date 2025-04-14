using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dapper;
using Yukonda.Models;

namespace Yukonda.ViewModels;

public partial class ConnectionPageViewModel : PageViewModelBase
{
    [ObservableProperty] private ConnectionModel _connection;
    [ObservableProperty] private ObservableCollection<DataTableModel> _tables;
    [ObservableProperty] private DataTableModel _currentTable;
    [ObservableProperty] private string _sql;
    [ObservableProperty] private string _sqlLog;
    [ObservableProperty] private bool _queryToolIsOpen;
    [ObservableProperty] private bool _dataViewIsOpen;
    [ObservableProperty] private bool _addingItem;
    [ObservableProperty] private ObservableCollection<DataTableCellModel> _cells;
    [ObservableProperty] private DataTableRowModel _selectedRow;

    public ConnectionPageViewModel(MainViewModel mainViewModel) : base(mainViewModel, "Connection view")
    {
    }

    public void Init(ConnectionModel connection)
    {
        Tables = new ObservableCollection<DataTableModel>();
        _connection = connection;

        var result =
            _connection.DbConnection.Query(
                "SELECT table_name, table_schema FROM information_schema.tables \nWHERE table_schema = 'public'");
        foreach (var name in result.Where(x => !x.table_name.Contains("hist")))
        {
            var dataTable = new DataTableModel(this, name.table_name, new SchemaModel(name.table_schema, false, null),
                null, null);
            Tables.Add(dataTable);
        }
    }

    [RelayCommand]
    public async Task DeleteItem()
    {
        var attr = SelectedRow.Cells.FirstOrDefault().Attribute;
        var v = SelectedRow.Cells.FirstOrDefault().Value;
        Sql = $"DELETE FROM {CurrentTable.Schema.Name}.\"{CurrentTable.Name}\" WHERE {attr} == '{v}'; \nSELECT * FROM {CurrentTable.Schema.Name}\"{CurrentTable.Name}\";";
        await ExecuteSQL(Sql);
    }

    

    [RelayCommand]
    public void AddItem()
    {
        Cells = new ObservableCollection<DataTableCellModel>();
        foreach (var model in CurrentTable.Columns)
        {
            var cell = new DataTableCellModel(model.Attribute, model.DataType, "", this);
            Cells.Add(cell);
        }
        AddingItem = true;
    }

    [RelayCommand]
    public async Task ConfirmAdding()
    {
        var queryFragment = "";
        foreach (var cell in Cells)
        {
            switch (cell.Type)
            {
                case "TEXT" or "VARCHAR" or "DATE" or "TIMESTAMP" or "TIME":
                    queryFragment += $" '{cell.Value}',";
                    break;
                case "INTEGER" or "NUMERIC" or "REAL" or "DECIMAL" or "BOOLEAN" or "MONEY":
                    if (string.IsNullOrEmpty(cell.Value.ToString()))
                        queryFragment += 0;
                    else
                        queryFragment += $" {cell.Value},";
                    break;
                default:
                    queryFragment += $" '{cell.Value}',";
                    break;
            }
        }

        var q = queryFragment.Substring(0, queryFragment.Length - 1);
        Sql = $"INSERT INTO {CurrentTable.Schema.Name}\"{CurrentTable.Name}\" VALUES ({q})" + $"; \nSELECT * FROM {CurrentTable.Schema.Name}\"{CurrentTable.Name}\";";
        await ExecuteSQL(Sql);
        AddingItem = false;
    }

    [RelayCommand]
    public async Task ExecuteSQL(string sql)
    {
        try
        {
            SqlLog = "";
            if (!sql.ToLower().Contains("select"))
            {
                int ra = await _connection.DbConnection.ExecuteAsync(sql);
                SqlLog = $"{ra} rows affected";
                if (sql.ToUpper().StartsWith("INSERT"))
                    SqlLog += "INSERT 0 1\n";
                else if (sql.ToUpper().StartsWith("DELETE"))
                    SqlLog += "DELETE 0 1\n";
                else if (sql.ToUpper().StartsWith("CREATE"))
                    SqlLog += "CREATE TABLE\n";
                else
                    SqlLog += "\n";
                SqlLog += "QUERY RETURNED SUCCESSFULLY";
            }
            else
            {
                CurrentTable = new(this, "RESULTS VIEW", Tables.FirstOrDefault(x => x.Schema.Name == "public").Schema,
                    new ObservableCollection<DataTableRowModel>(), new ObservableCollection<DataTableColumnModel>());
                CurrentTable.Rows = new ObservableCollection<DataTableRowModel>();
                CurrentTable.Columns = new ObservableCollection<DataTableColumnModel>();
                var query = await Connection.DbConnection.QueryAsync(sql);
                int i = 0;
                int columnIndex = 0;
                foreach (var row in query)
                {
                    if (CurrentTable.Columns.Count == 0)
                    {
                        foreach (var column in ((IDictionary<string, object>)row).Keys)
                        {
                            var columnItem = new DataTableColumnModel(column, columnIndex);

                            CurrentTable.Columns.Add(columnItem);
                            columnIndex += 1;
                        }
                    }

                    var rowModel = new DataTableRowModel(i, new ObservableCollection<DataTableCellModel>());
                    i += 1;


                    foreach (var column in ((IDictionary<string, object>)row).Keys)
                    {
                        try
                        {
                            var d = ((IDictionary<string, object>)row)[column];
                            var columnModel = CurrentTable.Columns.FirstOrDefault(x => x.Attribute == column);
                            rowModel.Cells.Add(new DataTableCellModel(columnModel.Attribute, "TYPE", d, this));
                        }
                        catch (Exception e)
                        {
                            SqlLog += e.Message;
                        }
                    }

                    CurrentTable.Rows.Add(rowModel);
                }
                if (sql.ToUpper().StartsWith("INSERT"))
                    SqlLog += "INSERT 0 1\n";
                else if (sql.ToUpper().StartsWith("DELETE"))
                    SqlLog += "DELETE 0 1\n";
                else if (sql.ToUpper().StartsWith("CREATE"))
                    SqlLog += "CREATE TABLE\n";
                else
                    SqlLog += "\n";
                SqlLog += "QUERY RETURNED SUCCESSFULLY";
            }
        }
        catch (Exception e)
        {
            CurrentTable = null!;
            Console.WriteLine(e);
            SqlLog = e.Message;
        }
    }


    [RelayCommand]
    public void ToggleSqlTool()
    {
        QueryToolIsOpen = !QueryToolIsOpen;
        if (QueryToolIsOpen) DataViewIsOpen = false;
        else
        {
            DataViewIsOpen = true;
        }
        CurrentTable = null!;
    }
}