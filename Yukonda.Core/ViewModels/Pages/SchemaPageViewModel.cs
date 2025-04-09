using System.Collections.ObjectModel;
using Dapper;
using Yukonda.Core.Models;
using Yukonda.Core.ViewModels.Controls;
using Yukonda.Core.ViewModels.Windows;

namespace Yukonda.Core.ViewModels.Pages;

public class SchemaPageViewModel : PageViewModelBase
{
    private MainWindowViewModel _mainWindowViewModel;
    private string _currentTable;
    private ConnectionModel _connectionModel;

    public InsertItemPopupViewModel InsertItemPopupViewModel { get; set; }
    public SchemaModel SchemaModel { get; set; }

    public string SqlQuery { get; set; }

    public string CurrentTable
    {
        get => _currentTable;
        set
        {
            _currentTable = value;
            if (_currentTable != null)
            {
                SqlQuery = $"SELECT * FROM {_currentTable}";
                ExecuteQueryCommand.Execute();
                RaisePropertyChanged();
            }
        }
    }

    public ObservableCollection<DataTableColumnModel> Data { get; set; } = new();
    public DataTableRowModel SelectedDataTableRowModel { get; set; }
    public DataTableColumnModel SelectedDataTableColumnModel { get; set; }

    public SchemaPageViewModel(InsertItemPopupViewModel insertItemPopupViewModel) : base("Schema View", PageType.Schema)
    {
        InsertItemPopupViewModel = insertItemPopupViewModel;
    }

    public DelegateCommand ExecuteQueryCommand { get; set; }
    public DelegateCommand AddColumnCommand { get; set; }
    public DelegateCommand AddRowCommand { get; set; }
    public DelegateCommand<DataTableCellModel> SelectRowCommand { get; set; }
    public DelegateCommand<DataTableColumnModel> SelectColumnCommand { get; set; }
    public DelegateCommand DeleteSelectedRowCommand { get; set; }
    public DelegateCommand DeleteSelectedColumnCommand { get; set; }
    public DelegateCommand ClearTableCommand { get; set; }
    public DelegateCommand DeleteTableCommand { get; set; }

    private void OnExecuteQuery()
    {
        if (_connectionModel == null) return;

        try
        {
            Data.Clear();
            var result = _connectionModel.DbConnection.Query(SqlQuery);
            var dataTypes = _connectionModel.DbConnection.Query(
                $"SELECT column_name, data_type\nFROM information_schema.columns\nWHERE table_schema = '{SchemaModel.SchemaName}' AND \ntable_name = '{_currentTable}';");
            var table = new ObservableCollection<DataTableColumnModel>();
            int i = 0;
            if (result.Count() == 0)
            {
                foreach (var dataType in dataTypes)
                {
                    table.Add(new DataTableColumnModel()
                    {
                        Header = dataType.column_name,
                        DataType = dataType.data_type switch
                        {
                            "integer" or "bigint" or "smallint" => "INTEGER",
                            "character varying (255)" or "character varying" => "VARCHAR",
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
                        }
                    });
                }
            }
            else
            {
                foreach (var row in result)
                {
                    if (table.Count == 0)
                    {
                        foreach (var column in ((IDictionary<string, object>)row).Keys)
                        {
                            var columnItem = new DataTableColumnModel();
                            columnItem.Header = column;
                            var columnInfo = dataTypes.FirstOrDefault(x => x.column_name == column);
                            columnItem.DataType = columnInfo.data_type switch
                            {
                                "integer" or "bigint" or "smallint" => "INTEGER",
                                "character varying (255)" or "character varying" => "VARCHAR",
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
                            columnItem.Items = new();

                            table.Add(columnItem);
                        }
                    }

                    i += 1;

                    foreach (var column in ((IDictionary<string, object>)row).Keys)
                    {
                        try
                        {
                            var d = ((IDictionary<string, object>)row)[column];
                            var columnModel = table.FirstOrDefault(x => x.Header == column);
                            columnModel.Items.Add(new DataTableCellModel()
                            {
                                AttributeName = column,
                                RowIndex = i,
                                Value = d == null ? "NULL" : d.ToString()
                            });
                        }
                        catch (Exception e)
                        {
                            _mainWindowViewModel.RaiseError(e);
                        }
                    }
                }
            }

            Data = table;
        }
        catch (Exception ex)
        {
            _mainWindowViewModel.RaiseError(ex);
        }
    }

    public override void Init()
    {
        InsertItemPopupViewModel.InsertRequested += InsertItemPopupViewModelOnInsertRequested;
        _mainWindowViewModel = IoC.Resolve<MainWindowViewModel>();
        ExecuteQueryCommand = new DelegateCommand(OnExecuteQuery);
        AddColumnCommand = new DelegateCommand(OnAddColumn);
        AddRowCommand = new DelegateCommand(OnAddRow);
        SelectRowCommand = new DelegateCommand<DataTableCellModel>(OnSelectRow);
        SelectColumnCommand = new DelegateCommand<DataTableColumnModel>(OnSelectColumn);
        DeleteSelectedRowCommand = new DelegateCommand(OnDeleteSelectedRow);
        DeleteSelectedColumnCommand = new DelegateCommand(OnDeleteSelectedColumn);
        ClearTableCommand = new DelegateCommand(OnClearTable);
        DeleteTableCommand = new DelegateCommand(OnDeleteTable);
    }

    private void OnDeleteTable()
    {
        SqlQuery = $"DROP TABLE {CurrentTable} CASCADE ; ";
        ExecuteQueryCommand.Execute();
        SchemaModel.Tables.Remove(CurrentTable);
        SelectedDataTableRowModel = null!;
        SelectedDataTableColumnModel = null!;

        CurrentTable = null!;
    }

    private void OnClearTable()
    {
        SqlQuery = $"TRUNCATE TABLE {CurrentTable}; ";
        ExecuteQueryCommand.Execute();
        foreach (var columnModel in Data)
        {
            columnModel.Items = null!;
        }
    }

    private void OnDeleteSelectedColumn()
    {
        SqlQuery =
            $"ALTER TABLE {CurrentTable} DROP COLUMN {SelectedDataTableColumnModel.Header}; \nSELECT * FROM {CurrentTable};";
        ExecuteQueryCommand.Execute();
        SelectedDataTableColumnModel = null!;
    }

    private void OnSelectColumn(DataTableColumnModel obj)
    {
        if(obj.Items == null) return;
        foreach (var columnModel in Data)
        {
            foreach (var rowModel in columnModel.Items)
            {
                rowModel.IsSelected = false;
                if (rowModel.AttributeName == obj.Header)
                {
                    rowModel.IsSelected = true;
                    continue;
                }
            }
        }

        SelectedDataTableColumnModel = obj;
        SelectedDataTableRowModel = null!;
    }

    private void OnDeleteSelectedRow()
    {
        var itemFirstAttributeName = SelectedDataTableRowModel.Cells.FirstOrDefault().AttributeName;
        var attributeValue = SelectedDataTableRowModel.Cells.FirstOrDefault().Value;
        SqlQuery =
            $"DELETE FROM {CurrentTable} WHERE {itemFirstAttributeName} = {attributeValue}; \nSELECT * FROM {CurrentTable};";
        ExecuteQueryCommand.Execute();
        SelectedDataTableRowModel = null!;
    }

    private void OnSelectRow(DataTableCellModel obj)
    {
        var selectedRow = new DataTableRowModel();
        selectedRow.Index = obj.RowIndex;
        selectedRow.Cells = new ObservableCollection<DataTableCellModel>();
        foreach (var columnModel in Data)
        {
            foreach (var rowModel in columnModel.Items)
            {
                rowModel.IsSelected = false;
                if (rowModel.RowIndex == obj.RowIndex)
                {
                    rowModel.IsSelected = true;
                    selectedRow.Cells.Add(rowModel);
                    continue;
                }
            }
        }

        SelectedDataTableRowModel = selectedRow;
        SelectedDataTableColumnModel = null!;
    }

    private void OnAddColumn()
    {
        InsertItemPopupViewModel.Init();
        InsertItemPopupViewModel.IsShown = true;
    }

    private void OnAddRow()
    {
        InsertItemPopupViewModel.Init(Data);
        InsertItemPopupViewModel.IsShown = true;
    }

    public void Init(ConnectionModel connectionModel, SchemaModel schemaModel, string connectionName)
    {
        Init();
        SchemaModel = schemaModel;
        _connectionModel = connectionModel;
        Name = $"{connectionName} : {SchemaModel.SchemaName}";
    }

    private void InsertItemPopupViewModelOnInsertRequested(object? sender, IEnumerable<IInsertItemModel> e)
    {
        if (e is List<IInsertItemModel> columnItemModels)
        {
            var column = columnItemModels.OfType<ColumnItemModel>().FirstOrDefault();
            SqlQuery = $"ALTER TABLE {CurrentTable} ADD COLUMN {column.AttributeName} {column.DataType}" +
                       $"; \nSELECT * FROM {CurrentTable}";
        }
        else if (e is IEnumerable<RowCellModel> cells)
        {
            var queryFragment = "";
            foreach (var cell in cells)
            {
                switch (cell.DataType)
                {
                    case "TEXT" or "VARCHAR" or "DATE" or "TIMESTAMP" or "TIME":
                        queryFragment += $" '{cell.Value}',";
                        break;
                    case "INTEGER" or "NUMERIC" or "REAL" or "DECIMAL" or "BOOLEAN" or "MONEY":
                        if (string.IsNullOrEmpty(cell.Value))
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
            SqlQuery = $"INSERT INTO {CurrentTable} VALUES ({q})" + $"; \nSELECT * FROM {CurrentTable}";
        }


        ExecuteQueryCommand.Execute();
        InsertItemPopupViewModel.IsShown = false;
        InsertItemPopupViewModel.Column = null!;
        InsertItemPopupViewModel.Cells = null!;
    }
}