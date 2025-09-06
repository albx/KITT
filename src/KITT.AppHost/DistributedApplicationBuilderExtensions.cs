using KITT.Services;
using Microsoft.Extensions.Hosting;

namespace KITT.AppHost;

public static class DistributedApplicationBuilderExtensions
{
    public static IResourceBuilder<IResourceWithConnectionString> AddKittDatabase(this IDistributedApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (builder.Environment.IsDevelopment())
        {
            var kittSql = builder.AddSqlServer(ServiceNames.Sql)
                .WithContainerName("sqlserver-local")
                .WithLifetime(ContainerLifetime.Persistent)
                .WithDataVolume("kitt-data");

            return kittSql.AddDatabase(ServiceNames.Database, databaseName: "KITT");
        }
        else
        {
            return builder.AddConnectionString(ServiceNames.Database);
        }
    }
}
