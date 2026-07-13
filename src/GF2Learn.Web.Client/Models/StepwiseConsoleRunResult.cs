namespace GF2Learn.Web.Client.Models;

public sealed class StepwiseConsoleRunResult
{
    public bool Success { get; init; }
    public string Output { get; init; } = string.Empty;
    public bool NeedsInput { get; init; }
    public string? Error { get; init; }
    public TimeSpan Elapsed { get; init; }
}
