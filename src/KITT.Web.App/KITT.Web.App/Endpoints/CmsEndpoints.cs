using KITT.Web.App.Endpoints.ReverseProxy;

namespace KITT.Web.App.Endpoints;

internal static class CmsEndpoints
{
    public static IEndpointRouteBuilder MapCmsEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapForwarder(
            "/api/cms/streamings", 
            "https+http://cms-api", 
            transformBuilder => transformBuilder.ConfigureWithTargetPath(
                "/api/streamings", 
                GetScopes))
            .RequireAuthorization();

        builder.MapForwarder(
            "/api/cms/streamings/{id}", 
            "https+http://cms-api", 
            transformBuilder => transformBuilder.ConfigureWithTargetPath(
                "/api/streamings/{id}",
                GetScopes))
            .RequireAuthorization();

        builder.MapForwarder(
            "/api/cms/streamings/import", 
            "https+http://cms-api", 
            transformBuilder => transformBuilder.ConfigureWithTargetPath(
                "/api/streamings/import",
                GetScopes))
            .RequireAuthorization();

        builder.MapForwarder(
            "/api/cms/streamings/stats", 
            "https+http://cms-api", 
            transformBuilder => transformBuilder.ConfigureWithTargetPath(
                "/api/streamings/stats", 
                GetScopes))
            .RequireAuthorization();

        builder.MapForwarder(
            "/api/cms/settings", 
            "https+http://cms-api", 
            transformBuilder => transformBuilder.ConfigureWithTargetPath(
                "/api/settings", 
                GetScopes))
            .RequireAuthorization();

        builder.MapForwarder(
            "/api/cms/settings/{id}",
            "https+http://cms-api",
            transformBuilder => transformBuilder.ConfigureWithTargetPath(
                "/api/settings/{id}",
                GetScopes))
            .RequireAuthorization();

        return builder;
    }

    private static IEnumerable<string> GetScopes(IConfiguration configuration)
        => [
            $"api://{configuration["CMS_APPID"]}/Cms.Read",
            $"api://{configuration["CMS_APPID"]}/Cms.Write",
        ];
}
