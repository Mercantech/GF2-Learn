namespace GF2Learn.Web.Client.Models;

public sealed class RunResult
{
    public bool Success { get; init; }
    public string Output { get; init; } = string.Empty;
    public string? Error { get; init; }
    public bool? ExpectedMatch { get; init; }
    public TimeSpan Elapsed { get; init; }
}
