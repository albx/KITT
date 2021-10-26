using KITT.Core.Models;
using KITT.Core.Persistence;
using System;
using System.Threading.Tasks;

namespace KITT.Core.Commands
{
    public class SettingsCommands : ISettingsCommands
    {
        private readonly KittDbContext _context;

        public SettingsCommands(KittDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Guid> CreateNewSettingsAsync(string userId, string twitchChannel)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new ArgumentException($"'{nameof(userId)}' cannot be null or whitespace.", nameof(userId));
            }

            if (string.IsNullOrWhiteSpace(twitchChannel))
            {
                throw new ArgumentException($"'{nameof(twitchChannel)}' cannot be null or whitespace.", nameof(twitchChannel));
            }

            var settings = Settings.CreateNew(userId, twitchChannel);
            _context.Add(settings);

            await _context.SaveChangesAsync();

            return settings.Id;
        }
    }
}
