using KITT.Cms.Web.Models.BlogPosts;
using KITT.Web.Shared.Security;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Security.Claims;

namespace KITT.Cms.Web.Api.BlogPosts;

public static class BlogPostsEndpoints
{
    extension(IEndpointRouteBuilder builder)
    {
        public IEndpointRouteBuilder MapBlogPostsEndpoints()
        {
            var blogPostsGroup = builder
                .MapGroup("api/posts")
                .RequireAuthorization();

            blogPostsGroup
                .MapGet("", GetAllBlogPosts)
                .WithName(nameof(GetAllBlogPosts));

            //blogPostsGroup
            //    .MapGet("{id:guid}", GetBlogPostDetail)
            //    .WithName(nameof(GetBlogPostDetail));
            //blogPostsGroup
            //    .MapPost("", CreateBlogPost)
            //    .WithName(nameof(CreateBlogPost));
            //blogPostsGroup
            //    .MapPut("{id:guid}", UpdateBlogPost)
            //    .WithName(nameof(UpdateBlogPost));

            return builder;
        }
    }

    private static async Task<Ok<BlogPostListModel>> GetAllBlogPosts(
        BlogPostsEndpointsServices services,
        ClaimsPrincipal user,
        [AsParameters] BlogPostsQueryModel query)
    {
        var userId = user.GetUserId();

        var model = await services.GetAllBlogPostsAsync(query, userId);
        return TypedResults.Ok(model);
    }
}
