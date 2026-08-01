namespace KITT.Core.Models;

public class BlogPost : Content
{
    #region Properties
    public string Content { get; protected set; } = string.Empty;
    #endregion

    #region Constructor
    protected BlogPost() : base() { }
    #endregion

    #region Public methods
    public virtual void UpdateContent(string content)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(content);
        Content = content;
    }
    #endregion

    #region Factories
    public static BlogPost CreateAsDraft(string title, string slug, string @abstract, string content, SeoData seo, string userId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        ArgumentException.ThrowIfNullOrWhiteSpace(slug);
        ArgumentException.ThrowIfNullOrWhiteSpace(@abstract);
        ArgumentException.ThrowIfNullOrWhiteSpace(content);
        ArgumentNullException.ThrowIfNull(seo);
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);

        var post = new BlogPost
        {
            Id = Guid.NewGuid(),
            Title = title,
            Slug = slug,
            Abstract = @abstract,
            Content = content,
            Status = ContentStatus.Draft,
            Seo = seo,
            UserId = userId
        };

        return post;
    }

    public static BlogPost Publish(string title, string slug, string @abstract, string content, SeoData seo, string userId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        ArgumentException.ThrowIfNullOrWhiteSpace(slug);
        ArgumentException.ThrowIfNullOrWhiteSpace(@abstract);
        ArgumentException.ThrowIfNullOrWhiteSpace(content);
        ArgumentNullException.ThrowIfNull(seo);
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);

        var post = new BlogPost
        {
            Id = Guid.NewGuid(),
            Title = title,
            Slug = slug,
            Abstract = @abstract,
            Content = content,
            Seo = seo,
            UserId = userId
        };

        post.Publish();

        return post;
    }

    public static BlogPost Import(string title, string slug, string @abstract, string content, DateTime creationDate, DateTime publicationDate, SeoData seo, string userId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        ArgumentException.ThrowIfNullOrWhiteSpace(slug);
        ArgumentException.ThrowIfNullOrWhiteSpace(@abstract);
        ArgumentException.ThrowIfNullOrWhiteSpace(content);
        ArgumentNullException.ThrowIfNull(seo);
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);

        var post = new BlogPost
        {
            Id = Guid.NewGuid(),
            Title = title,
            Slug = slug,
            Abstract = @abstract,
            Content = content,
            Seo = seo,
            UserId = userId,
            CreationDate = creationDate,
        };

        post.PublishOn(publicationDate);

        return post;
    }
    #endregion
}
