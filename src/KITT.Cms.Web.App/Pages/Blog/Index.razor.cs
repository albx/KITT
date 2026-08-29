using KITT.Cms.Web.App.Clients;
using KITT.Cms.Web.Models.BlogPosts;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;

namespace KITT.Cms.Web.App.Pages.Blog;

public partial class Index(
    IBlogClient client,
    NavigationManager navigationManager)
{
    private BlogPostListModel model = new();

    private IQueryable<BlogPostListModel.BlogPostListItemModel> posts = new List<BlogPostListModel.BlogPostListItemModel>().AsQueryable();

    private BlogPostsQueryModel query = new();

    private int numberOfPages = 0;

    private bool loading = false;

    private PaginationState paginationState = new();

    protected override void OnInitialized()
    {
        SetPaginationState();
    }

    protected override async Task OnInitializedAsync()
    {
        await LoadPostsAsync();
    }

    private async Task LoadPostsAsync()
    {
        loading = true;

        try
        {
            model = await client.GetBlogPostsAsync(query);
            posts = model.Items.AsQueryable();
            await paginationState.SetTotalItemCountAsync(model.TotalItems);
        }
        finally
        {
            loading = false;
        }
    }

    private void SetPaginationState()
    {
        paginationState.ItemsPerPage = query.Size;
    }

    private void OpenPostDetailPage(BlogPostListModel.BlogPostListItemModel post)
    {
        navigationManager.NavigateTo($"/blog/post/{post.Id}");
    }
}
