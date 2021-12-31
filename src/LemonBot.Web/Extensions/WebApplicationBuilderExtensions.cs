using FluentValidation;
using KITT.Core.Commands;
using KITT.Core.Persistence;
using KITT.Core.ReadModels;
using KITT.Core.Validators;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;

namespace LemonBot.Web.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<KittDbContext>(
                options => options.UseSqlServer(builder.Configuration.GetConnectionString("KittDatabase")));

        builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        builder.Services.Configure<JwtBearerOptions>(
            JwtBearerDefaults.AuthenticationScheme,
            options =>
            {
                options.TokenValidationParameters.NameClaimType = "name";
            });

        builder.Services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAdB2C"));

        builder.Services
            .AddValidatorsFromAssemblyContaining<StreamingValidator>()
            .AddScoped<IDatabase, Database>()
            .AddScoped<ISettingsCommands, SettingsCommands>()
            .AddScoped<IStreamingCommands, StreamingCommands>();

        builder.Services
            .AddScoped<Areas.Console.Services.StreamingsControllerServices>()
            .AddScoped<Areas.Console.Services.SettingsControllerServices>();

        //builder.Services.AddScoped<AccountControllerbuilder.Services>();

        builder.Services.AddSignalR();
        builder.Services.AddControllersWithViews();

        return builder;
    }
}
