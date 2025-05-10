using KITT.Web.App.Endpoints.ReverseProxy;

namespace KITT.Web.App.Endpoints;

internal static class CmsEndpoints
{
    public static IEndpointRouteBuilder MapCmsEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapForwarder(
            "/api/cms/streamings", 
            "https+http://cms-api", 
            transformBuilder => transformBuilder.ConfigureWithTargetPath("/api/streamings"));

        builder.MapForwarder(
            "/api/cms/streamings/{id}", 
            "https+http://cms-api", 
            transformBuilder => transformBuilder.ConfigureWithTargetPath("/api/streamings/{id}"));

        builder.MapForwarder(
            "/api/cms/streamings/import", 
            "https+http://cms-api", 
            transformBuilder => transformBuilder.ConfigureWithTargetPath("/api/streamings/import"));

        builder.MapForwarder(
            "/api/cms/streamings/stats", 
            "https+http://cms-api", 
            transformBuilder => transformBuilder.ConfigureWithTargetPath("/api/streamings/stats"));

        return builder;
    }
}
