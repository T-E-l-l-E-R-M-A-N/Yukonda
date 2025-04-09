namespace Yukonda.Core.ViewModels.Pages;

public abstract class PageViewModelBase : BindableBase, IPage
{
    public string Name { get; set; }
    public PageType Type { get; set; }
    protected PageViewModelBase(string name, PageType type)
    {
        Name = name;
        Type = type;
    }
    public abstract void Init();
}