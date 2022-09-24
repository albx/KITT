using KITT.Web.App.Tools.Clients;
using KITT.Web.Models.Tools;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Localization;
using MudBlazor;

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
    public IStringLocalizer<Resources.Pages.Index> Localizer { get; set; } = default!;

    private string message = string.Empty;
    private Severity messageSeverity = Severity.Info;

    private HubConnection? connection;
    private bool isBotRunning = false;
    private bool discoveringBotStatus = false;

    private ScheduledStreamingModel? currentStreaming = null;
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
            if (currentStreaming is null)
            {
                //errore
                return;
            }

            await StreamingsClient.SaveStreamingStatsAsync(
                currentStreaming.Id,
                new StreamingStats(viewers.Count, subscribers.Count, userJoinedNumber, userLeftNumber));

            //messaggio di salvataggio completato
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
            currentStreaming = scheduledStreamings.FirstOrDefault(s => s.ScheduleDate == DateTime.Today);
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
        connection = new HubConnectionBuilder()
            .WithUrl(Navigation.ToAbsoluteUri("/bot"))
            .Build();

        connection.On("BotStarted", () =>
        {
            message = Localizer[nameof(Resources.Pages.Index.BotRunningMessage)];
            messageSeverity = Severity.Success;
            isBotRunning = true;

            StateHasChanged();
        });

        connection.On("UserJoinReceived", (string username) =>
        {
            userJoinedNumber++;
            viewers.Add(username);

            StateHasChanged();
        });

        connection.On("UserLeftReceived", (string username) =>
        {
            if (viewers.Contains(username))
            {
                viewers.Remove(username);
            }

            userLeftNumber++;
            StateHasChanged();
        });

        connection.On("UserSubscriptionReceived", (string username) =>
        {
            subscribers.Add(username);
            StateHasChanged();
        });

        await connection.StartAsync();
    }
}
