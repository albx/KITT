using Microsoft.Extensions.DependencyInjection;
using Microsoft.FluentUI.AspNetCore.Components;

namespace KITT.Web.App.UI;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDefaultServices(this IServiceCollection services)
    {
        services.AddLocalization();
        services.AddFluentUIComponents(options =>
        {
            options.ValidateClassNames = false;
        });

        return services;
    }
}
