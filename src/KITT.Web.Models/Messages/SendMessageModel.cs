using System.ComponentModel.DataAnnotations;

namespace KITT.Web.Models.Messages;

public record SendMessageModel
{
    [Required]
    public string Text { get; set; } = string.Empty;
}
