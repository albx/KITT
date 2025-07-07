using Azure;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;

namespace KITT.Support.Seeder;

public class Worker : BackgroundService
{
    public const string ActivitySourceName = "KITT.Support.Seeder";

    private readonly ILogger<Worker> _logger;
    private readonly SecretClient _secretClient;
    private readonly IConfiguration _configuration;
    private readonly IHostApplicationLifetime _hostApplicationLifetime;

    private static readonly ActivitySource _activitySource = new(ActivitySourceName);

    public Worker(
        ILogger<Worker> logger, 
        SecretClient secretClient,
        IConfiguration configuration,
        IHostApplicationLifetime hostApplicationLifetime)
    {
        _logger = logger;
        _secretClient = secretClient;
        _configuration = configuration;
        _hostApplicationLifetime = hostApplicationLifetime;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await SetupSecretsAsync(stoppingToken);

        _hostApplicationLifetime.StopApplication();
    }

    private async Task SetupSecretsAsync(CancellationToken stoppingToken)
    {
        using var activity = _activitySource.StartActivity("Setup secrets");

        try
        {
            if (await ExistsSecretAsync("Identity__WebApp__AppSecret", stoppingToken))
            {
                return;
            }

            await _secretClient.SetSecretAsync(
                "Identity__WebApp__AppSecret",
                _configuration["Identity:WebApp:AppSecret"], 
                cancellationToken: stoppingToken);
        }
        catch (Exception ex)
        {
            activity?.AddException(ex);
            throw;
        }
        
    }

    private async Task<bool> ExistsSecretAsync(string secretName, CancellationToken cancellationToken)
    {
        try
        {
            var webAppSecretResponse = await _secretClient.GetSecretAsync("Identity__WebApp__AppSecret");
            if (webAppSecretResponse.Value is not null)
            {
                _logger.LogInformation("WebApp secret already set");
                return true;
            }

            return false;
        }
        catch (RequestFailedException ex) when (ex.Status == StatusCodes.Status404NotFound)
        {
            _logger.LogWarning(ex, "Secret not found");
            return false;
        }
    }
}
