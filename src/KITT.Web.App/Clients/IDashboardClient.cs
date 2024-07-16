using KITT.Web.Models.Dashboard;

namespace KITT.Web.App.Clients;

public interface IDashboardClient
{
    Task<DashboardModel> GetDashboardStatsAsync();
}
