using HotChocolate.AspNetCore;

namespace LemonBot.Web.GraphQL;

public static class WebApplicationExtensions
{
    public static WebApplication MapKittGraphQL(this WebApplication app, IWebHostEnvironment env)
    {
        var isDevelopment = env.IsDevelopment();

        var options = new GraphQLServerOptions { EnableSchemaRequests = isDevelopment };
        options.Tool.Enable = isDevelopment;

        app.MapGraphQL().WithOptions(options);

        return app;
    }
}
