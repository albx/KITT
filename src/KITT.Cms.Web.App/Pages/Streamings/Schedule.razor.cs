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

    private ScheduleForm.ViewModel model = new();

    private async Task ScheduleStreamingAsync(ScheduleForm.ViewModel model)
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

    private static ScheduleStreamingModel ConvertToApiModel(ScheduleForm.ViewModel model)
    {
        ArgumentNullException.ThrowIfNull(model.ScheduleDate);
        ArgumentNullException.ThrowIfNull(model.StartingTime);
        ArgumentNullException.ThrowIfNull(model.EndingTime);

        return new ScheduleStreamingModel
        {
            Title = model.Title,
            ScheduleDate = DateOnly.FromDateTime(model.ScheduleDate.Value),
            EndingTime = TimeOnly.FromTimeSpan(model.EndingTime.Value.TimeOfDay),
            HostingChannelUrl = $"https://www.twitch.tv/{model.HostingChannelUrl}",
            Slug = model.Slug,
            StartingTime = TimeOnly.FromTimeSpan(model.StartingTime.Value.TimeOfDay),
            StreamingAbstract = model.StreamingAbstract,
            Seo = model.Seo
        };
    }
}

