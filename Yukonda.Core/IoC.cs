using Microsoft.Extensions.DependencyInjection;
using Yukonda.Core.ViewModels.Controls;
using Yukonda.Core.ViewModels.Pages;
using Yukonda.Core.ViewModels.Tabs;
using Yukonda.Core.ViewModels.Windows;

namespace Yukonda.Core
{
    public static class IoC
    {
        private static ServiceProvider _provider;

        public static void RegisterServices()
        {
            var services = new ServiceCollection();

            services.AddTransient<IPage, ConnectionPageViewModel>();
            services.AddTransient<IPage, SchemaPageViewModel>();
            services.AddTransient<TabViewModelBase, SimpleTabViewModel>();

            services.AddScoped<InsertItemPopupViewModel>();
            services.AddScoped<MainWindowViewModel>();
            services.AddScoped<CreateConnectionWindowViewModel>();

            _provider = services.BuildServiceProvider();
        }
        public static T Resolve<T>() => _provider.GetRequiredService<T>();
    }
}