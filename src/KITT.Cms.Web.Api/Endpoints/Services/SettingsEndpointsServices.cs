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
}
