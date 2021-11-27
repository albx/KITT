namespace KITT.Web.Models.Settings;

public class SettingsListModel
{
    public IEnumerable<SettingsDescriptor> Items { get; set; } = Array.Empty<SettingsDescriptor>();

    public record SettingsDescriptor
    {
        public Guid Id { get; set; }

        public string TwitchChannel { get; set; } = string.Empty;
    }
}
