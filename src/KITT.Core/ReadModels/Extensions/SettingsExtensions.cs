namespace KITT.Core.ReadModels;

public static class SettingsExtensions
{
    extension(IQueryable<Settings> settings)
    {
        public IQueryable<Settings> ByUserId(string userId) => settings.Where(s => s.UserId == userId);
    }
}
