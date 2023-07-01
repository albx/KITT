using KITT.Telegram.Functions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((context, services) =>
    {
        services.Configure<TelegramConfigurationOptions>(
            option => option.ChatId = context.Configuration["TelegramChatId"]);

        services.AddSingleton<TelegramBotClient>(
            sp => new TelegramBotClient(context.Configuration["TelegramBotToken"]));
    })
    .Build();

host.Run();
