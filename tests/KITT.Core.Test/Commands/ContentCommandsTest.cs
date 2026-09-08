using KITT.Core.Commands;
using KITT.Core.Models;
using KITT.Core.Test.Fixtures;
using Xunit;

namespace KITT.Core.Test.Commands
{
    public class ContentCommandsTest : IClassFixture<KittDbContextFixture>
    {
        private readonly KittDbContextFixture _fixture;

        public ContentCommandsTest(KittDbContextFixture fixture)
        {
            _fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));
        }

        #region DeleteContentAsync tests

        [Fact(Skip = "ExecuteDeleteAsync is not supported by the EF Core InMemory provider. Requires a relational database provider.")]
        public async Task DeleteContentAsync_Should_Remove_Content_From_Database()
        {
            var post = BlogPost.CreateAsDraft("title", "slug-del-1", "abstract", "content",
                new Content.SeoData { Title = "seo" }, "user1");

            _fixture.PrepareData(ctx =>
            {
                ctx.Contents.Add(post);
                ctx.SaveChanges();
            });

            var commands = new ContentCommands(_fixture.Context);
            await commands.DeleteContentAsync(post.Id);

            var found = _fixture.Context.Contents.Find(post.Id);
            Assert.Null(found);
        }

        [Fact(Skip = "ExecuteDeleteAsync is not supported by the EF Core InMemory provider. Requires a relational database provider.")]
        public async Task DeleteContentAsync_Should_Not_Throw_If_Content_Does_Not_Exist()
        {
            var commands = new ContentCommands(_fixture.Context);
            var nonExistentId = Guid.NewGuid();

            var ex = await Record.ExceptionAsync(() => commands.DeleteContentAsync(nonExistentId));

            Assert.Null(ex);
        }

        #endregion

        #region PublishContentAsync tests

        [Fact]
        public async Task PublishContentAsync_Should_Throw_InvalidOperationException_If_Content_Not_Found()
        {
            var commands = new ContentCommands(_fixture.Context);
            var nonExistentId = Guid.NewGuid();

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => commands.PublishContentAsync(nonExistentId, DateTime.UtcNow));
        }

        [Fact]
        public async Task PublishContentAsync_Should_Set_Published_Status()
        {
            var post = BlogPost.CreateAsDraft("title", "slug-publish-1", "abstract", "content",
                new Content.SeoData { Title = "seo" }, "user1");

            _fixture.PrepareData(ctx =>
            {
                ctx.Contents.Add(post);
                ctx.SaveChanges();
            });

            var commands = new ContentCommands(_fixture.Context);
            await commands.PublishContentAsync(post.Id, DateTime.UtcNow);

            var updated = _fixture.Context.Contents.Find(post.Id);
            Assert.Equal(Content.ContentStatus.Published, updated!.Status);
        }

        [Fact]
        public async Task PublishContentAsync_Should_Set_PublicationDate()
        {
            var post = BlogPost.CreateAsDraft("title", "slug-publish-2", "abstract", "content",
                new Content.SeoData { Title = "seo" }, "user1");

            _fixture.PrepareData(ctx =>
            {
                ctx.Contents.Add(post);
                ctx.SaveChanges();
            });

            var publicationDate = new DateTime(2025, 6, 1, 0, 0, 0, DateTimeKind.Utc);
            var commands = new ContentCommands(_fixture.Context);
            await commands.PublishContentAsync(post.Id, publicationDate);

            var updated = _fixture.Context.Contents.Find(post.Id);
            Assert.Equal(publicationDate, updated!.PublicationDate);
        }

        #endregion
    }
}
