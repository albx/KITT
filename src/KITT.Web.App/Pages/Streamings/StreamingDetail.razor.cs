using KITT.Web.App.Clients;
using KITT.Web.Models.Streamings;
using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;

namespace KITT.Web.App.Pages.Streamings;

public partial class StreamingDetail
{
	[Parameter]
	[EditorRequired]
	public Guid Id { get; set; }

	[Inject]
    public IStreamingsClient Client { get; set; }

    private bool isReadOnly = true;

	private ViewModel model = new();

	private string pageTitle = "StreamingDetail";

	const string twitchBaseUrl = "https://www.twitch.tv/";

	const string youtubeBaseUrl = "https://www.youtube.com/?v=";

	void EnableEditing() => isReadOnly = false;

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();

		var streamingDetail = await Client.GetStreamingDetailAsync(Id);
		model = ViewModel.FromStreamingDetailModel(streamingDetail);

		pageTitle = model.Title;
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

		public StreamingDetailModel ToApiModel()
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
				HostingChannelUrl = $"{twitchBaseUrl}{this.HostingChannelUrl}",
				Slug = this.Slug,
				StartingTime = this.ScheduleDate.Value.Add(this.StartingTime.Value),
				StreamingAbstract = this.StreamingAbstract,
				YoutubeVideoUrl = $"{youtubeBaseUrl}{this.YoutubeVideoUrl}"
			};
		}

		public static ViewModel FromStreamingDetailModel(StreamingDetailModel model)
		{
			return new()
			{
				Title = model.Title,
				ScheduleDate = model.ScheduleDate,
				EndingTime = model.EndingTime.TimeOfDay,
				Slug = model.Slug,
				StartingTime = model.StartingTime.TimeOfDay,
				StreamingAbstract = model.StreamingAbstract,
				YoutubeVideoUrl = model.YoutubeVideoUrl?.Replace(youtubeBaseUrl, string.Empty).Trim(),
				HostingChannelUrl = model.HostingChannelUrl.Replace(twitchBaseUrl, string.Empty).Trim()
			};
		}
	}
}
