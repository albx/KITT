using KITT.Web.App.Models;
using KITT.Web.App.UI;
using KITT.Cms.Web.Models.Streamings;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components.Extensions;
using System.ComponentModel.DataAnnotations;
using KITT.Web.App.Clients;

namespace KITT.Web.App.Pages.Streamings;

public partial class StreamingDetail
{
    [Parameter]
    [EditorRequired]
    public Guid Id { get; set; }

    [Inject]
    public IStreamingsClient Client { get; set; } = default!;

    [Inject]
    public IToastService ToastService { get; set; } = default!;

    [Inject]
    public IMessageService MessageService { get; set; } = default!;

    private bool isReadOnly = true;

    private ViewModel model = new();

    private string pageTitle = "StreamingDetail";

    private const string twitchBaseUrl = "https://www.twitch.tv/";

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
            await Client.UpdateStreamingAsync(detail);

            isReadOnly = true;
            ToastService.ShowSuccess(Localizer[nameof(Resources.Pages.Streamings.StreamingDetail.StreamingSavedSuccessfully)]);

            streamingDetail = detail;
        }
        catch (ApplicationException ex)
        {
            await MessageService.ShowMessageBarAsync(
                ex.Message,
                MessageIntent.Error,
                SectionNames.MessagesTopSectionName);
        }
    }

    protected override async Task OnInitializedAsync()
    {
        streamingDetail = await Client.GetStreamingDetailAsync(Id) ?? new();
        model = ViewModel.FromStreamingDetailModel(streamingDetail);

        pageTitle = model.Title;
    }

    class ViewModel : ContentViewModel
    {
        [Required]
        public string Title { get; set; } = string.Empty;

        public string Slug { get; set; } = string.Empty;

        [Required]
        public DateTime? ScheduleDate { get; set; } = DateTime.Today;

        [Required]
        public DateTime? StartingTime { get; set; } = DateTime.Now;

        [Required]
        public DateTime? EndingTime { get; set; } = DateTime.Now.AddHours(1);

        [Required]
        public string HostingChannelUrl { get; set; } = string.Empty;

        public string? StreamingAbstract { get; set; }

        public string? YoutubeVideoUrl { get; set; }

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
                Title = this.Title,
                Slug = this.Slug,
                ScheduleDate = DateOnly.FromDateTime(this.ScheduleDate.Value),
                EndingTime = TimeOnly.FromTimeSpan(this.EndingTime.Value.TimeOfDay),
                HostingChannelUrl = $"{twitchBaseUrl}{this.HostingChannelUrl}",
                StartingTime = TimeOnly.FromTimeSpan(this.StartingTime.Value.TimeOfDay),
                StreamingAbstract = this.StreamingAbstract,
                YoutubeVideoUrl = this.YoutubeVideoUrl,
                Seo = this.Seo
            };
        }

        public static ViewModel FromStreamingDetailModel(StreamingDetailModel model)
        {
            return new()
            {
                Title = model.Title,
                Slug = model.Slug,
                ScheduleDate = model.ScheduleDate.ToDateTime(TimeOnly.MinValue),
                EndingTime = model.EndingTime.ToDateTime(),
                StartingTime = model.StartingTime.ToDateTime(),
                StreamingAbstract = model.StreamingAbstract,
                YoutubeVideoUrl = model.YoutubeVideoUrl,
                HostingChannelUrl = model.HostingChannelUrl.Replace(twitchBaseUrl, string.Empty).Trim()
            };
        }
    }
}
