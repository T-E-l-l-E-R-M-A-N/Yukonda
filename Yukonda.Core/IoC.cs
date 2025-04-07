using Microsoft.Extensions.DependencyInjection;
using Yukonda.Core.ViewModels.Windows;

namespace Yukonda.Core
{
    public static class IoC
    {
        private static ServiceProvider _provider;

        public static void RegisterServices()
        {
            var services = new ServiceCollection();

            services.AddScoped<MainWindowViewModel>();
            services.AddScoped<CreateConnectionWindowViewModel>();

            _provider = services.BuildServiceProvider();
        }
        public static T Resolve<T>() => _provider.GetRequiredService<T>();
    }
}