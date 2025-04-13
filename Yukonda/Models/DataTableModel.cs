using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Yukonda.ViewModels;

namespace Yukonda.Models;

public partial class DataTableModel : ViewModelBase
{
    [ObservableProperty] private string _name;
    [ObservableProperty] private SchemaModel _schema;
    [ObservableProperty] private ObservableCollection<DataTableRowModel> _rows;
    [ObservableProperty] private ObservableCollection<DataTableColumnModel> _columns;

    public DataTableModel(string name, SchemaModel schema, ObservableCollection<DataTableRowModel> rows, ObservableCollection<DataTableColumnModel> columns)
    {
        _name = name;
        _schema = schema;
        _rows = rows;
        _columns = columns;
    }
}