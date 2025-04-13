using CommunityToolkit.Mvvm.ComponentModel;
using Yukonda.ViewModels;

namespace Yukonda.Models;

public partial class ProviderModel : ViewModelBase
{
    [ObservableProperty] private string _providerName;

    public ProviderModel(string providerName)
    {
        _providerName = providerName;
    }
}