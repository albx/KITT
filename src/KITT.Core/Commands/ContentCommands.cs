namespace KITT.Core.Commands;

public class ContentCommands(KittDbContext context) : IContentCommands
{
    public Task DeleteContentAsync(Guid contentId)
    {
        return context.Contents
            .Where(c => c.Id == contentId)
            .ExecuteDeleteAsync();
    }

    public async Task PublishContentAsync(Guid contentId, DateTime publicationDate)
    {
        var content = await context.Contents.SingleOrDefaultAsync(c => c.Id == contentId);
        if (content is null)
        {
            throw new InvalidOperationException($"Content with ID {contentId} not found.");
        }

        content.PublishOn(publicationDate);
        await context.SaveChangesAsync();
    }
}
