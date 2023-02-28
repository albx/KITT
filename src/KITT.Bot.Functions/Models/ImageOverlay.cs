using System.ComponentModel.DataAnnotations;

namespace KITT.Bot.Functions.Models;

public record ImageOverlay
{
    [Required]
    public string ResourceUrl { get; init; } = string.Empty;
}
