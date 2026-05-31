namespace GF2Learn.Web.Models;

public sealed class KnowledgeCheckAnswer
{
    public long Id { get; set; }
    public required string UserSub { get; set; }
    public required string ContentSlug { get; set; }
    public int QuestionIndex { get; set; }
    public int SelectedIndex { get; set; }
    public bool IsCorrect { get; set; }
    public DateTimeOffset AnsweredAt { get; set; }
}
