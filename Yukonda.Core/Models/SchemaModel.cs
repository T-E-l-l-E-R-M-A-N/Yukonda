using System.Collections.ObjectModel;

namespace Yukonda.Core.Models;

public class SchemaModel : BindableBase
{
    public string SchemaName { get; set; }
    public bool IsReadOnly { get; set; }
    public ObservableCollection<string> Tables { get; set; } = new();
}