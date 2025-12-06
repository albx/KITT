namespace KITT.Core.ReadModels.Extensions;

public static class ContentExtensions
{
    extension<TContent>(IQueryable<TContent> source) where TContent : Content
    {
        public IQueryable<TContent> PublishedOnly() 
            => source.Where(c => c.Status == Content.ContentStatus.Published && c.PublicationDate <= DateTime.UtcNow);

        public IQueryable<TContent> DraftsOnly() => source.Where(c => c.Status == Content.ContentStatus.Draft);

        public IQueryable<TContent> UnpublishedOnly() => source.Where(c => c.Status == Content.ContentStatus.Unpublished);

        public IQueryable<TContent> OrderedByPublicationDate() => source.OrderBy(c => c.PublicationDate);

        public IQueryable<TContent> WithSlug(string slug) => source.Where(c => c.Slug == slug);
    }
    
}
