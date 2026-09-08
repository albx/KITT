using KITT.Core.Commands;
using KITT.Core.Models;
using KITT.Core.Test.Fixtures;
using Xunit;

namespace KITT.Core.Test.Commands
{
    public class BlogPostCommandsTest : IClassFixture<KittDbContextFixture>
    {
        private readonly KittDbContextFixture _fixture;

        private static Content.SeoData ValidSeo() => new() { Title = "seo title", Description = "seo desc", Keywords = "kw" };

        private static DateTime FakeCreationDate => new DateTime(2024, 1, 1, 10, 30, 0);

        private static DateTime FakePublicationDate => new DateTime(2024, 1, 15, 14, 0, 0);

        public BlogPostCommandsTest(KittDbContextFixture fixture)
        {
            _fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));
        }

        #region PublishPostAsync tests

        [Fact]
        public async Task PublishPostAsync_Should_Return_NonEmpty_PostId()
        {
            var commands = new BlogPostCommands(_fixture.Context);

            var postId = await commands.PublishPostAsync("title", "slug-pub-1", "abstract", "content", ValidSeo(), "user1");

            Assert.NotEqual(Guid.Empty, postId);
        }

        [Fact]
        public async Task PublishPostAsync_Should_Add_Post_To_Database()
        {
            var commands = new BlogPostCommands(_fixture.Context);

            var postId = await commands.PublishPostAsync("title", "slug-pub-2", "abstract", "content", ValidSeo(), "user1");

            var post = _fixture.Context.BlogPosts.Find(postId);
            Assert.NotNull(post);
        }

        [Fact]
        public async Task PublishPostAsync_Should_Add_Post_With_Published_Status()
        {
            var commands = new BlogPostCommands(_fixture.Context);

            var postId = await commands.PublishPostAsync("title", "slug-pub-3", "abstract", "content", ValidSeo(), "user1");

            var post = _fixture.Context.BlogPosts.Find(postId);
            Assert.Equal(Content.ContentStatus.Published, post!.Status);
        }

        [Fact]
        public async Task PublishPostAsync_Should_Add_Post_With_PublicationDate_Set()
        {
            var commands = new BlogPostCommands(_fixture.Context);

            var postId = await commands.PublishPostAsync("title", "slug-pub-4", "abstract", "content", ValidSeo(), "user1");

            var post = _fixture.Context.BlogPosts.Find(postId);
            Assert.NotNull(post!.PublicationDate);
        }

        [Fact]
        public async Task PublishPostAsync_Should_Add_Post_With_Correct_Properties()
        {
            var seo = ValidSeo();
            var commands = new BlogPostCommands(_fixture.Context);

            var postId = await commands.PublishPostAsync("my title", "slug-pub-5", "my abstract", "my content", seo, "user1");

            var post = _fixture.Context.BlogPosts.Find(postId);
            Assert.Equal("my title", post!.Title);
            Assert.Equal("slug-pub-5", post.Slug);
            Assert.Equal("my abstract", post.Abstract);
            Assert.Equal("my content", post.Content);
            Assert.Equal("user1", post.UserId);
        }

        #endregion

        #region CreatePostAsDraftAsync tests

        [Fact]
        public async Task CreatePostAsDraftAsync_Should_Return_NonEmpty_PostId()
        {
            var commands = new BlogPostCommands(_fixture.Context);

            var postId = await commands.CreatePostAsDraftAsync("title", "slug-draft-1", "abstract", "content", ValidSeo(), "user1");

            Assert.NotEqual(Guid.Empty, postId);
        }

        [Fact]
        public async Task CreatePostAsDraftAsync_Should_Add_Post_To_Database()
        {
            var commands = new BlogPostCommands(_fixture.Context);

            var postId = await commands.CreatePostAsDraftAsync("title", "slug-draft-2", "abstract", "content", ValidSeo(), "user1");

            var post = _fixture.Context.BlogPosts.Find(postId);
            Assert.NotNull(post);
        }

        [Fact]
        public async Task CreatePostAsDraftAsync_Should_Add_Post_With_Draft_Status()
        {
            var commands = new BlogPostCommands(_fixture.Context);

            var postId = await commands.CreatePostAsDraftAsync("title", "slug-draft-3", "abstract", "content", ValidSeo(), "user1");

            var post = _fixture.Context.BlogPosts.Find(postId);
            Assert.Equal(Content.ContentStatus.Draft, post!.Status);
        }

        [Fact]
        public async Task CreatePostAsDraftAsync_Should_Add_Post_With_Null_PublicationDate()
        {
            var commands = new BlogPostCommands(_fixture.Context);

            var postId = await commands.CreatePostAsDraftAsync("title", "slug-draft-4", "abstract", "content", ValidSeo(), "user1");

            var post = _fixture.Context.BlogPosts.Find(postId);
            Assert.Null(post!.PublicationDate);
        }

        [Fact]
        public async Task CreatePostAsDraftAsync_Should_Add_Post_With_Correct_Properties()
        {
            var seo = ValidSeo();
            var commands = new BlogPostCommands(_fixture.Context);

            var postId = await commands.CreatePostAsDraftAsync("my title", "slug-draft-5", "my abstract", "my content", seo, "user1");

            var post = _fixture.Context.BlogPosts.Find(postId);
            Assert.Equal("my title", post!.Title);
            Assert.Equal("slug-draft-5", post.Slug);
            Assert.Equal("my abstract", post.Abstract);
            Assert.Equal("my content", post.Content);
            Assert.Equal("user1", post.UserId);
        }

        #endregion

        #region ImportPostAsync tests

        [Fact]
        public async Task ImportPostAsync_Should_Return_NonEmpty_PostId()
        {
            var commands = new BlogPostCommands(_fixture.Context);

            var postId = await commands.ImportPostAsync(
                "title", 
                "slug-import-1", 
                "abstract", 
                "content", 
                FakeCreationDate,
                FakePublicationDate,
                ValidSeo(), 
                "user1");

            Assert.NotEqual(Guid.Empty, postId);
        }

        [Fact]
        public async Task ImportPostAsync_Should_Add_Post_To_Database()
        {
            var commands = new BlogPostCommands(_fixture.Context);

            var postId = await commands.ImportPostAsync(
                "title", 
                "slug-import-2", 
                "abstract", 
                "content",
                FakeCreationDate,
                FakePublicationDate,
                ValidSeo(), 
                "user1");

            var post = _fixture.Context.BlogPosts.Find(postId);
            Assert.NotNull(post);
        }

        [Fact]
        public async Task ImportPostAsync_Should_Add_Post_With_Published_Status()
        {
            var commands = new BlogPostCommands(_fixture.Context);

            var postId = await commands.ImportPostAsync(
                "title", 
                "slug-import-3", 
                "abstract", 
                "content",
                FakeCreationDate,
                FakePublicationDate,
                ValidSeo(), 
                "user1");

            var post = _fixture.Context.BlogPosts.Find(postId);
            Assert.Equal(Content.ContentStatus.Published, post!.Status);
        }

        [Fact]
        public async Task ImportPostAsync_Should_Add_Post_With_Correct_Properties()
        {
            var seo = ValidSeo();
            var commands = new BlogPostCommands(_fixture.Context);

            var postId = await commands.ImportPostAsync(
                "my title", 
                "slug-import-4", 
                "my abstract", 
                "my content",
                FakeCreationDate,
                FakePublicationDate,
                seo, 
                "user1");

            var post = _fixture.Context.BlogPosts.Find(postId);
            Assert.Equal("my title", post!.Title);
            Assert.Equal("slug-import-4", post.Slug);
            Assert.Equal("my abstract", post.Abstract);
            Assert.Equal("my content", post.Content);
            Assert.Equal("user1", post.UserId);
        }

        #endregion

        #region UpdatePostAsync tests

        [Fact]
        public async Task UpdatePostAsync_Should_Throw_InvalidOperationException_If_Post_Not_Found()
        {
            var commands = new BlogPostCommands(_fixture.Context);
            var nonExistentId = Guid.NewGuid();

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => commands.UpdatePostAsync(nonExistentId, "new title", "new abstract", "new content", ValidSeo()));
        }

        [Fact]
        public async Task UpdatePostAsync_Should_Update_Post_Title()
        {
            var commands = new BlogPostCommands(_fixture.Context);
            var postId = await commands.CreatePostAsDraftAsync("original title", "slug-upd-1", "abstract", "content", ValidSeo(), "user1");

            await commands.UpdatePostAsync(postId, "updated title", "abstract", "content", ValidSeo());

            var post = _fixture.Context.BlogPosts.Find(postId);
            Assert.Equal("updated title", post!.Title);
        }

        [Fact]
        public async Task UpdatePostAsync_Should_Update_Post_Abstract()
        {
            var commands = new BlogPostCommands(_fixture.Context);
            var postId = await commands.CreatePostAsDraftAsync("title", "slug-upd-2", "original abstract", "content", ValidSeo(), "user1");

            await commands.UpdatePostAsync(postId, "title", "updated abstract", "content", ValidSeo());

            var post = _fixture.Context.BlogPosts.Find(postId);
            Assert.Equal("updated abstract", post!.Abstract);
        }

        [Fact]
        public async Task UpdatePostAsync_Should_Update_Post_Content()
        {
            var commands = new BlogPostCommands(_fixture.Context);
            var postId = await commands.CreatePostAsDraftAsync("title", "slug-upd-3", "abstract", "original content", ValidSeo(), "user1");

            await commands.UpdatePostAsync(postId, "title", "abstract", "updated content", ValidSeo());

            var post = _fixture.Context.BlogPosts.Find(postId);
            Assert.Equal("updated content", post!.Content);
        }

        [Fact]
        public async Task UpdatePostAsync_Should_Update_Post_SeoData()
        {
            var commands = new BlogPostCommands(_fixture.Context);
            var postId = await commands.CreatePostAsDraftAsync("title", "slug-upd-4", "abstract", "content", ValidSeo(), "user1");
            var updatedSeo = new Content.SeoData { Title = "new seo title", Description = "new desc", Keywords = "new kw" };

            await commands.UpdatePostAsync(postId, "title", "abstract", "content", updatedSeo);

            var post = _fixture.Context.BlogPosts.Find(postId);
            Assert.Equal(updatedSeo, post!.Seo);
        }

        #endregion
    }
}
