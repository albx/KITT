using KITT.Cms.Web.App.Clients;
using KITT.Cms.Web.Models;
using KITT.Cms.Web.Models.BlogPosts;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using PostSortDirection = KITT.Cms.Web.Models.SortDirection;

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

    private Option<PostSortDirection>[] directions = [];

    private Option<ContentStatus?>[] statuses = [
        new() { Value = null, Text = "All" },
        new() { Value = ContentStatus.Draft, Text = "Drafts only" },
        new() { Value = ContentStatus.Published, Text = "Published only" }
    ];

    private Option<int>[] sizes = [
        new() { Value = 5, Text = "5" },
        new() { Value = 10, Text = "10" },
        new() { Value = 25, Text = "25" },
        new() { Value = 50, Text = "50" }
    ];

    private PaginationState paginationState = new();

    protected override void OnInitialized()
    {
        directions = Enum.GetValues<PostSortDirection>()
            .Select(v => new Option<PostSortDirection>() { Value = v, Text = v.ToString() })
            .ToArray();

        SetPaginationState();
    }

    private async Task LoadPostsAsync(BlogPostsQueryModel query)
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

    private async Task SearchAsync() => await LoadPostsAsync(query);

    private async Task OnPageChangedAsync(int pageIndex)
    {
        query.Page = pageIndex + 1;
        await LoadPostsAsync(query);
    }

    private void SetPaginationState()
    {
        paginationState.ItemsPerPage = query.Size;
    }

    private void OpenPostDetailPage(BlogPostListModel.BlogPostListItemModel post) 
        => navigationManager.NavigateTo($"/blog/post/{post.Id}");

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
            await LoadPostsAsync(query);
        }
    }

    private async Task ClearSearchAsync()
    {
        query = new();
        SetPaginationState();

        await LoadPostsAsync(query);
    }
}
