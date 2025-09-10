using KITT.Cms.Web.Models.Settings;
using KITT.Core.Commands;
using KITT.Core.ReadModels;
using Microsoft.EntityFrameworkCore;

namespace KITT.Cms.Web.Api.Endpoints.Services;

public class SettingsEndpointsServices
{
    public IDatabase Database { get; }
    public ISettingsCommands Commands { get; }

    public SettingsEndpointsServices(IDatabase database, ISettingsCommands commands)
    {
        Database = database ?? throw new ArgumentNullException(nameof(database));
        Commands = commands ?? throw new ArgumentNullException(nameof(commands));
    }

    public async Task<SettingsListModel> GetAllSettingsAsync(string userId)
    {
        var settings = await Database.Settings
            .ByUserId(userId)
            .OrderBy(s => s.TwitchChannel)
            .Select(s => new SettingsListModel.SettingsDescriptor
            {
                Id = s.Id,
                TwitchChannel = s.TwitchChannel
            }).ToArrayAsync();

        var model = new SettingsListModel { Items = settings };
        return model;
    }

    public Task<Guid> CreateNewSettingsAsync(CreateNewSettingsModel model, string userId)
    {
        return Commands.CreateNewSettingsAsync(userId, model.TwitchChannel);
    }

    public async Task<SettingsDetailModel?> GetSettingsDetailAsync(Guid settingsId)
    {
        var settings = await Database.Settings.SingleOrDefaultAsync(s => s.Id == settingsId);
        if (settings is null)
        {
            return null;
        }

        return new SettingsDetailModel
        {
            Id = settings.Id,
            TwitchChannel = settings.TwitchChannel
        };
    }
}
