using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Yukonda.ViewModels;

namespace Yukonda.Models;

public partial class SchemaModel : ViewModelBase
{
    [ObservableProperty] private string _name;
    [ObservableProperty] private bool _isReadOnly;
    [ObservableProperty] private ObservableCollection<DataTableModel> _tables;
    
    public SchemaModel(string name, bool isReadOnly, ObservableCollection<DataTableModel> tables)
    {
        _name = name;
        _isReadOnly = isReadOnly;
        _tables = tables;
    }
}