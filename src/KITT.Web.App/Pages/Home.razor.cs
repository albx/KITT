using KITT.Web.App.Models;

namespace KITT.Web.App.Pages;

public partial class Home
{
    //[Inject]
    //public IDashboardClient Client { get; set; } = default!;

    private DashboardModel model = new();

    private bool loading = false;

    protected override async Task OnInitializedAsync()
    {
        loading = true;

        try
        {
            //model = await Client.GetDashboardStatsAsync();
        }
        finally
        {
            loading = false;
        }
    }
}
