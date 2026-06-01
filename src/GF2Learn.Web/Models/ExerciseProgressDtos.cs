namespace GF2Learn.Web.Models;

public sealed record SaveExercisePartRequest(
    string ContentSlug,
    int PartIndex,
    string? AnswerText);

public sealed record ExercisePartAnswerDto(
    int PartIndex,
    string? AnswerText,
    DateTimeOffset CompletedAt);

public sealed record ExerciseChapterProgress(
    string ContentSlug,
    int TotalParts,
    int CompletedParts,
    bool IsComplete);
