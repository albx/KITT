using System.ComponentModel.DataAnnotations;

namespace KITT.Cms.Web.Models.Settings;

public class ChannelModel
{
    public Guid Id { get; set; }

    public ChannelType Type { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Url { get; set; } = string.Empty;
}

public enum ChannelType
{
    Twitch,
    YouTube
}
