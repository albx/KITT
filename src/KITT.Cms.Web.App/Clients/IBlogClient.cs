using KITT.Cms.Web.Models.BlogPosts;
using OperationResults;

namespace KITT.Cms.Web.App.Clients;

public interface IBlogClient
{
    Task<BlogPostListModel> GetBlogPostsAsync(BlogPostsQueryModel query);

    Task<Result<BlogPostDetailModel>> SavePostDraftAsync(DraftBlogPostModel model);

    Task<Result<BlogPostDetailModel>> ImportPostAsync(ImportBlogPostModel model);

    Task<Result<BlogPostDetailModel>> GetPostDetailAsync(Guid postId);

    Task<Result> UpdateBlogPostAsync(Guid postId, UpdateBlogPostModel model);
}
