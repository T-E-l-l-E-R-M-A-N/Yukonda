using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Yukonda.ViewModels;

namespace Yukonda.Models;

public partial class DataTableCellModel : ViewModelBase
{
    private ConnectionPageViewModel _connection;
    [ObservableProperty] private string _attribute;
    [ObservableProperty] private string _type;
    [ObservableProperty] private object _value;
    [ObservableProperty] private bool _isSelected;
    [ObservableProperty] private int _row;
    [ObservableProperty] private int _column;

    public DataTableCellModel(string attribute, string type, object value, ConnectionPageViewModel connection)
    {
        _attribute = attribute;
        _type = type;
        _value = value;
        _connection = connection;
    }
    
    [RelayCommand]
    public void SelectRow(int i)
    {
        _connection.SelectedRow = _connection.CurrentTable.Rows.FirstOrDefault(x => x.Index == i);
        foreach (var row in _connection.CurrentTable.Rows)
        {
            foreach (var cell in row.Cells)
            {
                cell.IsSelected = false;
                if (cell == this)
                    cell.IsSelected = true;
            }
        }
    }
}