using KITT.Web.App.Endpoints.Services;
using KITT.Web.Models.Settings;
using KITT.Web.Shared.Security;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KITT.Web.App.Endpoints;

internal static class SettingsEndpoints
{
    public static IEndpointRouteBuilder MapSettingsEndpoints(this IEndpointRouteBuilder builder)
    {
        var settingsGroup = builder
            .MapGroup("api/settings")
            //.RequireAuthorization()
            .WithParameterValidation();

        settingsGroup
            .MapGet("", GetAllSettings)
            .WithName(nameof(GetAllSettings))
            .WithOpenApi();

        settingsGroup
            .MapGet("{id}", GetSettingsDetails)
            .WithName(nameof(GetSettingsDetails))
            .WithOpenApi();

        settingsGroup
            .MapPost("", CreateNewSettings)
            .WithName(nameof(CreateNewSettings))
            .WithOpenApi();

        return builder;
    }

    private static async Task<Ok<SettingsListModel>> GetAllSettings(
       SettingsEndpointsServices services,
       ClaimsPrincipal user)
    {
        var model = await services.GetAllSettingsAsync(user.GetUserId());
        return TypedResults.Ok(model);
    }

    private static async Task<Results<Ok<SettingsDetailModel>, NotFound>> GetSettingsDetails(
        SettingsEndpointsServices services,
        Guid id)
    {
        var settings = await services.GetSettingsDetailAsync(id);
        if (settings is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(settings);
    }

    private static async Task<Results<CreatedAtRoute<CreateNewSettingsModel>, BadRequest, ValidationProblem>> CreateNewSettings(
        SettingsEndpointsServices services,
        ClaimsPrincipal user,
        [FromBody] CreateNewSettingsModel model)
    {
        var settingsId = await services.CreateNewSettingsAsync(model, user.GetUserId());
        return TypedResults.CreatedAtRoute(
            model,
            nameof(GetSettingsDetails),
            new { id = settingsId });
    }
}
