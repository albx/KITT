using KITT.Cms.Web.Models.BlogPosts;
using KITT.Web.Shared.Security;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KITT.Cms.Web.Api.BlogPosts;

public static class BlogPostsEndpoints
{
    extension(IEndpointRouteBuilder builder)
    {
        public IEndpointRouteBuilder MapBlogPostsEndpoints()
        {
            var blogPostsGroup = builder
                .MapGroup("api/blogposts")
                .RequireAuthorization();

            blogPostsGroup
                .MapGet("", GetAllBlogPosts)
                .WithName(nameof(GetAllBlogPosts));

            blogPostsGroup
                .MapGet("{id:guid}", GetBlogPostDetail)
                .WithName(nameof(GetBlogPostDetail));

            blogPostsGroup
                .MapPost("", PublishBlogPost)
                .WithName(nameof(PublishBlogPost));

            blogPostsGroup
                .MapPost("draft", CreateDraftBlogPost)
                .WithName(nameof(CreateDraftBlogPost));

            blogPostsGroup
                .MapPost("import", ImportBlogPost)
                .WithName(nameof(ImportBlogPost));

            blogPostsGroup
                .MapPut("{id:guid}", UpdateBlogPost)
                .WithName(nameof(UpdateBlogPost));

            return builder;
        }
    }

    private static async Task<Ok<BlogPostListModel>> GetAllBlogPosts(
        BlogPostsEndpointsServices services,
        ClaimsPrincipal user,
        [AsParameters] BlogPostQueryParameters query)
    {
        var userId = user.GetUserId();

        var model = await services.GetAllBlogPostsAsync(query, userId);
        return TypedResults.Ok(model);
    }

    private static async Task<Results<Ok<BlogPostDetailModel>, NotFound>> GetBlogPostDetail(
        BlogPostsEndpointsServices services,
        ClaimsPrincipal user,
        Guid id)
    {
        var userId = user.GetUserId();
        var model = await services.GetBlogPostDetailAsync(id, userId);
        if (model is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(model);
    }

    private static async Task<Results<CreatedAtRoute<BlogPostDetailModel>, BadRequest, ValidationProblem>> PublishBlogPost(
        BlogPostsEndpointsServices services,
        ClaimsPrincipal user,
        [FromBody] PublishBlogPostModel model)
    {
        var userId = user.GetUserId();
        var postId = await services.PublishBlogPostAsync(model, userId);

        return TypedResults.CreatedAtRoute(model.ToDetailModel(postId), nameof(GetBlogPostDetail), new { id = postId });
    }

    private static async Task<Results<CreatedAtRoute<BlogPostDetailModel>, BadRequest, ValidationProblem>> CreateDraftBlogPost(
        BlogPostsEndpointsServices services,
        ClaimsPrincipal user,
        [FromBody] DraftBlogPostModel model)
    {
        var userId = user.GetUserId();
        var postId = await services.CreateDraftBlogPostAsync(model, userId);

        return TypedResults.CreatedAtRoute(model.ToDetailModel(postId), nameof(GetBlogPostDetail), new { id = postId });
    }

    private static async Task<Results<CreatedAtRoute<BlogPostDetailModel>, BadRequest, ValidationProblem>> ImportBlogPost(
        BlogPostsEndpointsServices services,
        ClaimsPrincipal user,
        [FromBody] ImportBlogPostModel model)
    {
        var userId = user.GetUserId();
        var postId = await services.ImportBlogPostAsync(model, userId);

        return TypedResults.CreatedAtRoute(model.ToDetailModel(postId), nameof(GetBlogPostDetail), new { id = postId });
    }

    private static async Task<Results<NoContent, NotFound, BadRequest, ValidationProblem>> UpdateBlogPost(
        BlogPostsEndpointsServices services,
        Guid id,
        [FromBody] UpdateBlogPostModel model)
    {
        try
        {
            await services.UpdateBlogPostAsync(id, model);
            return TypedResults.NoContent();
        }
        catch (InvalidOperationException)
        {
            return TypedResults.NotFound();
        }
        
    }
}
