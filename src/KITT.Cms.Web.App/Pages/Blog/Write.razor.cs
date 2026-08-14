using KITT.Cms.Web.App.Clients;
using KITT.Cms.Web.Models;
using System.ComponentModel.DataAnnotations;

namespace KITT.Cms.Web.App.Pages.Blog;

public partial class Write(IBlogClient client)
{
    private ViewModel model = new();

    private void Cancel()
    {
        model = new();
    }

    private async Task SavePostAsync()
    {

    }

    public class ViewModel : ContentViewModel
    {
        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Slug { get; set; } = string.Empty;

        [Required]
        public string PostAbstract { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;
    }
}
