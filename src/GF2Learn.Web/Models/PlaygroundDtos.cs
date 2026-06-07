namespace GF2Learn.Web.Models;

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

public sealed record CreatePlaygroundProjectRequest(string? Name);

public sealed record RenamePlaygroundProjectRequest(string Name);

public sealed record SavePlaygroundFilesRequest(IReadOnlyList<SavePlaygroundFileItem> Files);

public sealed record SavePlaygroundFileItem(
    long? Id,
    string FileName,
    string? Content,
    int SortOrder);

public sealed record CreatePlaygroundFileRequest(string FileName, string? Content);

public sealed record RenamePlaygroundFileRequest(string FileName);

public sealed record FormatCSharpRequest(string? Code);
