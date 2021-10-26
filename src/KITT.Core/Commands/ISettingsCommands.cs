using System;
using System.Threading.Tasks;

namespace KITT.Core.Commands
{
    public interface ISettingsCommands
    {
        Task<Guid> CreateNewSettingsAsync(string userId, string twitchChannel);
    }
}
