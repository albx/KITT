using KITT.Web.App.Endpoints.ReverseProxy;

namespace KITT.Web.App.Endpoints;

internal static class ProposalEndpoints
{
    public static IEndpointRouteBuilder MapProposalEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapForwarder(
            "/api/proposals", 
            "https+http://proposals-api", 
            transformBuilder => transformBuilder.ConfigureWithTargetPath(
                "/api/proposals", 
                GetScopes))
            .RequireAuthorization();

        builder.MapForwarder(
            "/api/proposals/{id}", 
            "https+http://proposals-api", 
            transformBuilder => transformBuilder.ConfigureWithTargetPath(
                "/api/proposals/{id}", 
                GetScopes))
            .RequireAuthorization();

        builder.MapForwarder(
            "/api/proposals/{id}/refuse", 
            "https+http://proposals-api", 
            transformBuilder => transformBuilder.ConfigureWithTargetPath(
                "/api/proposals/{id}/refuse", 
                GetScopes))
            .RequireAuthorization();
        
        builder.MapForwarder(
            "/api/proposals/stats", 
            "https+http://proposals-api", 
            transformBuilder => transformBuilder.ConfigureWithTargetPath(
                "/api/proposals/stats", 
                GetScopes))
            .RequireAuthorization();

        return builder;
    }

    private static IEnumerable<string> GetScopes(IConfiguration configuration)
    {
        var scopes = configuration["PROPOSALS_API_SCOPES"]?.Split(",") ?? [];
        return scopes.Select(s => $"api://{configuration["PROPOSALS_APPID"]}/{s}");
    }
}
