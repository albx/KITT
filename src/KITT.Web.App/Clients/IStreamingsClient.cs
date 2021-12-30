using KITT.Web.Models.Streamings;

namespace KITT.Web.App.Clients;

public interface IStreamingsClient
{
    Task ScheduleStreamingAsync(ScheduleStreamingModel model);

    Task<StreamingsListModel> GetAllStreamingsAsync();

    Task<StreamingDetailModel?> GetStreamingDetailAsync(Guid streamingId);

    Task UpdateStreamingAsync(StreamingDetailModel model);

    Task DeleteStreamingAsync(Guid streamingId);
}
