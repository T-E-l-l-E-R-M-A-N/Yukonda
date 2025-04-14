using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dapper;
using Yukonda.ViewModels;

namespace Yukonda.Models;

public partial class DataTableModel : ViewModelBase
{
    private ConnectionPageViewModel _connectionPageViewModel;
    [ObservableProperty] private string _name;
    [ObservableProperty] private SchemaModel _schema;
    [ObservableProperty] private ObservableCollection<DataTableRowModel> _rows;
    [ObservableProperty] private ObservableCollection<DataTableColumnModel> _columns;
    [ObservableProperty] private string _rowsCount;
    

    public DataTableModel(ConnectionPageViewModel connectionPageViewModel, string name, SchemaModel schema, ObservableCollection<DataTableRowModel> rows, ObservableCollection<DataTableColumnModel> columns)
    {
        _name = name;
        _schema = schema;
        _rows = rows;
        _columns = columns;
        _connectionPageViewModel = connectionPageViewModel;
        OpenTableCommand = new AsyncRelayCommand(OpenTable);
    }

    public AsyncRelayCommand OpenTableCommand { get; }
    private async Task OpenTable()
    {
        Rows = new ObservableCollection<DataTableRowModel>();
        Columns = new ObservableCollection<DataTableColumnModel>();
        var dataTypes = _connectionPageViewModel.Connection.DbConnection.Query(
            $"SELECT column_name, data_type\nFROM information_schema.columns\nWHERE table_schema = '{Schema.Name}' AND \ntable_name = '{Name}';");
        _connectionPageViewModel.QueryToolIsOpen = false;
        _connectionPageViewModel.DataViewIsOpen = true;
        _connectionPageViewModel.CurrentTable = this;
        var query = await _connectionPageViewModel.Connection.DbConnection.QueryAsync($"SELECT * FROM {Schema.Name}.\"{Name}\"");
        Rows.Clear();
        Columns.Clear();
        int i = 0;
        int columnIndex = 0;
        foreach (var row in query)
        {
            if (Columns.Count == 0)
            {
                foreach (var column in ((IDictionary<string, object>)row).Keys)
                {
                    var columnItem = new DataTableColumnModel(column, columnIndex);
                    foreach (var dataType in dataTypes)
                    {
                        columnItem.DataType = dataType.data_type switch
                        {
                            "integer" or "bigint" or "smallint" => "INTEGER",
                            "character varying (255)" or "character varying" or "name" => "VARCHAR",
                            "boolean" => "BOOLEAN",
                            "date" => "DATE",
                            "timestamp" => "TIMESTAMP",
                            "time" => "TIME",
                            "character" => "CHAR",
                            "text" => "TEXT",
                            "serial" => "SERIAL",
                            "numeric" => "NUMERIC",
                            "decimal" => "DECIMAL",
                            "real" => "REAL",
                            "money" => "MONEY"
                        };
                    }
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
                    rowModel.Cells.Add(new DataTableCellModel(columnModel.Attribute, "TYPE", d, _connectionPageViewModel) {Column = columnIndex, Row = i});
                }
                catch (Exception e)
                {
                    _connectionPageViewModel.SqlLog += e.Message;
                }
            }

            Rows.Add(rowModel);
            RowsCount = $"Записей в таблице: {i + 1}";
        }
        
    }
}