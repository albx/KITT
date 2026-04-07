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
            services.AddFluentUIComponents(options =>
            {
                options.ValidateClassNames = false;
            });

            return services;
        }
    }
}
