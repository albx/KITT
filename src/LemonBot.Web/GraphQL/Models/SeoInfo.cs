namespace LemonBot.Web.GraphQL.Models;

public record SeoInfo
{
    public string Title { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public string Keywords { get; init; } = string.Empty;
}
