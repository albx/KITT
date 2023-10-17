using KITT.Web.Models.Streamings;
using LemonBot.Web.Endpoints.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LemonBot.Web.Endpoints;

public static class StreamingsEndpoints
{
    public static IEndpointRouteBuilder MapStreamings(this IEndpointRouteBuilder builder)
    {
        var streamingsGroup = builder
            .MapGroup("api/console/streamings")
            .RequireAuthorization()
            .WithParameterValidation();

        streamingsGroup
            .MapGet("", GetAllStreamings)
            .WithName(nameof(GetAllStreamings));

        streamingsGroup
            .MapGet("{id:guid}", GetStreamingDetail)
            .WithName(nameof(GetStreamingDetail));

        streamingsGroup
            .MapPost("", ScheduleStreaming)
            .WithName(nameof(ScheduleStreaming));

        streamingsGroup
            .MapPost("import", ImportStreaming)
            .WithName(nameof(ImportStreaming));

        streamingsGroup
            .MapPut("{id:guid}", UpdateStreaming)
            .WithName(nameof(UpdateStreaming));

        streamingsGroup
            .MapDelete("{id:guid}", DeleteStreaming)
            .WithName(nameof(DeleteStreaming));

        return builder;
    }

    private static async Task<IResult> GetAllStreamings(
        StreamingsEndpointsServices services,
        ClaimsPrincipal user,
        int p = 1,
        int s = 10,
        StreamingQueryModel.SortDirection sort = StreamingQueryModel.SortDirection.Descending,
        string? q = null)
    {
        var userId = user.GetUserId();

        var streamings = await services.GetAllStreamingsAsync(
            userId,
            page: p,
            size: s,
            sort,
            query: q);

        return Results.Ok(streamings);
    }

    private static async Task<IResult> GetStreamingDetail(
        StreamingsEndpointsServices services,
        Guid id)
    {
        var streaming = await services.GetStreamingDetailAsync(id);
        if (streaming is null)
        {
            return Results.NotFound();
        }

        return Results.Ok(streaming);
    }

    private static async Task<IResult> ScheduleStreaming(
        StreamingsEndpointsServices services,
        ClaimsPrincipal user,
        [FromBody] ScheduleStreamingModel model)
    {
        var scheduledStreamingId = await services.ScheduleStreamingAsync(model, user.GetUserId());
        return Results.CreatedAtRoute(
            nameof(GetStreamingDetail),
            new { id = scheduledStreamingId },
            model);
    }

    private static async Task<IResult> ImportStreaming(
        StreamingsEndpointsServices services,
        ClaimsPrincipal user,
        [FromBody] ImportStreamingModel model)
    {
        var importedStreamingId = await services.ImportStreamingAsync(model, user.GetUserId());
        return Results.CreatedAtRoute(
            nameof(GetStreamingDetail),
            new { id = importedStreamingId },
            model);
    }

    private static async Task<IResult> UpdateStreaming(
        StreamingsEndpointsServices services,
        Guid id,
        [FromBody] StreamingDetailModel model)
    {
        if (id == Guid.Empty)
        {
            return Results.NotFound();
        }

        await services.UpdateStreamingAsync(id, model);
        return Results.Ok();
    }

    private static async Task<IResult> DeleteStreaming(
        StreamingsEndpointsServices services,
        Guid id)
    {
        await services.DeleteStreamingAsync(id);
        return Results.Ok();
    }
}
