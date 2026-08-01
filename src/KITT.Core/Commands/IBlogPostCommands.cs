namespace KITT.Core.Commands;

public interface IBlogPostCommands
{
    Task<Guid> PublishPostAsync(string title, string slug, string @abstract, string content, Content.SeoData seo, string userId);

    Task<Guid> CreatePostAsDraftAsync(string title, string slug, string @abstract, string content, Content.SeoData seo, string userId);

    Task<Guid> ImportPostAsync(string title, string slug, string @abstract, string content, Content.SeoData seo, string userId);

    Task UpdatePostAsync(Guid postId, string title, string @abstract, string content, Content.SeoData seo);
}
