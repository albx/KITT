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

    private ScheduleStreamingModel ConvertToApiModel(ScheduleForm.ViewModel model)
    {
        if (model.ScheduleDate is null)
        {
            throw new ArgumentNullException(nameof(model.ScheduleDate));
        }

        if (model.StartingTime is null)
        {
            throw new ArgumentNullException(nameof(model.StartingTime));
        }

        if (model.EndingTime is null)
        {
            throw new ArgumentNullException(nameof(model.EndingTime));
        }

        return new ScheduleStreamingModel
        {
            Title = model.Title,
            ScheduleDate = model.ScheduleDate.Value,
            EndingTime = model.ScheduleDate.Value.Add(model.EndingTime.Value),
            HostingChannelUrl = $"https://www.twitch.tv/{model.HostingChannelUrl}",
            Slug = model.Slug,
            StartingTime = model.ScheduleDate.Value.Add(model.StartingTime.Value),
            StreamingAbstract = model.StreamingAbstract,
            Seo = model.Seo
        };
    }
}
