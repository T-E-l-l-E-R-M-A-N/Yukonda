namespace Yukonda.Core.ViewModels.Pages;

public interface IPage
{
    string Name { get; }
    PageType Type { get; }
    void Init();
}