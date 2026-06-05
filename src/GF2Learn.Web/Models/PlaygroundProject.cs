namespace GF2Learn.Web.Models;

public sealed class PlaygroundProject
{
    public Guid Id { get; set; }
    public required string UserSub { get; set; }
    public required string Name { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public ICollection<PlaygroundFile> Files { get; set; } = [];
}
