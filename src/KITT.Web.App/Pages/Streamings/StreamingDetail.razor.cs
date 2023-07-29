using KITT.Web.App.Clients;
using KITT.Web.App.Model;
using KITT.Web.Models.Streamings;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.ComponentModel.DataAnnotations;

namespace KITT.Web.App.Pages.Streamings;

public partial class StreamingDetail
{
    [Parameter]
    [EditorRequired]
    public Guid Id { get; set; }

    [Inject]
    public IStreamingsClient Client { get; set; }

    [Inject]
    ISnackbar Snackbar { get; set; } = default!;

    private bool isReadOnly = true;

    private ViewModel model = new();

    private string pageTitle = "StreamingDetail";

    const string twitchBaseUrl = "https://www.twitch.tv/";

    void EnableEditing()
    {
        isReadOnly = false;
        StateHasChanged();
    }

    void DisableEditing()
    {
        model = ViewModel.FromStreamingDetailModel(streamingDetail);
        isReadOnly = true;

        StateHasChanged();
    }

    private string? errorMessage;

    private StreamingDetailModel streamingDetail = new();

    async Task EditStreamingAsync(ViewModel model)
    {
        try
        {
            var detail = model.ToApiModel(Id);
            await Client.UpdateStreamingAsync(detail);

            isReadOnly = true;
            Snackbar.Add("Streaming informations saved successfully!", Severity.Success);

            streamingDetail = detail;
        }
        catch (ApplicationException ex)
        {
            errorMessage = ex.Message;
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
        public DateTime? ScheduleDate { get; set; } = DateTime.Now;

        [Required]
        public TimeSpan? StartingTime { get; set; } = DateTime.Now.TimeOfDay;

        [Required]
        public TimeSpan? EndingTime { get; set; } = DateTime.Now.TimeOfDay.Add(TimeSpan.FromHours(1));

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
                ScheduleDate = this.ScheduleDate.Value,
                EndingTime = this.EndingTime.Value,
                HostingChannelUrl = $"{twitchBaseUrl}{this.HostingChannelUrl}",
                StartingTime = this.StartingTime.Value,
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
                ScheduleDate = model.ScheduleDate,
                EndingTime = model.EndingTime,
                StartingTime = model.StartingTime,
                StreamingAbstract = model.StreamingAbstract,
                YoutubeVideoUrl = model.YoutubeVideoUrl,
                HostingChannelUrl = model.HostingChannelUrl.Replace(twitchBaseUrl, string.Empty).Trim()
            };
        }
    }
}
