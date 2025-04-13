using CommunityToolkit.Mvvm.ComponentModel;
using Yukonda.ViewModels;

namespace Yukonda.Models;

public partial class DataTableCellModel : ViewModelBase
{
    [ObservableProperty] private string _attribute;
    [ObservableProperty] private string _type;
    [ObservableProperty] private object _value;
    [ObservableProperty] private bool _isSelected;

    public DataTableCellModel(string attribute, string type, object value)
    {
        _attribute = attribute;
        _type = type;
        _value = value;
    }
}