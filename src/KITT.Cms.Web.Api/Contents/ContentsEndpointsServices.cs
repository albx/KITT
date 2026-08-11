using KITT.Cms.Web.Models.Contents;
using KITT.Core.Commands;

namespace KITT.Cms.Web.Api.Contents;

public class ContentsEndpointsServices(IContentCommands commands)
{
    public Task PublishContentAsync(Guid contentId, PublishContentModel model) 
        => commands.PublishContentAsync(contentId, model.PublicationDate);

    public Task DeleteContentAsync(Guid contentId) 
        => commands.DeleteContentAsync(contentId);
}
