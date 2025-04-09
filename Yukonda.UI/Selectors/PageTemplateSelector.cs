using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Yukonda.Core.ViewModels.Pages;

namespace Yukonda.UI.Selectors
{
    internal class PageTemplateSelector : IDataTemplate
    {
        public Control? Build(object? param)
        {
            var resources = App.Current.Resources;
            var name = param.GetType().Name.Replace("ViewModel", "");
            object template = null!;
            resources.TryGetResource(name, Avalonia.Styling.ThemeVariant.Default, out template);
            if(template is IDataTemplate dt)
            {
                return dt.Build(param);
            }

            return null!;
        }

        public bool Match(object? data)
        {
            return data is IPage;
        }
    }
}
