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

        return builder;
    }

    private static IEnumerable<string> GetScopes(IConfiguration configuration)
    {
        var scopes = configuration["CMS_API_SCOPES"]?.Split(",") ?? [];
        return scopes.Select(s => $"api/{configuration["CMS_APPID"]}/{s}");
    }
}
