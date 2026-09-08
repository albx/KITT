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

    private Option<ContentStatus?>[] statuses = [];

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
            .Select(v => new Option<PostSortDirection>() { Value = v, Text = Localizer[v.ToString()] })
            .ToArray();

        statuses = [
            new() { Value = null, Text = Localizer[nameof(Resources.Pages.Blog.Index.AllStatusFilterLabel)] },
            new() { Value = ContentStatus.Draft, Text = Localizer[nameof(Resources.Pages.Blog.Index.DraftsOnlyFilterLabel)] },
            new() { Value = ContentStatus.Published, Text = Localizer[nameof(Resources.Pages.Blog.Index.PublishedOnlyFilterLabel)] }
        ];

        SetPaginationState();
    }

    private async Task LoadPostsAsync(BlogPostsQueryModel query)
    {
        loading = true;

        try
        {
            model = await blogClient.GetBlogPostsAsync(query);
            numberOfPages = (int)Math.Ceiling(model.TotalItems / (decimal)query.Size);

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
        string confirmText = Localizer[nameof(Resources.Pages.Blog.Index.DeletePostConfirmText), postTitle];

        var confirm = await dialogService.ShowConfirmationAsync(
            confirmText,
            primaryText: CommonLocalizer[nameof(KITT.Web.App.UI.Resources.Common.Confirm)],
            secondaryText: CommonLocalizer[(nameof(KITT.Web.App.UI.Resources.Common.Cancel))],
            title: Localizer[nameof(Resources.Pages.Blog.Index.DeletePostConfirmTitle), postTitle]);

        var result = await confirm.Result;
        if (!result.Cancelled)
        {
            var deleteResult = await contentClient.DeleteContentAsync(post.Id);
            if (!deleteResult.Success)
            {
                toastService.ShowError(Localizer[nameof(Resources.Pages.Blog.Index.DeletePostErrorMessage), postTitle]);
                return;
            }

            toastService.ShowSuccess(Localizer[nameof(Resources.Pages.Blog.Index.DeletePostSuccessMessage), postTitle]);
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
