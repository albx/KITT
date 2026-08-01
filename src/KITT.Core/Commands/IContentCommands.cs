namespace KITT.Core.Commands;

public interface IContentCommands
{
    Task DeleteContentAsync(Guid contentId);

    Task PublishContentAsync(Guid contentId, DateTime publicationDate);
}
