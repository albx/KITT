using KITT.Cms.Web.Api.Endpoints.Services;
using KITT.Cms.Web.Models.Settings;
using KITT.Web.Shared.Security;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KITT.Cms.Web.Api.Endpoints;

public static class SettingsEndpoints
{
    public static IEndpointRouteBuilder MapSettingsEndpoints(this IEndpointRouteBuilder builder)
    {
        var settingsGroup = builder
            .MapGroup("api/settings")
            .RequireAuthorization();

        return builder;
    }
}
