using KITT.Cms.Web.Models.Contents;
using OperationResults;

namespace KITT.Cms.Web.App.Clients;

public interface IContentClient
{
    Task<Result> PublishContentAsync(Guid contentId, PublishContentModel model);

    Task<Result> DeleteContentAsync(Guid contentId);
}
