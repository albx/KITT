using KITT.Web.Models.Tools;

namespace LemonBot.Web.Areas.Tools.Services
{
    public interface IBotHttpClient
    {
        Task<BotJobDetail> GetDetailAsync();
        Task StartAsync();
        Task StopAsync();
    }
}