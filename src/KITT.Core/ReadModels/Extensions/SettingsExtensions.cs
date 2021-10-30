using KITT.Core.Models;
using System.Linq;

namespace KITT.Core.ReadModels
{
    public static class SettingsExtensions
    {
        public static IQueryable<Settings> ByUserId(this IQueryable<Settings> settings, string userId)
            => settings.Where(s => s.UserId == userId);
    }
}
