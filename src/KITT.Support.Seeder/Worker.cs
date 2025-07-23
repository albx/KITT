using Azure;
using Azure.Security.KeyVault.Secrets;
using KITT.Core.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using System.Diagnostics;

namespace KITT.Support.Seeder;

public class Worker : BackgroundService
{
    public const string ActivitySourceName = "KITT.Support.Seeder";

    private readonly ILogger<Worker> _logger;
    private readonly IConfiguration _configuration;
    private readonly IHostApplicationLifetime _hostApplicationLifetime;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    private static readonly ActivitySource _activitySource = new(ActivitySourceName);

    public Worker(
        ILogger<Worker> logger, 
        IConfiguration configuration,
        IHostApplicationLifetime hostApplicationLifetime,
        IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _configuration = configuration;
        _hostApplicationLifetime = hostApplicationLifetime;
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        //await SetupSecretsAsync(stoppingToken);
        await SetupDatabaseAsync(stoppingToken);

        _hostApplicationLifetime.StopApplication();
    }

    private async Task SetupDatabaseAsync(CancellationToken stoppingToken)
    {
        using var activity = _activitySource.StartActivity("Setup database");

        using var scope = _serviceScopeFactory.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<KittDbContext>();
        await EnsureDatabaseIsCreatedAsync(context, stoppingToken);

        _logger.LogInformation("Applying all pending migrations...");
        await context.Database.MigrateAsync(stoppingToken).ConfigureAwait(false);
        _logger.LogInformation("Migrations applied correctly!");

        await CreateSecurityCacheAsync(context, stoppingToken);
    }

    private async Task CreateSecurityCacheAsync(KittDbContext context, CancellationToken stoppingToken)
    {
        try
        {
            var securityCacheSqlString = """
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'SecurityCache')
                BEGIN
                    CREATE TABLE [dbo].[SecurityCache] (
                        [Id]                         NVARCHAR (449)     COLLATE SQL_Latin1_General_CP1_CS_AS NOT NULL,
                        [Value]                      VARBINARY (MAX)    NOT NULL,
                        [ExpiresAtTime]              DATETIMEOFFSET (7) NOT NULL,
                        [SlidingExpirationInSeconds] BIGINT             NULL,
                        [AbsoluteExpiration]         DATETIMEOFFSET (7) NULL,
                        PRIMARY KEY CLUSTERED ([Id] ASC)
                    );
                END
                """;

            _logger.LogInformation("Creating security cache table");
            await context.Database.ExecuteSqlRawAsync(securityCacheSqlString).ConfigureAwait(false);
            _logger.LogInformation("Security cache table created successfully");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error creating SecurityCache table: {ErrorMessage}", ex.Message);
            return;
        }

        try
        {
            var expiresAtTimeIndexSqlString = """
                IF NOT EXISTS (
                    SELECT 1
                    FROM sys.indexes
                    WHERE name = 'Index_ExpiresAtTime'
                      AND object_id = OBJECT_ID('dbo.SecurityCache')
                )
                BEGIN
                    CREATE NONCLUSTERED INDEX [Index_ExpiresAtTime]
                        ON [dbo].[SecurityCache]([ExpiresAtTime] ASC);
                END
                """;

            _logger.LogInformation("Creating index on ExpiresAtTime table");
            await context.Database.ExecuteSqlRawAsync(expiresAtTimeIndexSqlString).ConfigureAwait(false);
            _logger.LogInformation("Index created successfully");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error creating index on ExpiresAtTime column: {ErrorMessage}", ex.Message);
        }
    }

    private async Task EnsureDatabaseIsCreatedAsync(KittDbContext context, CancellationToken stoppingToken)
    {
        var creator = context.GetService<IRelationalDatabaseCreator>();
        if (await creator.ExistsAsync(stoppingToken))
        {
            _logger.LogInformation("Database already exist!");
            return;
        }

        _logger.LogInformation("Creating database...");
        await creator.CreateAsync(stoppingToken);
        _logger.LogInformation("Database created successfully.");
    }

    //private async Task SetupSecretsAsync(CancellationToken stoppingToken)
    //{
    //    using var activity = _activitySource.StartActivity("Setup secrets");

    //    try
    //    {
    //        if (await ExistsSecretAsync("Identity__WebApp__AppSecret", stoppingToken))
    //        {
    //            return;
    //        }

    //        await _secretClient.SetSecretAsync(
    //            "Identity__WebApp__AppSecret",
    //            _configuration["Identity:WebApp:AppSecret"], 
    //            cancellationToken: stoppingToken);
    //    }
    //    catch (Exception ex)
    //    {
    //        activity?.AddException(ex);
    //        throw;
    //    }
        
    //}

    //private async Task<bool> ExistsSecretAsync(string secretName, CancellationToken cancellationToken)
    //{
    //    try
    //    {
    //        var webAppSecretResponse = await _secretClient.GetSecretAsync("Identity__WebApp__AppSecret");
    //        if (webAppSecretResponse.Value is not null)
    //        {
    //            _logger.LogInformation("WebApp secret already set");
    //            return true;
    //        }

    //        return false;
    //    }
    //    catch (RequestFailedException ex) when (ex.Status == StatusCodes.Status404NotFound)
    //    {
    //        _logger.LogWarning(ex, "Secret not found");
    //        return false;
    //    }
    //}
}
