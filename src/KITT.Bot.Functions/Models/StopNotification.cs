using System;

namespace KITT.Bot.Functions.Models;

public record StopNotification
{
    public DateTime StopTime { get; init; }
}
