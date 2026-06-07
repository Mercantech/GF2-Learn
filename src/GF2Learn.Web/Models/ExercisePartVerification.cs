namespace GF2Learn.Web.Models;

public sealed class ExercisePartVerification
{
    public long Id { get; set; }
    public required string UserSub { get; set; }
    public required string ContentSlug { get; set; }
    public int PartIndex { get; set; }
    public bool IsSolved { get; set; }
    public DateTimeOffset VerifiedAt { get; set; }
}
