using KITT.Cms.Web.Api.Endpoints.Services;
using KITT.Cms.Web.Models.Settings;
using KITT.Web.Shared.Security;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KITT.Cms.Web.Api.Endpoints;

public static class SettingsEndpoints
{
    extension(IEndpointRouteBuilder builder)
    {
        public IEndpointRouteBuilder MapSettingsEndpoints()
        {
            var settingsGroup = builder
                .MapGroup("api/settings")
                .RequireAuthorization();

            settingsGroup
                .MapGet("channels", GetChannels)
                .WithName(nameof(GetChannels));

            settingsGroup
                .MapPost("channels", CreateChannel)
                .WithName(nameof(CreateChannel));

            settingsGroup
                .MapGet("channels/{id:guid}", GetChannelDetail)
                .WithName(nameof(GetChannelDetail));

            settingsGroup
                .MapPut("channels/{id:guid}", UpdateChannel)
                .WithName(nameof(UpdateChannel));

            settingsGroup
                .MapDelete("channels/{id}", DeleteChannel)
                .WithName(nameof(DeleteChannel));

            return builder;
        }
    }

    private static async Task<Ok<ChannelModel[]>> GetChannels(
        ChannelsEndpointsServices services,
        ClaimsPrincipal user)
    {
        var userId = user.GetUserId();
        var channels = await services.GetChannelsAsync(userId);

        return TypedResults.Ok(channels);
    }

    private static async Task<Results<CreatedAtRoute<ChannelModel>, BadRequest, ValidationProblem>> CreateChannel(
        ChannelsEndpointsServices services,
        ClaimsPrincipal user,
        [FromBody] ChannelModel model)
    {
        var userId = user.GetUserId();
        var createdChannel = await services.CreateChannelAsync(model, userId);

        return TypedResults.CreatedAtRoute(createdChannel, nameof(GetChannelDetail), new { id = createdChannel.Id });
    }

    private static async Task<Results<Ok<ChannelModel>, NotFound>> GetChannelDetail(
        ChannelsEndpointsServices services,
        ClaimsPrincipal user,
        Guid id)
    {
        var userId = user.GetUserId();
        var channel = await services.GetChannelAsync(id, userId);
        if (channel is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(channel);
    }

    private static async Task<Results<NoContent, NotFound, BadRequest, ValidationProblem>> UpdateChannel(
        ChannelsEndpointsServices services,
        ClaimsPrincipal user,
        Guid id,
        [FromBody] ChannelModel model)
    {
        var userId = user.GetUserId();

        try
        {
            await services.UpdateChannelAsync(id, model, userId);
            return TypedResults.NoContent();
        }
        catch (ArgumentOutOfRangeException)
        {
            return TypedResults.NotFound();
        }
    }

    private static async Task<Results<NoContent, NotFound>> DeleteChannel(
        ChannelsEndpointsServices services,
        ClaimsPrincipal user,
        Guid id)
    {
        var userId = user.GetUserId();
        await services.DeleteChannelAsync(id, userId);

        return TypedResults.NoContent();
    }
}
