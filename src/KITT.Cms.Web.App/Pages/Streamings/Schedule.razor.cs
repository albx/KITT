using KITT.Cms.Web.App.Clients;
using KITT.Cms.Web.App.Components;
using KITT.Cms.Web.Models.Streamings;
using KITT.Web.App.UI;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;

namespace KITT.Cms.Web.App.Pages.Streamings;

public partial class Schedule
{
    [Inject]
    public IStreamingsClient Client { get; set; } = default!;

    [Inject]
    public NavigationManager Navigation { get; set; } = default!;

    [Inject]
    public IToastService ToastService { get; set; } = default!;

    [Inject]
    public IDialogService DialogService { get; set; } = default!;

    [Inject]
    public IMessageService MessageService { get; set; } = default!;

    private StreamingForm.ViewModel model = new();

    private async Task ScheduleStreamingAsync(StreamingForm.ViewModel model)
    {
        try
        {
            var scheduleStreamingModel = ConvertToApiModel(model);
            await Client.ScheduleStreamingAsync(scheduleStreamingModel);

            ToastService.ShowSuccess(
                Localizer[nameof(Resources.Pages.Streamings.Schedule.StreamingScheduledSuccessfully), model.Title]);

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

    private static ScheduleStreamingModel ConvertToApiModel(StreamingForm.ViewModel model)
    {
        if (!model.ScheduleDate.HasValue)
        {
            throw new ArgumentNullException(nameof(model.ScheduleDate));
        }

        if (!model.StartingTime.HasValue)
        {
            throw new ArgumentNullException(nameof(model.StartingTime));
        }

        if (!model.EndingTime.HasValue)
        {
            throw new ArgumentNullException(nameof(model.EndingTime));
        }

        return new ScheduleStreamingModel
        {
            Title = model.Title,
            TwitchChannel = model.TwitchChannel,
            YouTubeChannel = model.YouTubeChannel,
            ScheduleDate = DateOnly.FromDateTime(model.ScheduleDate.Value),
            EndingTime = TimeOnly.FromTimeSpan(model.EndingTime.Value.TimeOfDay),
            TwitchUrl = $"https://www.twitch.tv/{model.TwitchUrl}",
            YouTubeUrl = model.YouTubeUrl,
            Slug = model.Slug,
            StartingTime = TimeOnly.FromTimeSpan(model.StartingTime.Value.TimeOfDay),
            StreamingAbstract = model.StreamingAbstract,
            Seo = model.Seo
        };
    }
}

