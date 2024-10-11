using KITT.Web.App.Clients;
using KITT.Web.App.Model;
using KITT.Web.App.UI;
using KITT.Web.Models.Streamings;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;

namespace KITT.Web.App.Pages.Streamings;

public partial class Import
{
    [Inject]
    public IStreamingsClient Client { get; set; } = default!;

    [Inject]
    public NavigationManager Navigation { get; set; } = default!;

    [Inject]
    public IToastService ToastService { get; set; } = default!;

    [Inject]
    public IMessageService MessageService { get; set; } = default!;

    private ViewModel model = new();

    private string errorMessage = string.Empty;

    private async Task ImportStreamingAsync(ViewModel model)
    {
        try
        {
            await Client.ImportStreamingAsync(model.ToApiModel());
            ToastService.ShowSuccess(
                Localizer[nameof(Resources.Pages.Streamings.Import.StreamingImportedSuccessfully), model.Title]);

            Navigation.NavigateTo("/streamings");
        }
        catch (ApplicationException ex)
        {
            await MessageService.ShowMessageBarAsync(
                ex.Message,
                MessageIntent.Error,
                SectionNames.MessagesTopSectionName);
        }
    }

    void Cancel() => model = new();

    class ViewModel : ContentViewModel
    {
        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Slug { get; set; } = string.Empty;

        [Required]
        public DateTime? ScheduleDate { get; set; } = DateTime.Now;

        [Required]
        public DateTime? StartingTime { get; set; } = DateTime.Now;

        [Required]
        public DateTime? EndingTime { get; set; } = DateTime.Now.AddHours(1);

        [Required]
        public string HostingChannelUrl { get; set; } = string.Empty;

        public string? StreamingAbstract { get; set; }

        public string? YoutubeVideoUrl { get; set; }

        public ImportStreamingModel ToApiModel()
        {
            if (this.ScheduleDate is null)
            {
                throw new ArgumentNullException(nameof(ScheduleDate));
            }

            if (this.StartingTime is null)
            {
                throw new ArgumentNullException(nameof(this.StartingTime));
            }

            if (this.EndingTime is null)
            {
                throw new ArgumentNullException(nameof(this.EndingTime));
            }

            return new()
            {
                Title = this.Title,
                ScheduleDate = this.ScheduleDate.Value,
                EndingTime = this.EndingTime.Value.TimeOfDay,
                HostingChannelUrl = $"https://www.twitch.tv/{this.HostingChannelUrl}",
                Slug = this.Slug,
                StartingTime = this.StartingTime.Value.TimeOfDay,
                StreamingAbstract = this.StreamingAbstract,
                YoutubeVideoUrl = this.YoutubeVideoUrl,
                Seo = this.Seo
            };
        }
    }
}