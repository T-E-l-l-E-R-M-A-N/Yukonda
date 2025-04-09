using System.Collections.ObjectModel;
using Yukonda.Core.Models;

namespace Yukonda.Core.ViewModels.Controls;


public class InsertItemPopupViewModel : BindableBase
{
    public ItemType InsertType { get; set; }
    public ObservableCollection<RowCellModel> Cells { get; set; }
    public ColumnItemModel Column { get; set; }
    public List<string> DataTypes { get; set; }
    public event EventHandler<IEnumerable<IInsertItemModel>> InsertRequested;
    public bool IsShown { get; set; }
    public InsertItemPopupViewModel()
    {
        DataTypes = new List<string>(new string[] { "SERIAL", "NUMERIC", "DECIMAL", "REAL", "INTEGER", "MONEY", "VARCHAR (255)", "BOOLEAN", "DATE", "TIMESTAMP", "TIME", "CHAR (255)", "TEXT" });
    }
    
    public void Init(IEnumerable<DataTableColumnModel> table)
    {
        InsertType = ItemType.Row;
        Cells = new ObservableCollection<RowCellModel>();
        
        InsertCommand = new DelegateCommand(OnInsert);
        CancelInsertingCommand = new DelegateCommand(OnCancelInserting);
        foreach (var dataTableColumnModel in table)
        {
            
            Cells.Add(new RowCellModel(dataTableColumnModel.Header, dataTableColumnModel.DataType)
            {
                
            });
        }
    }

    public void Init()
    {
        InsertType = ItemType.Column;
        InsertCommand = new DelegateCommand(OnInsert);
        CancelInsertingCommand = new DelegateCommand(OnCancelInserting);
        Column = new ColumnItemModel();
    }

    private void OnCancelInserting()
    {
        Cells = null!;
        Column = null!;
        IsShown = false;
    }

    private void OnInsert()
    {
        if (InsertType == ItemType.Column)
        {
            InsertRequested?.Invoke(this, new List<IInsertItemModel>() {Column});
        }
        else if (InsertType == ItemType.Row)
        {
            InsertRequested?.Invoke(this, Cells);
        }
    }

    public DelegateCommand InsertCommand { get; set; }
    public DelegateCommand CancelInsertingCommand { get; set; }
}

public enum ItemType
{
    Column,
    Row
}

public interface IInsertItemModel
{
    public string AttributeName { get; }
}
public class RowCellModel : BindableBase, IInsertItemModel
{
    public string AttributeName { get; }
    public string Value { get; set; }
    public string DataType { get;}

    public RowCellModel(string name, string dataType)
    {
        AttributeName = name;
        DataType = dataType;
    }
}

public class ColumnItemModel : BindableBase, IInsertItemModel
{
    public string AttributeName { get; set; }
    public string DataType { get; set; }
    
    public string Constraint { get; set; }
}
