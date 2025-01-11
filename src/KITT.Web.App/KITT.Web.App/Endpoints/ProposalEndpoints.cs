namespace KITT.Web.App.Endpoints;

internal static class ProposalEndpoints
{
    public static IEndpointRouteBuilder MapProposalEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapForwarder("/api/proposals", "https+http://proposals-api", "/api/proposals");
        builder.MapForwarder("/api/proposals/{id}", "https+http://proposals-api", "/api/proposals/{id}");
        builder.MapForwarder("/api/proposals/{id}/refuse", "https+http://proposals-api", "/api/proposals/{id}/refuse");
        builder.MapForwarder("/api/proposals/stats", "https+http://proposals-api", "/api/proposals/stats");

        return builder;
    }
}
