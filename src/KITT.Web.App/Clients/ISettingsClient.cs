using KITT.Web.Models.Settings;

namespace KITT.Web.App.Clients;

public interface ISettingsClient
{
    Task<SettingsListModel> GetAllSettingsAsync();

    Task CreateNewSettingsAsync(CreateNewSettingsModel model);
}
