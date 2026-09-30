using KITT.Cms.Web.Models.Contents;
using KITT.Core.Commands;

namespace KITT.Cms.Web.Api.Contents;

public class ContentsEndpointsServices(IContentCommands commands)
{
    public Task PublishContentAsync(Guid contentId, PublishContentModel model, string userId) 
        => commands.PublishContentAsync(contentId, model.PublicationDate!.Value, userId);

    public Task DeleteContentAsync(Guid contentId, string userId) 
        => commands.DeleteContentAsync(contentId, userId);
}
