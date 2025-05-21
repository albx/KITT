using KITT.Web.App.Endpoints.ReverseProxy;

namespace KITT.Web.App.Endpoints;

internal static class ProposalEndpoints
{
    public static IEndpointRouteBuilder MapProposalEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapForwarder(
            "/api/proposals", 
            "https+http://proposals-api", 
            transformBuilder => transformBuilder.ConfigureWithTargetPath("/api/proposals", _ => []));

        builder.MapForwarder(
            "/api/proposals/{id}", 
            "https+http://proposals-api", 
            transformBuilder => transformBuilder.ConfigureWithTargetPath("/api/proposals/{id}", _ => []));

        builder.MapForwarder(
            "/api/proposals/{id}/refuse", 
            "https+http://proposals-api", 
            transformBuilder => transformBuilder.ConfigureWithTargetPath("/api/proposals/{id}/refuse", _ => []));
        
        builder.MapForwarder(
            "/api/proposals/stats", 
            "https+http://proposals-api", 
            transformBuilder => transformBuilder.ConfigureWithTargetPath("/api/proposals/stats", _ => []));

        return builder;
    }
}
