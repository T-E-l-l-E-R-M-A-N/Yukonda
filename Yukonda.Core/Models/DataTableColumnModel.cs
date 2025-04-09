using System.Collections.ObjectModel;

namespace Yukonda.Core.Models;

public class DataTableColumnModel : BindableBase
{
    public string Header { get; set; }
    public string DataType { get; set; }
    public ObservableCollection<DataTableCellModel> Items { get; set; }
    public bool IsSelected { get; set; }
    
}

public class DataTableCellModel : BindableBase
{
    public string AttributeName { get; set; }
    public object Value { get; set; }
    public int RowIndex { get; set; }
    public bool IsSelected { get; set; }
}

public class DataTableRowModel : BindableBase
{
    public int Index { get; set; }
    public ObservableCollection<DataTableCellModel> Cells { get; set; }
}

