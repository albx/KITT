using KITT.Web.Models.Dashboard;
using LemonBot.Web.Endpoints.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Security.Claims;

namespace LemonBot.Web.Endpoints;

public static class DashboardEndpoints
{
    public static IEndpointRouteBuilder MapDashboardEndpoints(this IEndpointRouteBuilder builder)
    {
        var dashboardGroup = builder
            .MapGroup("api/dashboard")
            .RequireAuthorization();

        dashboardGroup
            .MapGet("/", GetDashboard);

        return builder;
    }

    private static async Task<Ok<DashboardModel>> GetDashboard(
        DashboardEndpointsServices services,
        ClaimsPrincipal user)
    {
        var userId = user.GetUserId();

        var model = await services.GetDashboardAsync(userId);
        return TypedResults.Ok(model);
    }
}
