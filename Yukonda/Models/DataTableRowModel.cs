using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Yukonda.ViewModels;

namespace Yukonda.Models;

public partial class DataTableRowModel : ViewModelBase
{
    [ObservableProperty] private int _index;
    [ObservableProperty] private ObservableCollection<DataTableCellModel> _cells;

    public DataTableRowModel(int index, ObservableCollection<DataTableCellModel> cells)
    {
        _index = index;
        _cells = cells;
    }
}