using KITT.Web.App.Tools.Clients;
using KITT.Web.Models.Tools;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;
using System.ComponentModel.DataAnnotations;

namespace KITT.Web.App.Tools.Pages;

public partial class Index
{
    [Inject]
    public IBotClient Client { get; set; } = default!;

    [Inject]
    public IStreamingsClient StreamingsClient { get; set; } = default!;

    [Inject]
    public NavigationManager Navigation { get; set; } = default!;

    [Inject]
    public IRealtimeClient RealtimeClient { get; set; } = default!;

    [Inject]
    ISnackbar Snackbar { get; set; } = default!;

    [Inject]
    public IStringLocalizer<Resources.Pages.Index> Localizer { get; set; } = default!;

    private string message = string.Empty;
    private Severity messageSeverity = Severity.Info;

    private bool isBotRunning = false;
    private bool discoveringBotStatus = false;

    private ViewModel model = new();
    private IEnumerable<ScheduledStreamingModel> scheduledStreamings = Array.Empty<ScheduledStreamingModel>();
    private bool isLoadingStreamings = true;

    private bool isSavingStats = false;

    private int userJoinedNumber = 0;
    private int userLeftNumber = 0;
    private HashSet<string> viewers = new();
    private HashSet<string> subscribers = new();

    async Task SaveStreamingStatsAsync()
    {
        isSavingStats = true;

        try
        {
            if (model.CurrentStreaming is null)
            {
                Snackbar.Add(Localizer[nameof(Resources.Pages.Index.SaveStreamingStatsMissingStreamingMessage)], Severity.Warning);
                return;
            }

            await StreamingsClient.SaveStreamingStatsAsync(
                model.CurrentStreaming.Id,
                new StreamingStats(viewers.Count, subscribers.Count, userJoinedNumber, userLeftNumber));

            Snackbar.Add(Localizer[nameof(Resources.Pages.Index.SaveStreamingStatsSuccessMessage)], Severity.Success);
        }
        catch
        {
            Snackbar.Add(Localizer[nameof(Resources.Pages.Index.SaveStreamingStatsErrorMessage)], Severity.Error);
        }
        finally
        {
            isSavingStats = false;
        }
    }

    async Task StartBotAsync()
    {
        discoveringBotStatus = true;

        try
        {
            await Client.StartAsync();
            message = Localizer[nameof(Resources.Pages.Index.BotStartingMessage)];
            messageSeverity = Severity.Success;
        }
        catch (Exception ex)
        {
            message = ex.Message;
            messageSeverity = Severity.Warning;
        }
        finally
        {
            discoveringBotStatus = false;
            await LoadDetails();
        }
    }

    async Task StopBotAsync()
    {
        discoveringBotStatus = true;

        try
        {
            await Client.StopAsync();
            message = Localizer[nameof(Resources.Pages.Index.BotStatusDefaultMessage)];
            messageSeverity = Severity.Info;
        }
        catch (Exception ex)
        {
            message = ex.Message;
            messageSeverity = Severity.Warning;
        }
        finally
        {
            discoveringBotStatus = false;
            await LoadDetails();
        }
    }

    private async Task LoadDetails()
    {
        var jobDetails = await Client.GetDetailsAsync();
        isBotRunning = jobDetails?.IsRunning ?? false;
    }

    private async Task LoadScheduledStreamings()
    {
        try
        {
            scheduledStreamings = await StreamingsClient.GetScheduledStreamingsAsync();
            model.CurrentStreaming = scheduledStreamings.FirstOrDefault(s => s.ScheduleDate == DateTime.Today);
        }
        finally
        {
            isLoadingStreamings = false;
        }
    }

    protected override async Task OnInitializedAsync()
    {
        message = Localizer[nameof(Resources.Pages.Index.BotStatusDefaultMessage)];

        await base.OnInitializedAsync();
        await LoadScheduledStreamings();

        await LoadDetails();

        await InitializeSignalRConnection();
    }

    private async Task InitializeSignalRConnection()
    {
        RealtimeClient.ConnectToEndpoint(Navigation.ToAbsoluteUri("/bot"));

        RealtimeClient.On("BotStarted", () =>
        {
            message = Localizer[nameof(Resources.Pages.Index.BotRunningMessage)];
            messageSeverity = Severity.Success;
            isBotRunning = true;

            StateHasChanged();
        });

        RealtimeClient.On("UserJoinReceived", (string username) =>
        {
            userJoinedNumber++;
            viewers.Add(username);

            StateHasChanged();
        });

        RealtimeClient.On("UserLeftReceived", (string username) =>
        {
            if (viewers.Contains(username))
            {
                viewers.Remove(username);
            }

            userLeftNumber++;
            StateHasChanged();
        });

        RealtimeClient.On("UserSubscriptionReceived", (string username) =>
        {
            subscribers.Add(username);
            StateHasChanged();
        });

        await RealtimeClient.StartAsync();
    }

    class ViewModel
    {
        [Required]
        public ScheduledStreamingModel? CurrentStreaming { get; set; }
    }
}
