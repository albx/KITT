using KITT.Cms.Web.App.Clients;
using KITT.Cms.Web.Models;
using KITT.Cms.Web.Models.BlogPosts;
using KITT.Web.App.UI;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using OperationResults;
using System.ComponentModel.DataAnnotations;

namespace KITT.Cms.Web.App.Pages.Blog;

public partial class PostDetail(
    IBlogClient client,
    IToastService toastService,
    IMessageService messageService)
{
    [Parameter]
    public Guid Id { get; set; }

    private bool isReadOnly = false;

    private ViewModel model = new();

    protected override async Task OnInitializedAsync()
    {
        isReadOnly = true;
        await LoadBlogPostDetailAsync();
    }

    private async Task LoadBlogPostDetailAsync()
    {
        var detailResult = await client.GetPostDetailAsync(Id);
        if (!detailResult.Success)
        {
            string errorMessage = detailResult.FailureReason switch
            {
                FailureReasons.ItemNotFound => "The requested blog post was not found.",
                _ => "There was an error retrieving the blog post details."
            };

            await messageService.ShowMessageBarAsync(
                errorMessage,
                MessageIntent.Error,
                SectionNames.MessagesTopSectionName);

            return;
        }

        model = MapToViewModel(detailResult.Content!);
    }

    private void EnableEditing() => isReadOnly = false;

    private static ViewModel MapToViewModel(BlogPostDetailModel model)
        => new()
        {
            Content = model.Content,
            PostAbstract = model.PostAbstract ?? string.Empty,
            Seo = model.Seo,
            Slug = model.Slug,
            Title = model.Title,
            CreationDate = model.CreationDate,
            PublicationDate = model.PublicationDate
        };

    private static UpdateBlogPostModel MapToUpdateBlogPostModel(ViewModel model)
        => new()
        {
            Content = model.Content,
            PostAbstract = model.PostAbstract,
            Seo = model.Seo,
            Title = model.Title
        };

    class ViewModel : ContentViewModel
    {
        [Required]
        public string Title { get; set; } = string.Empty;
        
        public string Slug { get; set; } = string.Empty;

        [Required]
        public string PostAbstract { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        public DateTime? CreationDate { get; set; }

        public DateTime? PublicationDate { get; set; }
    }
}
