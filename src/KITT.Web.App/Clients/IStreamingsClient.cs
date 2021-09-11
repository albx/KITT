using KITT.Web.Models.Streamings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KITT.Web.App.Clients
{
    public interface IStreamingsClient
    {
        Task ScheduleStreamingAsync(ScheduleStreamingModel model);

        Task<StreamingsListModel> GetAllStreamingsAsync();
    }
}
