namespace KITT.Core.Models;

public abstract class Content
{
    #region Properties
    public Guid Id { get; protected set; }

    public string UserId { get; protected set; }

    public string Title { get; protected set; }

    public string Slug { get; protected set; }

    public string Abstract { get; protected set; }

    public virtual SeoData Seo { get; protected set; }

    public DateTime? PublicationDate { get; protected set; }

    public DateTime CreationDate { get; protected set; }

    public ContentStatus Status { get; protected set; } = ContentStatus.Draft;
    #endregion

    #region Constructor
    protected Content()
    {
        CreationDate = DateTime.UtcNow;
        Seo = new();
    }
    #endregion

    #region Public methods
    public virtual void ChangeTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("value cannot be null", nameof(title));
        }

        Title = title;
    }

    public virtual void SetAbstract(string @abstract)
    {
        Abstract = @abstract;
    }

    public virtual void SetSeoData(SeoData seo) => Seo = seo ?? throw new ArgumentNullException(nameof(seo));

    public virtual void Publish() => PublishOn(DateTime.UtcNow);

    public virtual void PublishOn(DateTime publicationDate)
    {
        PublicationDate = publicationDate;
        Status = ContentStatus.Published;
    }

    public virtual void Unpublish() => Status = ContentStatus.Unpublished;

    public virtual void RestoreAsPublished() => Status = ContentStatus.Published;
    #endregion

    #region Inner classes
    public record SeoData
    {
        public string Title { get; init; }

        public string Description { get; init; }

        public string Keywords { get; init; }
    }

    public enum ContentStatus
    {
        Draft,
        Published,
        Unpublished
    }
    #endregion
}
