using KITT.Web.App.Clients;
using KITT.Web.App.Model;
using KITT.Web.Models.Streamings;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.ComponentModel.DataAnnotations;

namespace KITT.Web.App.Pages.Streamings;

public partial class Import
{
    [Inject]
    public IStreamingsClient Client { get; set; } = default!;

    [Inject]
    public NavigationManager Navigation { get; set; } = default!;

    [Inject]
    ISnackbar Snackbar { get; set; } = default!;

    private ViewModel model = new();

    private string errorMessage = string.Empty;

    async Task ImportStreamingAsync(ViewModel model)
    {
        try
        {
            await Client.ImportStreamingAsync(model.ToApiModel());
            Snackbar.Add(Localizer[nameof(Resources.Pages.Streamings.Import.StreamingImportedSuccessfully), model.Title], Severity.Success);

            Navigation.NavigateTo("/streamings");
        }
        catch (ApplicationException ex)
        {
            errorMessage = ex.Message;
        }
    }

    void Cancel() => model = new();

    class ViewModel : ContentViewModel
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

        public ImportStreamingModel ToApiModel()
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
                EndingTime = this.EndingTime.Value,
                HostingChannelUrl = $"https://www.twitch.tv/{this.HostingChannelUrl}",
                Slug = this.Slug,
                StartingTime = this.StartingTime.Value,
                StreamingAbstract = this.StreamingAbstract,
                YoutubeVideoUrl = this.YoutubeVideoUrl,
                Seo = this.Seo
            };
        }
    }
}