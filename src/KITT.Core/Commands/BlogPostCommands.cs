using static KITT.Core.Models.Content;

namespace KITT.Core.Commands;

public class BlogPostCommands(KittDbContext context) : IBlogPostCommands
{
    public async Task<Guid> PublishPostAsync(string title, string slug, string @abstract, string content, SeoData seo, string userId)
    {
        var post = BlogPost.Publish(title, slug, @abstract, content, seo, userId);
        context.BlogPosts.Add(post);
        
        await context.SaveChangesAsync();
        return post.Id;
    }
    public async Task<Guid> CreatePostAsDraftAsync(string title, string slug, string @abstract, string content, SeoData seo, string userId)
    {
        var post = BlogPost.CreateAsDraft(title, slug, @abstract, content, seo, userId);
        context.BlogPosts.Add(post);
        
        await context.SaveChangesAsync();
        return post.Id;
    }
    public async Task<Guid> ImportPostAsync(string title, string slug, string @abstract, string content, DateTime creationDate, DateTime publicationDate, SeoData seo, string userId)
    {
        var post = BlogPost.Import(title, slug, @abstract, content, creationDate, publicationDate, seo, userId);
        context.BlogPosts.Add(post);
        
        await context.SaveChangesAsync();
        return post.Id;
    }
    public async Task UpdatePostAsync(Guid postId, string title, string @abstract, string content, SeoData seo)
    {
        var post = await context.BlogPosts.SingleOrDefaultAsync(p => p.Id == postId);
        if (post is null)
        {
            throw new InvalidOperationException($"Blog post {postId} not found");
        }

        post.ChangeTitle(title);
        post.SetAbstract(@abstract);
        post.UpdateContent(content);
        post.SetSeoData(seo);

        await context.SaveChangesAsync();
    }
}
