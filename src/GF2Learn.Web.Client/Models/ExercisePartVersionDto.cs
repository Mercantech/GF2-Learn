using System.Text.Json.Serialization;

namespace GF2Learn.Web.Client.Models;

public sealed record ExercisePartVersionDto(
    [property: JsonPropertyName("id")] long Id,
    [property: JsonPropertyName("answerText")] string? AnswerText,
    [property: JsonPropertyName("savedAt")] DateTimeOffset SavedAt);
