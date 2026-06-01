namespace GF2Learn.Web.Models;

public sealed class ExerciseAnswer
{
    public long Id { get; set; }
    public required string UserSub { get; set; }
    public required string ContentSlug { get; set; }
    public int PartIndex { get; set; }
    public string? AnswerText { get; set; }
    public DateTimeOffset CompletedAt { get; set; }
}
