using LemonBot.Clients.Configurations;
using LemonBot.Clients.Extensions;
using LemonBot.Commands.Extensions;
using LemonBot.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder()
    .ConfigureServices((context, services) =>
    {
        var configuration = context.Configuration;

        services.Configure<TwitchBotOptions>(configuration.GetSection("TwitchBot"));

        services.AddTwitchClient();
        services.AddBotCommands();
        services.AddHostedService<TwitchBotService>();
    })
    .Build();

await host.RunAsync();