﻿using LemonBot.Commands.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LemonBot.Commands.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBotCommands(this IServiceCollection services)
        {
            services.AddSingleton<CommandsProvider>();
            services.AddSingleton<BotCommandResolver>();
            services
                .AddSingleton<HelpCommand>()
                .AddSingleton<TodayCommand>()
                .AddSingleton<SayCommand>()
                .AddSingleton<ImageCommand>()
                .AddSingleton<GithubCommand>()
                .AddSingleton<SoCommand>();

            return services;
        }
    }
}
