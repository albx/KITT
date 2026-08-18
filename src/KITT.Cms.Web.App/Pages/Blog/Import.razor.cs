using KITT.Cms.Web.App.Clients;
using KITT.Cms.Web.Models;
using KITT.Cms.Web.Models.BlogPosts;
using KITT.Web.App.UI;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;

namespace KITT.Cms.Web.App.Pages.Blog;

public partial class Import(
    IBlogClient client, 
    NavigationManager navigationManager, 
    IToastService toastService, 
    IMessageService messageService)
{
    private ViewModel model = new();

    private void Cancel() => model = new();

    private async Task SavePostAsync()
    {
        var importModel = MapToImportBlogPostModel(model);
        var result = await client.ImportPostAsync(importModel);

        if (!result.Success)
        {
            await messageService.ShowMessageBarAsync(
                "There was an error importing the post",
                MessageIntent.Error,
                SectionNames.MessagesTopSectionName);

            return;
        }

        toastService.ShowSuccess("Post imported successfully!");
        navigationManager.NavigateTo($"blog/post/{result.Content!.Id}");
    }

    private static ImportBlogPostModel MapToImportBlogPostModel(ViewModel model)
        => new()
        {
            Content = model.Content,
            PostAbstract = model.PostAbstract,
            Seo = model.Seo,
            Slug = model.Slug,
            Title = model.Title,
            CreationDate = model.CreationDate,
            PublicationDate = model.PublicationDate
        };

    public class ViewModel : ContentViewModel
    {
        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Slug { get; set; } = string.Empty;

        [Required]
        public DateTime? CreationDate { get; set; }

        [Required]
        public DateTime? PublicationDate { get; set; }

        [Required]
        public string PostAbstract { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;
    }
}
