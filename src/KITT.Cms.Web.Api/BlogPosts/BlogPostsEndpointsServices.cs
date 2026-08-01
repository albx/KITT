using KITT.Cms.Web.Models;
using KITT.Cms.Web.Models.BlogPosts;
using KITT.Core.Commands;
using KITT.Core.ReadModels;
using Microsoft.EntityFrameworkCore;

namespace KITT.Cms.Web.Api.BlogPosts;

public class BlogPostsEndpointsServices(IDatabase database, IBlogPostCommands commands)
{
    public async Task<BlogPostListModel?> GetAllBlogPostsAsync(BlogPostsQueryModel query, string userId)
    {
        var ascending = query.PublishSort == SortDirection.Ascending;
        var postsQuery = database.BlogPosts
            .ByUserId(userId)
            .OrderedByPublicationDate(ascending);
        
        postsQuery = query.Status switch
        {
            ContentStatus.Draft => postsQuery.DraftsOnly(),
            ContentStatus.Published => postsQuery.PublishedOnly(),
            ContentStatus.Unpublished => postsQuery.UnpublishedOnly(),
            _ => postsQuery
        };

        if (!string.IsNullOrWhiteSpace(query.Query))
        {
            postsQuery = postsQuery.Where(p => p.Title.Contains(query.Query));
        }

        var skip = (query.Page - 1) * query.Size;

        var posts = await postsQuery
            .Select(p => new BlogPostListModel.BlogPostListItemModel
            {
                Id = p.Id,
                Title = p.Title,
                Slug = p.Slug,
                PublishedOn = p.PublicationDate,
                Status = (ContentStatus)p.Status
            })
            .Skip(skip)
            .Take(query.Size)
            .ToArrayAsync();

        return new()
        {
            TotalItems = await postsQuery.CountAsync(),
            Items = posts,
        };
    }
}
