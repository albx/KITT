using System.ComponentModel.DataAnnotations;

namespace KITT.Web.Models.Settings
{
    public class CreateNewSettingsModel
    {
        [Required]
        public string TwitchChannel { get; set; }
    }
}
