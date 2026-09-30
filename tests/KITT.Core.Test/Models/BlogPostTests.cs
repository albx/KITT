using KITT.Core.Models;
using Xunit;

namespace KITT.Core.Test.Models
{
    public class BlogPostTests
    {
        private static Content.SeoData ValidSeo() => new() { Title = "seo title", Description = "seo desc", Keywords = "kw" };

        #region CreateAsDraft tests

        [Fact]
        public void CreateAsDraft_Should_Throw_ArgumentNullException_If_Title_Is_Null()
        {
            var ex = Assert.Throws<ArgumentNullException>(
                () => BlogPost.CreateAsDraft(null!, "slug", "abstract", "content", ValidSeo(), "user1"));

            Assert.Equal("title", ex.ParamName);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void CreateAsDraft_Should_Throw_ArgumentException_If_Title_Is_Empty(string title)
        {
            var ex = Assert.Throws<ArgumentException>(
                () => BlogPost.CreateAsDraft(title, "slug", "abstract", "content", ValidSeo(), "user1"));

            Assert.Equal(nameof(title), ex.ParamName);
        }

        [Fact]
        public void CreateAsDraft_Should_Throw_ArgumentNullException_If_Slug_Is_Null()
        {
            var ex = Assert.Throws<ArgumentNullException>(
                () => BlogPost.CreateAsDraft("title", null!, "abstract", "content", ValidSeo(), "user1"));

            Assert.Equal("slug", ex.ParamName);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void CreateAsDraft_Should_Throw_ArgumentException_If_Slug_Is_Empty(string slug)
        {
            var ex = Assert.Throws<ArgumentException>(
                () => BlogPost.CreateAsDraft("title", slug, "abstract", "content", ValidSeo(), "user1"));

            Assert.Equal(nameof(slug), ex.ParamName);
        }

        [Fact]
        public void CreateAsDraft_Should_Throw_ArgumentNullException_If_Abstract_Is_Null()
        {
            var ex = Assert.Throws<ArgumentNullException>(
                () => BlogPost.CreateAsDraft("title", "slug", null!, "content", ValidSeo(), "user1"));

            Assert.Equal("@abstract", ex.ParamName);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void CreateAsDraft_Should_Throw_ArgumentException_If_Abstract_Is_Empty(string @abstract)
        {
            var ex = Assert.Throws<ArgumentException>(
                () => BlogPost.CreateAsDraft("title", "slug", @abstract, "content", ValidSeo(), "user1"));

            Assert.Equal("@abstract", ex.ParamName);
        }

        [Fact]
        public void CreateAsDraft_Should_Throw_ArgumentNullException_If_Content_Is_Null()
        {
            var ex = Assert.Throws<ArgumentNullException>(
                () => BlogPost.CreateAsDraft("title", "slug", "abstract", null!, ValidSeo(), "user1"));

            Assert.Equal("content", ex.ParamName);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void CreateAsDraft_Should_Throw_ArgumentException_If_Content_Is_Empty(string content)
        {
            var ex = Assert.Throws<ArgumentException>(
                () => BlogPost.CreateAsDraft("title", "slug", "abstract", content, ValidSeo(), "user1"));

            Assert.Equal(nameof(content), ex.ParamName);
        }

        [Fact]
        public void CreateAsDraft_Should_Throw_ArgumentNullException_If_Seo_Is_Null()
        {
            var ex = Assert.Throws<ArgumentNullException>(
                () => BlogPost.CreateAsDraft("title", "slug", "abstract", "content", null!, "user1"));

            Assert.Equal("seo", ex.ParamName);
        }

        [Fact]
        public void CreateAsDraft_Should_Throw_ArgumentNullException_If_UserId_Is_Null()
        {
            var ex = Assert.Throws<ArgumentNullException>(
                () => BlogPost.CreateAsDraft("title", "slug", "abstract", "content", ValidSeo(), null!));

            Assert.Equal("userId", ex.ParamName);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void CreateAsDraft_Should_Throw_ArgumentException_If_UserId_Is_Empty(string userId)
        {
            var ex = Assert.Throws<ArgumentException>(
                () => BlogPost.CreateAsDraft("title", "slug", "abstract", "content", ValidSeo(), userId));

            Assert.Equal(nameof(userId), ex.ParamName);
        }

        [Fact]
        public void CreateAsDraft_Should_Return_Post_With_Draft_Status()
        {
            var post = BlogPost.CreateAsDraft("title", "slug", "abstract", "content", ValidSeo(), "user1");

            Assert.Equal(Content.ContentStatus.Draft, post.Status);
        }

        [Fact]
        public void CreateAsDraft_Should_Return_Post_With_Null_PublicationDate()
        {
            var post = BlogPost.CreateAsDraft("title", "slug", "abstract", "content", ValidSeo(), "user1");

            Assert.Null(post.PublicationDate);
        }

        [Fact]
        public void CreateAsDraft_Should_Return_Post_With_Correct_Properties()
        {
            var seo = ValidSeo();
            var post = BlogPost.CreateAsDraft("my title", "my-slug", "my abstract", "my content", seo, "user1");

            Assert.NotEqual(Guid.Empty, post.Id);
            Assert.Equal("my title", post.Title);
            Assert.Equal("my-slug", post.Slug);
            Assert.Equal("my abstract", post.Abstract);
            Assert.Equal("my content", post.Content);
            Assert.Equal(seo, post.Seo);
            Assert.Equal("user1", post.UserId);
        }

        #endregion

        #region Publish (factory) tests

        [Fact]
        public void Publish_Factory_Should_Throw_ArgumentNullException_If_Title_Is_Null()
        {
            var ex = Assert.Throws<ArgumentNullException>(
                () => BlogPost.Publish(null!, "slug", "abstract", "content", ValidSeo(), "user1"));

            Assert.Equal("title", ex.ParamName);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void Publish_Factory_Should_Throw_ArgumentException_If_Title_Is_Empty(string title)
        {
            var ex = Assert.Throws<ArgumentException>(
                () => BlogPost.Publish(title, "slug", "abstract", "content", ValidSeo(), "user1"));

            Assert.Equal(nameof(title), ex.ParamName);
        }

        [Fact]
        public void Publish_Factory_Should_Throw_ArgumentNullException_If_Slug_Is_Null()
        {
            var ex = Assert.Throws<ArgumentNullException>(
                () => BlogPost.Publish("title", null!, "abstract", "content", ValidSeo(), "user1"));

            Assert.Equal("slug", ex.ParamName);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void Publish_Factory_Should_Throw_ArgumentException_If_Slug_Is_Empty(string slug)
        {
            var ex = Assert.Throws<ArgumentException>(
                () => BlogPost.Publish("title", slug, "abstract", "content", ValidSeo(), "user1"));

            Assert.Equal(nameof(slug), ex.ParamName);
        }

        [Fact]
        public void Publish_Factory_Should_Throw_ArgumentNullException_If_Abstract_Is_Null()
        {
            var ex = Assert.Throws<ArgumentNullException>(
                () => BlogPost.Publish("title", "slug", null!, "content", ValidSeo(), "user1"));

            Assert.Equal("@abstract", ex.ParamName);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void Publish_Factory_Should_Throw_ArgumentException_If_Abstract_Is_Empty(string @abstract)
        {
            var ex = Assert.Throws<ArgumentException>(
                () => BlogPost.Publish("title", "slug", @abstract, "content", ValidSeo(), "user1"));

            Assert.Equal("@abstract", ex.ParamName);
        }

        [Fact]
        public void Publish_Factory_Should_Throw_ArgumentNullException_If_Content_Is_Null()
        {
            var ex = Assert.Throws<ArgumentNullException>(
                () => BlogPost.Publish("title", "slug", "abstract", null!, ValidSeo(), "user1"));

            Assert.Equal("content", ex.ParamName);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void Publish_Factory_Should_Throw_ArgumentException_If_Content_Is_Empty(string content)
        {
            var ex = Assert.Throws<ArgumentException>(
                () => BlogPost.Publish("title", "slug", "abstract", content, ValidSeo(), "user1"));

            Assert.Equal(nameof(content), ex.ParamName);
        }

        [Fact]
        public void Publish_Factory_Should_Throw_ArgumentNullException_If_Seo_Is_Null()
        {
            var ex = Assert.Throws<ArgumentNullException>(
                () => BlogPost.Publish("title", "slug", "abstract", "content", null!, "user1"));

            Assert.Equal("seo", ex.ParamName);
        }

        [Fact]
        public void Publish_Factory_Should_Throw_ArgumentNullException_If_UserId_Is_Null()
        {
            var ex = Assert.Throws<ArgumentNullException>(
                () => BlogPost.Publish("title", "slug", "abstract", "content", ValidSeo(), null!));

            Assert.Equal("userId", ex.ParamName);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void Publish_Factory_Should_Throw_ArgumentException_If_UserId_Is_Empty(string userId)
        {
            var ex = Assert.Throws<ArgumentException>(
                () => BlogPost.Publish("title", "slug", "abstract", "content", ValidSeo(), userId));

            Assert.Equal(nameof(userId), ex.ParamName);
        }

        [Fact]
        public void Publish_Factory_Should_Return_Post_With_Published_Status()
        {
            var post = BlogPost.Publish("title", "slug", "abstract", "content", ValidSeo(), "user1");

            Assert.Equal(Content.ContentStatus.Published, post.Status);
        }

        [Fact]
        public void Publish_Factory_Should_Return_Post_With_PublicationDate_Set()
        {
            var post = BlogPost.Publish("title", "slug", "abstract", "content", ValidSeo(), "user1");

            Assert.NotNull(post.PublicationDate);
        }

        [Fact]
        public void Publish_Factory_Should_Return_Post_With_Correct_Properties()
        {
            var seo = ValidSeo();
            var post = BlogPost.Publish("my title", "my-slug", "my abstract", "my content", seo, "user1");

            Assert.NotEqual(Guid.Empty, post.Id);
            Assert.Equal("my title", post.Title);
            Assert.Equal("my-slug", post.Slug);
            Assert.Equal("my abstract", post.Abstract);
            Assert.Equal("my content", post.Content);
            Assert.Equal(seo, post.Seo);
            Assert.Equal("user1", post.UserId);
        }

        #endregion

        #region Import tests

        [Fact]
        public void Import_Should_Throw_ArgumentNullException_If_Title_Is_Null()
        {
            var ex = Assert.Throws<ArgumentNullException>(
                () => BlogPost.Import(null!, "slug", "abstract", "content", DateTime.UtcNow, DateTime.UtcNow, ValidSeo(), "user1"));

            Assert.Equal("title", ex.ParamName);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void Import_Should_Throw_ArgumentException_If_Title_Is_Empty(string title)
        {
            var ex = Assert.Throws<ArgumentException>(
                () => BlogPost.Import(title, "slug", "abstract", "content", DateTime.UtcNow, DateTime.UtcNow, ValidSeo(), "user1"));

            Assert.Equal(nameof(title), ex.ParamName);
        }

        [Fact]
        public void Import_Should_Throw_ArgumentNullException_If_Slug_Is_Null()
        {
            var ex = Assert.Throws<ArgumentNullException>(
                () => BlogPost.Import("title", null!, "abstract", "content", DateTime.UtcNow, DateTime.UtcNow, ValidSeo(), "user1"));

            Assert.Equal("slug", ex.ParamName);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void Import_Should_Throw_ArgumentException_If_Slug_Is_Empty(string slug)
        {
            var ex = Assert.Throws<ArgumentException>(
                () => BlogPost.Import("title", slug, "abstract", "content", DateTime.UtcNow, DateTime.UtcNow, ValidSeo(), "user1"));

            Assert.Equal(nameof(slug), ex.ParamName);
        }

        [Fact]
        public void Import_Should_Throw_ArgumentNullException_If_Abstract_Is_Null()
        {
            var ex = Assert.Throws<ArgumentNullException>(
                () => BlogPost.Import("title", "slug", null!, "content", DateTime.UtcNow, DateTime.UtcNow, ValidSeo(), "user1"));

            Assert.Equal("@abstract", ex.ParamName);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void Import_Should_Throw_ArgumentException_If_Abstract_Is_Empty(string @abstract)
        {
            var ex = Assert.Throws<ArgumentException>(
                () => BlogPost.Import("title", "slug", @abstract, "content", DateTime.UtcNow, DateTime.UtcNow, ValidSeo(), "user1"));

            Assert.Equal("@abstract", ex.ParamName);
        }

        [Fact]
        public void Import_Should_Throw_ArgumentNullException_If_Content_Is_Null()
        {
            var ex = Assert.Throws<ArgumentNullException>(
                () => BlogPost.Import("title", "slug", "abstract", null!, DateTime.UtcNow, DateTime.UtcNow, ValidSeo(), "user1"));

            Assert.Equal("content", ex.ParamName);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void Import_Should_Throw_ArgumentException_If_Content_Is_Empty(string content)
        {
            var ex = Assert.Throws<ArgumentException>(
                () => BlogPost.Import("title", "slug", "abstract", content, DateTime.UtcNow, DateTime.UtcNow, ValidSeo(), "user1"));

            Assert.Equal(nameof(content), ex.ParamName);
        }

        [Fact]
        public void Import_Should_Throw_ArgumentNullException_If_Seo_Is_Null()
        {
            var ex = Assert.Throws<ArgumentNullException>(
                () => BlogPost.Import("title", "slug", "abstract", "content", DateTime.UtcNow, DateTime.UtcNow, null!, "user1"));

            Assert.Equal("seo", ex.ParamName);
        }

        [Fact]
        public void Import_Should_Throw_ArgumentNullException_If_UserId_Is_Null()
        {
            var ex = Assert.Throws<ArgumentNullException>(
                () => BlogPost.Import("title", "slug", "abstract", "content", DateTime.UtcNow, DateTime.UtcNow, ValidSeo(), null!));

            Assert.Equal("userId", ex.ParamName);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void Import_Should_Throw_ArgumentException_If_UserId_Is_Empty(string userId)
        {
            var ex = Assert.Throws<ArgumentException>(
                () => BlogPost.Import("title", "slug", "abstract", "content", DateTime.UtcNow, DateTime.UtcNow, ValidSeo(), userId));

            Assert.Equal(nameof(userId), ex.ParamName);
        }

        [Fact]
        public void Import_Should_Return_Post_With_Published_Status()
        {
            var post = BlogPost.Import("title", "slug", "abstract", "content", DateTime.UtcNow, DateTime.UtcNow, ValidSeo(), "user1");

            Assert.Equal(Content.ContentStatus.Published, post.Status);
        }

        [Fact]
        public void Import_Should_Return_Post_With_Provided_CreationDate()
        {
            var creationDate = new DateTime(2024, 1, 15, 0, 0, 0, DateTimeKind.Utc);
            var post = BlogPost.Import("title", "slug", "abstract", "content", creationDate, DateTime.UtcNow, ValidSeo(), "user1");

            Assert.Equal(creationDate, post.CreationDate);
        }

        [Fact]
        public void Import_Should_Return_Post_With_Provided_PublicationDate()
        {
            var publicationDate = new DateTime(2024, 3, 20, 0, 0, 0, DateTimeKind.Utc);
            var post = BlogPost.Import("title", "slug", "abstract", "content", DateTime.UtcNow, publicationDate, ValidSeo(), "user1");

            Assert.Equal(publicationDate, post.PublicationDate);
        }

        [Fact]
        public void Import_Should_Return_Post_With_Correct_Properties()
        {
            var seo = ValidSeo();
            var creationDate = new DateTime(2024, 1, 15, 0, 0, 0, DateTimeKind.Utc);
            var publicationDate = new DateTime(2024, 3, 20, 0, 0, 0, DateTimeKind.Utc);

            var post = BlogPost.Import("my title", "my-slug", "my abstract", "my content", creationDate, publicationDate, seo, "user1");

            Assert.NotEqual(Guid.Empty, post.Id);
            Assert.Equal("my title", post.Title);
            Assert.Equal("my-slug", post.Slug);
            Assert.Equal("my abstract", post.Abstract);
            Assert.Equal("my content", post.Content);
            Assert.Equal(seo, post.Seo);
            Assert.Equal("user1", post.UserId);
        }

        #endregion

        #region UpdateContent tests

        [Fact]
        public void UpdateContent_Should_Throw_ArgumentNullException_If_Content_Is_Null()
        {
            var post = BlogPost.CreateAsDraft("title", "slug", "abstract", "initial content", ValidSeo(), "user1");

            var ex = Assert.Throws<ArgumentNullException>(() => post.UpdateContent(null!));

            Assert.Equal("content", ex.ParamName);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void UpdateContent_Should_Throw_ArgumentException_If_Content_Is_Empty(string content)
        {
            var post = BlogPost.CreateAsDraft("title", "slug", "abstract", "initial content", ValidSeo(), "user1");

            var ex = Assert.Throws<ArgumentException>(() => post.UpdateContent(content));

            Assert.Equal(nameof(content), ex.ParamName);
        }

        [Fact]
        public void UpdateContent_Should_Update_Content_Property()
        {
            var post = BlogPost.CreateAsDraft("title", "slug", "abstract", "initial content", ValidSeo(), "user1");

            post.UpdateContent("updated content");

            Assert.Equal("updated content", post.Content);
        }

        #endregion
    }
}
