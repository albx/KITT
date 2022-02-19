using KITT.Web.App.Clients;
using KITT.Web.Models.Tools;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Localization;
using MudBlazor;

namespace KITT.Web.App.Pages.Tools;

public partial class Index
{
    [Inject]
    public IBotClient Client { get; set; }

    [Inject]
    public Clients.Tools.IStreamingsClient StreamingsClient { get; set; }

    [Inject]
    public NavigationManager Navigation { get; set; }

    [Inject]
    public IStringLocalizer<Resources.Common> CommonLocalizer { get; set; }

    [Inject]
    public IStringLocalizer<Resources.Pages.Tools.Index> Localizer { get; set; }

    private string message = string.Empty;
    private Severity messageSeverity = Severity.Info;

    private HubConnection? connection;
    private bool isBotRunning = false;
    private bool discoveringBotStatus = false;

    private ScheduledStreamingModel? currentStreaming = null;
    private IEnumerable<ScheduledStreamingModel> scheduledStreamings = Array.Empty<ScheduledStreamingModel>();
    private bool isLoadingStreamings = true;

    private bool isSavingStats = false;

    async Task StartBotAsync()
    {
        discoveringBotStatus = true;

        try
        {
            await Client.StartAsync();
            message = Localizer[nameof(Resources.Pages.Tools.Index.BotStartingMessage)];
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
            message = Localizer[nameof(Resources.Pages.Tools.Index.BotStatusDefaultMessage)];
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
        message = Localizer[nameof(Resources.Pages.Tools.Index.BotStatusDefaultMessage)];

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
            message = Localizer[nameof(Resources.Pages.Tools.Index.BotRunningMessage)];
            messageSeverity = Severity.Success;
            isBotRunning = true;

            StateHasChanged();
        });

        await connection.StartAsync();
    }
}
