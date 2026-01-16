using KITT.Cms.Web.App.Clients;
using KITT.Cms.Web.Models;
using KITT.Cms.Web.Models.Settings;
using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;

namespace KITT.Cms.Web.App.Components;

public partial class StreamingForm(
    IConnectedChannelsClient channelsClient)
{
    [Parameter]
    public ViewModel Model { get; set; } = new();

    [Parameter]
    public EventCallback<ViewModel> OnSave { get; set; }

    [Parameter]
    public EventCallback OnCancel { get; set; }

    private ChannelModel[] availableTwitchChannels = [];

    private ChannelModel[] availableYouTubeChannels = [];

    protected override async Task OnInitializedAsync()
    {
        var channels = await channelsClient.GetConnectedChannelsAsync();

        availableTwitchChannels = channels.Where(c => c.Type is Settings.Models.ChannelType.Twitch).ToArray();
        availableYouTubeChannels = channels.Where(c => c.Type is Settings.Models.ChannelType.YouTube).ToArray();
    }

    public class ViewModel : ContentViewModel, IValidatableObject
    {
        public string? TwitchChannel { get; set; }

        public string? YouTubeChannel { get; set; }

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

        public string? TwitchUrl { get; set; } = string.Empty;

        public string? StreamingAbstract { get; set; }

        public string? YouTubeUrl { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(TwitchChannel) && string.IsNullOrWhiteSpace(YouTubeChannel))
            {
                yield return new("At least a channel must be selected", [nameof(TwitchChannel), nameof(YouTubeChannel)]);
            }

            if (!string.IsNullOrWhiteSpace(TwitchChannel) && string.IsNullOrWhiteSpace(TwitchUrl))
            {
                yield return new("Twitch URL is required since a Twitch channel has been set", [nameof(TwitchUrl)]);
            }

            if (!string.IsNullOrWhiteSpace(YouTubeChannel) && string.IsNullOrWhiteSpace(YouTubeUrl))
            {
                yield return new("YouTube URL is required since a YouTube channel has been set", [nameof(YouTubeUrl)]);
            }
        }
    }
}
