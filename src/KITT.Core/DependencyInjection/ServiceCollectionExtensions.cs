using FluentValidation;
using KITT.Core.Commands;
using KITT.Core.ReadModels;
using KITT.Core.Validators;
using Microsoft.Extensions.DependencyInjection;

namespace KITT.Core.DependencyInjection;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddKittCore()
        {
            services
                .AddValidatorsFromAssemblyContaining<StreamingValidator>()
                .AddScoped<IDatabase, Database>()
                .AddScoped<ISettingsCommands, SettingsCommands>()
                .AddScoped<IStreamingCommands, StreamingCommands>()
                .AddScoped<IProposalCommands, ProposalCommands>();

            return services;
        }
    }
    
}
