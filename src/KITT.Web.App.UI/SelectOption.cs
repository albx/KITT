namespace KITT.Web.App.UI;

/// <summary>
/// Lightweight replacement for the v4 <c>Option&lt;T&gt;</c> type (removed in Fluent UI Blazor v5),
/// used to build <c>Items</c> lists for <c>FluentSelect</c>/<c>FluentCombobox</c> components.
/// </summary>
public record SelectOption<T>
{
    public required T Value { get; init; }

    public required string Text { get; init; }
}
