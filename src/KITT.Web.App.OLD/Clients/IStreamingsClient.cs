using KITT.Web.Models.Streamings;

namespace KITT.Web.App.Clients;

public interface IStreamingsClient
{
    Task ScheduleStreamingAsync(ScheduleStreamingModel model);

    Task<StreamingsListModel> GetAllStreamingsAsync(StreamingQueryModel query);

    Task<StreamingDetailModel?> GetStreamingDetailAsync(Guid streamingId);

    Task UpdateStreamingAsync(StreamingDetailModel model);

    Task DeleteStreamingAsync(Guid streamingId);

    Task ImportStreamingAsync(ImportStreamingModel model);
}
