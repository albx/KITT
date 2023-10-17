using KITT.Web.Models.Streamings;
using LemonBot.Web.Endpoints.Services;
using Microsoft.AspNetCore.Http.HttpResults;
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
            .WithName(nameof(GetAllStreamings))
            .WithOpenApi();

        streamingsGroup
            .MapGet("{id:guid}", GetStreamingDetail)
            .WithName(nameof(GetStreamingDetail))
            .WithOpenApi();

        streamingsGroup
            .MapPost("", ScheduleStreaming)
            .WithName(nameof(ScheduleStreaming))
            .WithOpenApi();

        streamingsGroup
            .MapPost("import", ImportStreaming)
            .WithName(nameof(ImportStreaming))
            .WithOpenApi();

        streamingsGroup
            .MapPut("{id:guid}", UpdateStreaming)
            .WithName(nameof(UpdateStreaming))
            .WithOpenApi();

        streamingsGroup
            .MapDelete("{id:guid}", DeleteStreaming)
            .WithName(nameof(DeleteStreaming))
            .WithOpenApi();

        return builder;
    }

    private static async Task<Ok<StreamingsListModel>> GetAllStreamings(
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

        return TypedResults.Ok(streamings);
    }

    private static async Task<Results<Ok<StreamingDetailModel>, NotFound>> GetStreamingDetail(
        StreamingsEndpointsServices services,
        Guid id)
    {
        var streaming = await services.GetStreamingDetailAsync(id);
        if (streaming is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(streaming);
    }

    private static async Task<Results<CreatedAtRoute<ScheduleStreamingModel>, BadRequest, ValidationProblem>> ScheduleStreaming(
        StreamingsEndpointsServices services,
        ClaimsPrincipal user,
        [FromBody] ScheduleStreamingModel model)
    {
        var scheduledStreamingId = await services.ScheduleStreamingAsync(model, user.GetUserId());
        return TypedResults.CreatedAtRoute(
            model,
            nameof(GetStreamingDetail),
            new { id = scheduledStreamingId });
    }

    private static async Task<Results<CreatedAtRoute<ImportStreamingModel>, BadRequest, ValidationProblem>> ImportStreaming(
        StreamingsEndpointsServices services,
        ClaimsPrincipal user,
        [FromBody] ImportStreamingModel model)
    {
        var importedStreamingId = await services.ImportStreamingAsync(model, user.GetUserId());
        return TypedResults.CreatedAtRoute(
            model,
            nameof(GetStreamingDetail),
            new { id = importedStreamingId });
    }

    private static async Task<Results<NoContent, NotFound, BadRequest, ValidationProblem>> UpdateStreaming(
        StreamingsEndpointsServices services,
        Guid id,
        [FromBody] StreamingDetailModel model)
    {
        if (id == Guid.Empty)
        {
            return TypedResults.NotFound();
        }

        await services.UpdateStreamingAsync(id, model);
        return TypedResults.NoContent();
    }

    private static async Task<Results<NoContent, NotFound, BadRequest, ValidationProblem>> DeleteStreaming(
        StreamingsEndpointsServices services,
        Guid id)
    {
        if (id == Guid.Empty)
        {
            return TypedResults.NotFound();
        }

        await services.DeleteStreamingAsync(id);
        return TypedResults.NoContent();
    }
}
