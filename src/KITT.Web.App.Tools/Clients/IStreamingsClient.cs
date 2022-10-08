﻿using KITT.Web.Models.Tools;

namespace KITT.Web.App.Tools.Clients;

public interface IStreamingsClient
{
    Task<IEnumerable<ScheduledStreamingModel>> GetScheduledStreamingsAsync();
    Task SaveStreamingStatsAsync(Guid id, StreamingStats streamingStats);
}
