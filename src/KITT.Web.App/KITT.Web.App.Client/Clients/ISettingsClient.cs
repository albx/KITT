using KITT.Cms.Web.Models.Settings;

namespace KITT.Web.App.Client.Clients;

public interface ISettingsClient
{
    Task<SettingsListModel> GetAllSettingsAsync();

    Task CreateNewSettingsAsync(CreateNewSettingsModel model);
}
