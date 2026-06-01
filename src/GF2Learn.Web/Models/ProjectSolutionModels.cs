namespace GF2Learn.Web.Models;

public sealed record DeliverableItem(
    string Title,
    string Description,
    bool Required = true);

public sealed record DeliverableExpectations(
    IReadOnlyList<DeliverableItem> Items,
    string? Notes = null);

public sealed record ProjectSolutionInfo(
    string Slug,
    DeliverableExpectations Deliverables,
    string? ConsoleDemoCode,
    Type? MvpBlazorComponent,
    Type? ExtendedBlazorComponent);
