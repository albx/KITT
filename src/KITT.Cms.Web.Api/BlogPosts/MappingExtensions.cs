using KITT.Cms.Web.Models.BlogPosts;

namespace KITT.Cms.Web.Api.BlogPosts;

public static class MappingExtensions
{
    extension(DraftBlogPostModel model)
    {
        public BlogPostDetailModel ToDetailModel(Guid postId)
        {
            return new BlogPostDetailModel
            {
                Id = postId,
                Title = model.Title,
                Slug = model.Slug,
                PostAbstract = model.PostAbstract,
                Content = model.Content,
                Seo = model.Seo
            };
        }
    }

    extension(PublishBlogPostModel model)
    {
        public BlogPostDetailModel ToDetailModel(Guid postId)
        {
            return new BlogPostDetailModel
            {
                Id = postId,
                Title = model.Title,
                Slug = model.Slug,
                PostAbstract = model.PostAbstract,
                Content = model.Content,
                Seo = model.Seo
            };
        }
    }

    extension(ImportBlogPostModel model)
    {
        public BlogPostDetailModel ToDetailModel(Guid postId)
        {
            return new BlogPostDetailModel
            {
                Id = postId,
                Title = model.Title,
                Slug = model.Slug,
                PostAbstract = model.PostAbstract,
                Content = model.Content,
                Seo = model.Seo
            };
        }
    }
}
