using KITT.Web.Models.Lives;
using System;
using System.Threading.Tasks;

namespace LemonBot.Web.Services
{
    public class StreamingsControllerServices
    {
        public StreamingsListModel GetAllStreamings()
        {
            var model = new StreamingsListModel();
            model.Items = new[]
            {
                new StreamingsListModel.StreamingListItemModel { Id = Guid.NewGuid(), Title = "KITT - Streaming tools", ScheduledOn = DateTime.Today, StartingTime = TimeSpan.FromHours(16), EndingTime = TimeSpan.FromHours(18), TwitchChannelUrl = "https://www.twitch.tv/albx87" }
            };

            return model;
        }

        public object GetStreamingDetail(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<Guid> ScheduleStreamingAsync(ScheduleStreamingModel model)
        {
            throw new NotImplementedException();
        }
    }
}
