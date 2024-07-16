using KITT.Web.App.Clients;
using KITT.Web.Models.Dashboard;
using Microsoft.AspNetCore.Components;

namespace KITT.Web.App.Pages;

public partial class Index
{
    [Inject]
    public IDashboardClient Client { get; set; } = default!;

    private DashboardModel model = new();

    private bool loading = false;

    protected override async Task OnInitializedAsync()
    {
        loading = true;

        try
        {
            model = await Client.GetDashboardStatsAsync();
        }
        finally
        {
            loading = false;
        }
    }
}
