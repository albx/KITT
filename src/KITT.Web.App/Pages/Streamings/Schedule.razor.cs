using KITT.Web.App.Clients;
using KITT.Web.App.Components;
using KITT.Web.Models.Streamings;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace KITT.Web.App.Pages.Streamings;

public partial class Schedule
{
    [Inject]
    public IStreamingsClient Client { get; set; } = default!;

    [Inject]
    public NavigationManager Navigation { get; set; } = default!;

    [Inject]
    ISnackbar Snackbar { get; set; } = default!;

    [Inject]
    public IDialogService Dialog { get; set; } = default!;

    private ScheduleForm.ViewModel model = new();

    private string? errorMessage;

    async Task ScheduleStreamingAsync(ScheduleForm.ViewModel model)
    {
        try
        {
            var scheduleStreamingModel = ConvertToApiModel(model);
            await Client.ScheduleStreamingAsync(scheduleStreamingModel);

            Snackbar.Add(Localizer[nameof(Resources.Pages.Streamings.Schedule.StreamingScheduledSuccessfully), model.Title], Severity.Success);

            Navigation.NavigateTo("/streamings");
        }
        catch (ApplicationException ex)
        {
            errorMessage = ex.Message;
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
            ScheduleDate = model.ScheduleDate.Value,
            EndingTime = model.EndingTime.Value,
            HostingChannelUrl = $"https://www.twitch.tv/{model.HostingChannelUrl}",
            Slug = model.Slug,
            StartingTime = model.StartingTime.Value,
            StreamingAbstract = model.StreamingAbstract,
            Seo = model.Seo
        };
    }
}
