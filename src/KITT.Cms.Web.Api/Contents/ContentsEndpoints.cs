using KITT.Cms.Web.Models.Contents;
using KITT.Web.Shared.Security;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
        ClaimsPrincipal user,
        Guid id,
        [FromBody] PublishContentModel model)
    {
        try
        {
            await services.PublishContentAsync(id, model, user.GetUserId());
            return TypedResults.NoContent();
        }
        catch (InvalidOperationException)
        {
            return TypedResults.NotFound();
        }
    }

    private static async Task<Results<NoContent, NotFound>> DeleteContent(
        ContentsEndpointsServices services,
        ClaimsPrincipal user,
        Guid id)
    {
        try
        {
            await services.DeleteContentAsync(id, user.GetUserId());
            return TypedResults.NoContent();
        }
        catch (InvalidOperationException)
        {
            return TypedResults.NotFound();
        }
    }
}
