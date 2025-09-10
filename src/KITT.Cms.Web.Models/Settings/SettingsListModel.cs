namespace KITT.Cms.Web.Models.Settings;

public class SettingsListModel
{
    public IEnumerable<SettingsDescriptor> Items { get; set; } = [];

    public record SettingsDescriptor
    {
        public Guid Id { get; set; }

        public string TwitchChannel { get; set; } = string.Empty;
    }
}
