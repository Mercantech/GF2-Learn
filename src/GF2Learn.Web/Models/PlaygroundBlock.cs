namespace GF2Learn.Web.Models;

public sealed class PlaygroundBlock
{
    public required string Code { get; init; }
    public string Expected { get; init; } = string.Empty;
    public IReadOnlyList<string> Refs { get; init; } = [];
}

public sealed class ContentSegment
{
    public string? Html { get; init; }
    public PlaygroundBlock? Playground { get; init; }
    public bool IsPlayground => Playground is not null;
}
