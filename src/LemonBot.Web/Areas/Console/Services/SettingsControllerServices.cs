using KITT.Core.Commands;
using KITT.Core.ReadModels;
using KITT.Web.Models.Settings;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LemonBot.Web.Areas.Console.Services
{
    public class SettingsControllerServices
    {
        public IDatabase Database { get; }
        public ISettingsCommands Commands { get; }

        public SettingsControllerServices(IDatabase database, ISettingsCommands commands)
        {
            Database = database ?? throw new ArgumentNullException(nameof(database));
            Commands = commands ?? throw new ArgumentNullException(nameof(commands));
        }

        public SettingsListModel GetAllSettings(string userId)
        {
            var settings = Database.Settings
                .ByUserId(userId)
                .OrderBy(s => s.TwitchChannel)
                .Select(s => new SettingsListModel.SettingsDescriptor
                {
                    Id = s.Id,
                    TwitchChannel = s.TwitchChannel
                }).ToArray();

            var model = new SettingsListModel { Items = settings };
            return model;
        }

        public Task<Guid> CreateNewSettingsAsync(CreateNewSettingsModel model, string userId)
        {
            return Commands.CreateNewSettingsAsync(userId, model.TwitchChannel);
        }
    }
}
