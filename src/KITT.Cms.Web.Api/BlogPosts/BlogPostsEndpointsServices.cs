using KITT.Cms.Web.Api.Mapping;
using KITT.Cms.Web.Models;
using KITT.Cms.Web.Models.BlogPosts;
using KITT.Core.Commands;
using KITT.Core.ReadModels;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

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

        var rawPosts = await postsQuery
            .Select(p => new
            {
                p.Id,
                p.Title,
                p.Slug,
                p.PublicationDate,
                p.Status
            })
            .Skip(skip)
            .Take(query.Size)
            .ToArrayAsync();

        // The enum-to-enum cast between the entity's ContentStatus and the model's ContentStatus
        // cannot be translated to SQL, so the mapping is performed in memory after materialization.
        var posts = rawPosts.Select(p => new BlogPostListModel.BlogPostListItemModel
        {
            Id = p.Id,
            Title = p.Title,
            Slug = p.Slug,
            PublishedOn = p.PublicationDate,
            Status = MapStatus(p.Status)
        });

        return new()
        {
            TotalItems = await postsQuery.CountAsync(),
            Items = posts,
        };
    }

    private static ContentStatus MapStatus(Core.Models.Content.ContentStatus status) => status switch
    {
        Core.Models.Content.ContentStatus.Draft => ContentStatus.Draft,
        Core.Models.Content.ContentStatus.Published => ContentStatus.Published,
        Core.Models.Content.ContentStatus.Unpublished => ContentStatus.Unpublished,
        _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
    };

    public async Task<BlogPostDetailModel?> GetBlogPostDetailAsync(Guid postId, string userId)
    {
        var post = await database.BlogPosts
            .ByUserId(userId)
            .SingleOrDefaultAsync(p => p.Id == postId);

        if (post is null) 
        {
            return null;
        }

        return new()
        {
            Id = post.Id,
            Content = post.Content,
            PostAbstract = post.Abstract,
            Seo = post.Seo.ToModel(),
            Slug = post.Slug,
            Title = post.Title,
            CreationDate = post.CreationDate,
            PublicationDate = post.PublicationDate,
            Status = MapStatus(post.Status)
        };
    }

    public Task<Guid> PublishBlogPostAsync(PublishBlogPostModel model, string userId)
    {
        return commands.PublishPostAsync(
            model.Title,
            model.Slug,
            model.PostAbstract,
            model.Content,
            model.Seo.ToEntity(),
            userId);
    }

    public Task<Guid> CreateDraftBlogPostAsync(DraftBlogPostModel model, string userId)
    {
        return commands.CreatePostAsDraftAsync(
            model.Title,
            model.Slug,
            model.PostAbstract,
            model.Content,
            model.Seo.ToEntity(),
            userId);
    }

    public Task<Guid> ImportBlogPostAsync(ImportBlogPostModel model, string userId)
    {
        if (!model.CreationDate.HasValue)
        {
            throw new ValidationException("Creation date is required");
        }

        if (!model.PublicationDate.HasValue)
        {
            throw new ValidationException("Publication date is required");
        }

        return commands.ImportPostAsync(
            model.Title,
            model.Slug,
            model.PostAbstract,
            model.Content,
            model.CreationDate.Value,
            model.PublicationDate.Value,
            model.Seo.ToEntity(),
            userId);
    }

    public Task UpdateBlogPostAsync(Guid postId, UpdateBlogPostModel model)
    {
        return commands.UpdatePostAsync(
            postId,
            model.Title,
            model.PostAbstract,
            model.Content,
            model.Seo.ToEntity());
    }
}
