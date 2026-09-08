namespace KITT.Core.Commands;

public class ContentCommands(KittDbContext context) : IContentCommands
{
    public async Task DeleteContentAsync(Guid contentId)
    {
        var content = await context.Contents.SingleOrDefaultAsync(c => c.Id == contentId);
        if (content is null)
        {
            throw new InvalidOperationException($"Content with ID {contentId} not found.");
        }

        context.Contents.Remove(content);
        await context.SaveChangesAsync();
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
