using KITT.Web.App.Model;
using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;

namespace KITT.Web.App.Components;

public partial class ScheduleForm
{
    [Parameter]
    public ViewModel Model { get; set; } = new();

    [Parameter]
    public EventCallback<ViewModel> OnSave { get; set; }

    [Parameter]
    public EventCallback OnCancel { get; set; }

    private async Task SubmitAsync() => await OnSave.InvokeAsync(Model);

    private async Task CancelAsync()
    {
        Model = new();
        await OnCancel.InvokeAsync();
    }

    public class ViewModel : ContentViewModel
    {
        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
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
    }
}
