using CommunityToolkit.Mvvm.ComponentModel;
using Yukonda.ViewModels;

namespace Yukonda.Models;

public partial class DataTableColumnModel : ViewModelBase
{
    [ObservableProperty] private string _attribute;
    [ObservableProperty] private int _index;
    [ObservableProperty] private string _dataType;

    public DataTableColumnModel(string attribute, int index)
    {
        _attribute = attribute;
        _index = index;
    }
}