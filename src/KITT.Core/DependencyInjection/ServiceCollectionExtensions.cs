using FluentValidation;
using KITT.Core.Commands;
using KITT.Core.ReadModels;
using KITT.Core.Validators;
using Microsoft.Extensions.DependencyInjection;

namespace KITT.Core.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddKittCore(this IServiceCollection services)
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
