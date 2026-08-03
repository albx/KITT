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
            Seo = new()
            {
                Title = post.Seo?.Title ?? string.Empty,
                Description = post.Seo?.Description ?? string.Empty,
                Keywords = post.Seo?.Keywords ?? string.Empty,
            },
            Slug = post.Slug,
            Title = post.Title,
        };
    }

    public Task<Guid> PublishBlogPostAsync(PublishBlogPostModel model, string userId)
    {
        return commands.PublishPostAsync(
            model.Title,
            model.Slug,
            model.PostAbstract,
            model.Content,
            new()
            {
                Title = model.Seo.Title,
                Description = model.Seo.Description,
                Keywords = model.Seo.Keywords
            },
            userId);
    }

    public Task<Guid> CreateDraftBlogPostAsync(DraftBlogPostModel model, string userId)
    {
        return commands.CreatePostAsDraftAsync(
            model.Title,
            model.Slug,
            model.PostAbstract,
            model.Content,
            new()
            {
                Title = model.Seo.Title,
                Description = model.Seo.Description,
                Keywords = model.Seo.Keywords
            },
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
            new()
            {
                Title = model.Seo.Title,
                Description = model.Seo.Description,
                Keywords = model.Seo.Keywords
            },
            userId);
    }
}
