namespace KITT.Core.ReadModels.Extensions;

public static class ContentExtensions
{
    public static IQueryable<TContent> PublishedOnly<TContent>(this IQueryable<TContent> source) where TContent : Content 
        => source.Where(c => c.Status == Content.ContentStatus.Published && c.PublicationDate <= DateTime.UtcNow);

    public static IQueryable<TContent> DraftsOnly<TContent>(this IQueryable<TContent> source) where TContent : Content
        => source.Where(c => c.Status == Content.ContentStatus.Draft);

    public static IQueryable<TContent> UnpublishedOnly<TContent>(this IQueryable<TContent> source) where TContent : Content
        => source.Where(c => c.Status == Content.ContentStatus.Unpublished);

    public static IQueryable<TContent> OrderedByPublicationDate<TContent>(this IQueryable<TContent> source) where TContent : Content
        => source.OrderBy(c => c.PublicationDate);
}
