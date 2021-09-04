using KITT.Auth.Persistence;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace KITT.Auth
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAuthDataInitializer(this IServiceCollection services, Action<DataInitializer.DataInitializerOptions> configureOptions)
        {
            services.Configure(configureOptions);
            services.AddScoped<DataInitializer>();

            return services;
        }
    }
}
