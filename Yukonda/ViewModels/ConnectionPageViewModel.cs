using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dapper;
using Yukonda.Models;

namespace Yukonda.ViewModels;

public partial class ConnectionPageViewModel : PageViewModelBase
{
    [ObservableProperty] private ConnectionModel _connection;
    [ObservableProperty] private ObservableCollection<DataTableModel> _tables;
    [ObservableProperty] private ObservableCollection<DataTableColumnModel> _columns;
    [ObservableProperty] private ObservableCollection<DataTableRowModel> _data;
    [ObservableProperty] private string _sql;
    [ObservableProperty] private string _sqlLog;

    public ConnectionPageViewModel(MainViewModel mainViewModel) : base(mainViewModel, "Connection view")
    {
    }

    public void Init(ConnectionModel connection)
    {
        Data = new ObservableCollection<DataTableRowModel>();
        Columns = new ObservableCollection<DataTableColumnModel>();
        Tables = new ObservableCollection<DataTableModel>();
        _connection = connection;

        var result =
            _connection.DbConnection.Query(
                "SELECT table_name, table_schema FROM information_schema.tables \nWHERE table_schema = 'public'");
        foreach (var name in result)
        {
            var dataTable = new DataTableModel(name.table_name, new SchemaModel(name.table_schema, false, null), null, null);
            Tables.Add(dataTable);
        }
    }

    [RelayCommand]
    public void ExecuteSQL(string sql)
    {
        try
        {
            var result = _connection.DbConnection.Query(sql);
            Data.Clear();
            Columns.Clear();
            int i = 0;
            int columnIndex = 0;
            foreach (var row in result)
            {
                if (Columns.Count == 0)
                {
                    foreach (var column in ((IDictionary<string, object>)row).Keys)
                    {
                        var columnItem = new DataTableColumnModel(column, columnIndex);

                        Columns.Add(columnItem);
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
                        var columnModel = Columns.FirstOrDefault(x => x.Attribute == column);
                        rowModel.Cells.Add(new DataTableCellModel(columnModel.Attribute, "TYPE", d));
                    }
                    catch (Exception e)
                    {
                        _sqlLog += e.Message;
                    }
                }

                Data.Add(rowModel);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            Sql = e.Message;
        }
    }
}