using System.ComponentModel.DataAnnotations;
using KITT.Web.App.Clients;
using KITT.Web.Models.Streamings;
using Microsoft.AspNetCore.Components;

namespace KITT.Web.App.Pages.Streamings;

public partial class Import
{
    [Inject]
    public IStreamingsClient Client { get; set; }

    [Inject]
    public NavigationManager Navigation { get; set; }

    private ViewModel model = new();

    private string errorMessage = string.Empty;

    async Task ImportStreamingAsync()
    {
        try
        {
            await Client.ImportStreamingAsync(model.ToApiModel());

            Navigation.NavigateTo("/streamings");
        }
        catch (ApplicationException ex)
        {
            errorMessage = ex.Message;
        }
    }

    class ViewModel
    {
        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Slug { get; set; } = string.Empty;

        [Required]
        public DateTime? ScheduleDate { get; set; } = DateTime.Now;

        [Required]
        public TimeSpan? StartingTime { get; set; } = DateTime.Now.TimeOfDay;

        [Required]
        public TimeSpan? EndingTime { get; set; } = DateTime.Now.TimeOfDay.Add(TimeSpan.FromHours(1));

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
                EndingTime = this.ScheduleDate.Value.Add(this.EndingTime.Value),
                HostingChannelUrl = $"https://www.twitch.tv/{this.HostingChannelUrl}",
                Slug = this.Slug,
                StartingTime = this.ScheduleDate.Value.Add(this.StartingTime.Value),
                StreamingAbstract = this.StreamingAbstract,
                YoutubeVideoUrl = this.YoutubeVideoUrl
            };
        }
    }
}