using System.ComponentModel.DataAnnotations;

namespace KITT.Bot.Functions.Models;

public record TextOverlay
{
    [Required]
    public string UserName { get; init; } = string.Empty;

    [Required]
    public string Message { get; init; } = string.Empty;
}
