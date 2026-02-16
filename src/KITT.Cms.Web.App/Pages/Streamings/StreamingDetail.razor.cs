using KITT.Cms.Web.App.Clients;
using KITT.Cms.Web.Models;
using KITT.Cms.Web.Models.Settings;
using KITT.Cms.Web.Models.Streamings;
using KITT.Web.App.UI;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components.Extensions;
using System.ComponentModel.DataAnnotations;

namespace KITT.Cms.Web.App.Pages.Streamings;

public partial class StreamingDetail(
    IStreamingsClient client,
    IToastService toastService,
    IMessageService messageService,
    IConnectedChannelsClient channelsClient)
{
    [Parameter]
    [EditorRequired]
    public Guid Id { get; set; }

    private bool isReadOnly = true;

    private ViewModel model = new();

    private string pageTitle = "StreamingDetail";

    private ChannelModel?[] availableTwitchChannels = [];

    private ChannelModel?[] availableYouTubeChannels = [];

    private void EnableEditing() => isReadOnly = false;

    private void DisableEditing()
    {
        model = ViewModel.FromStreamingDetailModel(streamingDetail);
        isReadOnly = true;
    }

    private StreamingDetailModel streamingDetail = new();

    private async Task EditStreamingAsync(ViewModel model)
    {
        try
        {
            var detail = model.ToApiModel(Id);
            await client.UpdateStreamingAsync(detail);

            isReadOnly = true;
            toastService.ShowSuccess(Localizer[nameof(Resources.Pages.Streamings.StreamingDetail.StreamingSavedSuccessfully)]);

            streamingDetail = detail;
        }
        catch (ApplicationException ex)
        {
            await messageService.ShowMessageBarAsync(
                ex.Message,
                MessageIntent.Error,
                SectionNames.MessagesTopSectionName);
        }
    }

    protected override async Task OnInitializedAsync()
    {
        await LoadConnectedChannelsAsync();

        streamingDetail = await client.GetStreamingDetailAsync(Id) ?? new();
        model = ViewModel.FromStreamingDetailModel(streamingDetail);

        pageTitle = model.Title;
    }

    private async Task LoadConnectedChannelsAsync()
    {
        var channels = await channelsClient.GetConnectedChannelsAsync();

        availableTwitchChannels = [null, .. channels.Where(c => c.Type is Cms.Settings.Models.ChannelType.Twitch).ToArray()];
        availableYouTubeChannels = [null, .. channels.Where(c => c.Type is Cms.Settings.Models.ChannelType.YouTube).ToArray()];
    }

    class ViewModel : ContentViewModel
    {
        public string? TwitchChannel { get; set; }

        public string? YouTubeChannel { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        public string Slug { get; set; } = string.Empty;

        [Required]
        public DateTime? ScheduleDate { get; set; } = DateTime.Today;

        [Required]
        public DateTime? StartingTime { get; set; } = DateTime.Now;

        [Required]
        public DateTime? EndingTime { get; set; } = DateTime.Now.AddHours(1);

        public string TwitchUrl { get; set; } = string.Empty;

        public string? StreamingAbstract { get; set; }

        public string YouTubeUrl { get; set; } = string.Empty;

        public StreamingDetailModel ToApiModel(Guid streamingId)
        {
            if (streamingId == Guid.Empty)
            {
                throw new ArgumentException("value cannot be empty", nameof(streamingId));
            }

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
                Id = streamingId,
                TwitchChannel = this.TwitchChannel,
                YouTubeChannel = this.YouTubeChannel,
                Title = this.Title,
                Slug = this.Slug,
                ScheduleDate = DateOnly.FromDateTime(this.ScheduleDate.Value),
                EndingTime = TimeOnly.FromTimeSpan(this.EndingTime.Value.TimeOfDay),
                TwitchUrl = this.TwitchUrl,
                StartingTime = TimeOnly.FromTimeSpan(this.StartingTime.Value.TimeOfDay),
                StreamingAbstract = this.StreamingAbstract,
                YouTubeUrl = this.YouTubeUrl,
                Seo = this.Seo
            };
        }

        public static ViewModel FromStreamingDetailModel(StreamingDetailModel model)
        {
            return new()
            {
                Title = model.Title,
                TwitchChannel = model.TwitchChannel,
                YouTubeChannel = model.YouTubeChannel,
                Slug = model.Slug,
                ScheduleDate = model.ScheduleDate.ToDateTime(TimeOnly.MinValue),
                EndingTime = model.EndingTime.ToDateTime(),
                StartingTime = model.StartingTime.ToDateTime(),
                StreamingAbstract = model.StreamingAbstract,
                YouTubeUrl = model.YouTubeUrl,
                TwitchUrl = model.TwitchUrl,
                Seo = model.Seo
            };
        }
    }
}

