using KITT.Cms.Web.Models.Contents;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace KITT.Cms.Web.Api.Contents;

public static class ContentsEndpoints
{
    extension(IEndpointRouteBuilder builder)
    {
        public IEndpointRouteBuilder MapContentsEndpoints()
        {
            var contentsGroup = builder
                .MapGroup("api/contents")
                .RequireAuthorization();

            contentsGroup
                .MapPatch("{id:guid}/publish", PublishContent)
                .WithName(nameof(PublishContent));

            contentsGroup
                .MapDelete("{id:guid}", DeleteContent)
                .WithName(nameof(DeleteContent));

            return builder;
        }
    }

    private static async Task<Results<NoContent, NotFound>> PublishContent(
        ContentsEndpointsServices services,
        Guid id,
        [FromBody] PublishContentModel model)
    {
        try
        {
            await services.PublishContentAsync(id, model);
            return TypedResults.NoContent();
        }
        catch (InvalidOperationException)
        {
            return TypedResults.NotFound();
        }
    }

    private static async Task<Results<NoContent, NotFound>> DeleteContent(
        ContentsEndpointsServices services,
        Guid id)
    {
        try
        {
            await services.DeleteContentAsync(id);
            return TypedResults.NoContent();
        }
        catch (InvalidOperationException)
        {
            return TypedResults.NotFound();
        }
    }
}
