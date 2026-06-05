namespace GF2Learn.Web.Models;

public sealed class PlaygroundFile
{
    public long Id { get; set; }
    public Guid ProjectId { get; set; }
    public required string FileName { get; set; }
    public string? Content { get; set; }
    public int SortOrder { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public PlaygroundProject? Project { get; set; }
}
