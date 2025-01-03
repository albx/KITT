using KITT.Web.App.Client.Clients;
using KITT.Web.Models.Dashboard;
using Microsoft.AspNetCore.Components;

namespace KITT.Web.App.Client.Pages;

public partial class Home
{

    private DashboardModel model = new();

    private bool loading = false;

    protected override async Task OnInitializedAsync()
    {
        //loading = true;

        //try
        //{
        //    model = await Client.GetDashboardStatsAsync();
        //}
        //finally
        //{
        //    loading = false;
        //}
    }
}
