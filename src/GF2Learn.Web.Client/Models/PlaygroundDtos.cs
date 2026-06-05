namespace GF2Learn.Web.Client.Models;

public sealed record PlaygroundProjectSummaryDto(
    Guid Id,
    string Name,
    DateTimeOffset UpdatedAt,
    int FileCount);

public sealed record PlaygroundFileDto(
    long Id,
    string FileName,
    string? Content,
    int SortOrder,
    DateTimeOffset UpdatedAt);

public sealed record PlaygroundProjectDetailDto(
    Guid Id,
    string Name,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    IReadOnlyList<PlaygroundFileDto> Files);

public sealed record SavePlaygroundFileItem(
    long? Id,
    string FileName,
    string? Content,
    int SortOrder);

public sealed record ProjectSourceFile(string FileName, string Content);
