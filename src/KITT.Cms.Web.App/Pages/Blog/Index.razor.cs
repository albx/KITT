using KITT.Cms.Web.App.Clients;
using KITT.Cms.Web.Models.BlogPosts;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;

namespace KITT.Cms.Web.App.Pages.Blog;

public partial class Index(
    IBlogClient blogClient,
    IContentClient contentClient,
    NavigationManager navigationManager,
    IToastService toastService,
    IDialogService dialogService)
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
            model = await blogClient.GetBlogPostsAsync(query);
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

    private async Task DeletePostAsync(BlogPostListModel.BlogPostListItemModel post)
    {
        var postTitle = post.Title;
        string confirmText = $"You are going to delete the post {postTitle}. Are you sure?";

        var confirm = await dialogService.ShowConfirmationAsync(
            confirmText,
            primaryText: CommonLocalizer[nameof(KITT.Web.App.UI.Resources.Common.Confirm)],
            secondaryText: CommonLocalizer[(nameof(KITT.Web.App.UI.Resources.Common.Cancel))],
            title: $"Deleting {postTitle}");

        var result = await confirm.Result;
        if (!result.Cancelled)
        {
            var deleteResult = await contentClient.DeleteContentAsync(post.Id);
            if (!deleteResult.Success)
            {
                toastService.ShowError($"There was an error deleting post {postTitle}");
                return;
            }

            toastService.ShowSuccess($"Post {postTitle} deleted successfully!");
            await LoadPostsAsync();
        }
    }
}
