using KITT.Cms.Web.Models.BlogPosts;
using OperationResults;

namespace KITT.Cms.Web.App.Clients;

public interface IBlogClient
{
    Task<Result<BlogPostDetailModel>> SavePostDraftAsync(DraftBlogPostModel model);

    Task<Result<BlogPostDetailModel>> ImportPostAsync(ImportBlogPostModel model);

    Task<Result<BlogPostDetailModel>> GetPostDetailAsync(Guid postId);
}
