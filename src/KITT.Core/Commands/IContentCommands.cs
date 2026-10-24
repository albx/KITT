namespace KITT.Core.Commands;

public interface IContentCommands
{
    Task DeleteContentAsync(Guid contentId, string userId);

    Task PublishContentAsync(Guid contentId, DateTime publicationDate, string userId);
}
