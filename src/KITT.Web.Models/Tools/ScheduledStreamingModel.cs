namespace KITT.Web.Models.Tools;

public record ScheduledStreamingModel
{
    public Guid Id { get; init; }

    public string Title { get; init; } = string.Empty;

    public override string ToString() => Title;
}
