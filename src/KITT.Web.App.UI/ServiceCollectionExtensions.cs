using Microsoft.Extensions.DependencyInjection;
using Microsoft.FluentUI.AspNetCore.Components;

namespace KITT.Web.App.UI;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddDefaultServices()
        {
            services.AddLocalization();
            services.AddFluentUIComponents(config =>
            {
                config.DefaultValues.For<FluentStack>().Set(p => p.HorizontalGap, "10px");
                config.DefaultValues.For<FluentStack>().Set(p => p.VerticalGap, "10px");
                config.Toast.Position = ToastPosition.TopCenter;
            });

            return services;
        }
    }
}
