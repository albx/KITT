using KITT.Web.Models.Settings;
using System.Threading.Tasks;

namespace KITT.Web.App.Clients
{
    public interface ISettingsClient
    {
        Task<SettingsListModel> GetAllSettingsAsync();

        Task CreateNewSettingsAsync(CreateNewSettingsModel model);
    }
}
