using KITT.Services;
using Microsoft.Extensions.Hosting;

namespace KITT.AppHost;

public static class DistributedApplicationBuilderExtensions
{
    extension(IDistributedApplicationBuilder builder)
    {
        public IResourceBuilder<IResourceWithConnectionString> AddKittDatabase()
        {
            ArgumentNullException.ThrowIfNull(builder);

            if (!builder.ExecutionContext.IsPublishMode)
            {
                var kittSql = builder.AddSqlServer(ServiceNames.Sql)
                    .WithDataVolume("kitt-data")
                    .WithDbGate(containerBuilder =>
                    {
                        containerBuilder.WithExplicitStart();
                    });

                return kittSql.AddDatabase(ServiceNames.Database, databaseName: "KITT");
            }
            else
            {
                return builder.AddConnectionString(ServiceNames.Database);
            }
        }
    }
    
}
