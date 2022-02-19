using KITT.Web.Models.Tools;

namespace KITT.Web.App.Clients.Tools;

public interface IStreamingsClient
{
    Task<IEnumerable<ScheduledStreamingModel>> GetScheduledStreamingsAsync();
}
