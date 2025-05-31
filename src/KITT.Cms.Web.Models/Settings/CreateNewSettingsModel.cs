using System.ComponentModel.DataAnnotations;

namespace KITT.Cms.Web.Models.Settings;

public class CreateNewSettingsModel
{
    [Required]
    public string TwitchChannel { get; set; } = string.Empty;
}
