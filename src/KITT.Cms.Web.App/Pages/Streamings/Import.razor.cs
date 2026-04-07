using KITT.Cms.Web.App.Clients;
using KITT.Cms.Web.App.Components;
using KITT.Cms.Web.Models.Streamings;
using KITT.Web.App.UI;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;

namespace KITT.Cms.Web.App.Pages.Streamings;

public partial class Import(
    IStreamingsClient client,
    NavigationManager navigation,
    IToastService toastService,
    IMessageService messageService)
{
    private StreamingForm.ViewModel model = new();

    private async Task ImportStreamingAsync(StreamingForm.ViewModel model)
    {
        try
        {
            await client.ImportStreamingAsync(ConvertToApiModel(model));
            toastService.ShowSuccess(
                Localizer[nameof(Resources.Pages.Streamings.Import.StreamingImportedSuccessfully), model.Title]);

            navigation.NavigateTo("/streamings");
        }
        catch (ApplicationException ex)
        {
            await messageService.ShowMessageBarAsync(
                ex.Message,
                MessageIntent.Error,
                SectionNames.MessagesTopSectionName);
        }
    }

    void Cancel() => model = new();

    private static ImportStreamingModel ConvertToApiModel(StreamingForm.ViewModel model)
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

        return new ImportStreamingModel
        {
            Title = model.Title,
            TwitchChannel = model.TwitchChannel,
            YouTubeChannel = model.YouTubeChannel,
            ScheduleDate = DateOnly.FromDateTime(model.ScheduleDate.Value),
            EndingTime = TimeOnly.FromTimeSpan(model.EndingTime.Value.TimeOfDay),
            TwitchUrl = model.TwitchUrl,
            YouTubeUrl = model.YouTubeUrl,
            Slug = model.Slug,
            StartingTime = TimeOnly.FromTimeSpan(model.StartingTime.Value.TimeOfDay),
            StreamingAbstract = model.StreamingAbstract,
            Seo = model.Seo
        };
    }
}
