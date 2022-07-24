using FluentValidation;
using Hellang.Middleware.ProblemDetails;
using KITT.Core.DependencyInjection;
using KITT.Core.Persistence;
using LemonBot.Web.Areas.Tools.Services;
using LemonBot.Web.Configuration;
using LemonBot.Web.GraphQL;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using System.Text;

namespace LemonBot.Web.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<KittDbContext>(
                options => options.UseSqlServer(builder.Configuration.GetConnectionString("KittDatabase")));

        builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        builder.Services.Configure<BotConfiguration>(builder.Configuration.GetSection(nameof(BotConfiguration)));

        builder.Services.Configure<JwtBearerOptions>(
            JwtBearerDefaults.AuthenticationScheme,
            options =>
            {
                options.TokenValidationParameters.NameClaimType = "name";
            });

        builder.Services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAdB2C"));

        builder.Services.AddKittCore();

        builder.Services.AddHttpClient<IBotHttpClient, BotHttpClient>(c =>
        {
            c.BaseAddress = new Uri(builder.Configuration["BotConfiguration:Endpoint"]);

            var credentials = $"{builder.Configuration["BotConfiguration:Username"]}:{builder.Configuration["BotConfiguration:Password"]}";
            var authorizationValue = Convert.ToBase64String(Encoding.UTF8.GetBytes(credentials));
            c.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authorizationValue);
        });

        builder.Services.AddScoped<Areas.Tools.Services.StreamingsControllerServices>();

        builder.Services
            .AddScoped<Areas.Console.Services.StreamingsControllerServices>()
            .AddScoped<Areas.Console.Services.SettingsControllerServices>()
            .AddScoped<Areas.Console.Services.ProposalsControllerServices>();

        builder.Services.AddSignalR();
        builder.Services.AddControllersWithViews();

        builder.Services.AddKittGrahpQL();

        builder.Services.AddProblemDetails(options =>
        {
            options.Map<HttpRequestException>(ex =>
            {
                int statusCode = StatusCodes.Status503ServiceUnavailable;
                if (ex.StatusCode.HasValue)
                {
                    statusCode = (int)ex.StatusCode.Value;
                }

                return new StatusCodeProblemDetails(statusCode);
            });

            options.Map<ValidationException>(ex =>
            {
                var result = new StatusCodeProblemDetails(StatusCodes.Status400BadRequest);
                result.Extensions.Add("errors", ex.Errors.Select(e => new { Name = e.PropertyName, Message = e.ErrorMessage }));

                return result;
            });

            options.Map<ArgumentException>(ex =>
            {
                var result = new StatusCodeProblemDetails(StatusCodes.Status400BadRequest);
                result.Extensions.Add("errors", new[] { new { Name = ex.ParamName, Message = ex.Message } });

                return result;
            });

            options.Map<InvalidOperationException>(ex => new StatusCodeProblemDetails(StatusCodes.Status400BadRequest));
        });

        return builder;
    }
}
