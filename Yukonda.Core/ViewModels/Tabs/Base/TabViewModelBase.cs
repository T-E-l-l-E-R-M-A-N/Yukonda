using Yukonda.Core.ViewModels.Pages;

namespace Yukonda.Core.ViewModels.Tabs;

public abstract class TabViewModelBase : BindableBase
{
    public string Name { get; set; }
    public PageViewModelBase Page { get; set; }
}