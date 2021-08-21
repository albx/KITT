using KITT.Web.Models.Lives;
using System;
using System.Threading.Tasks;

namespace LemonBot.Web.Services
{
    public class LivesControllerServices
    {
        public LiveListModel GetAllLives()
        {
            var model = new LiveListModel();
            model.Items = new[]
            {
                new LiveListModel.LiveListItemModel { Id = Guid.NewGuid(), Title = "KITT - Streaming tools", ScheduledOn = DateTime.Today, StartingTime = TimeSpan.FromHours(16), EndingTime = TimeSpan.FromHours(18), TwitchChannelUrl = "https://www.twitch.tv/albx87" }
            };

            return model;
        }

        public object GetLiveDetail(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<Guid> CreateNewLiveAsync(CreateLiveModel model)
        {
            throw new NotImplementedException();
        }
    }
}
