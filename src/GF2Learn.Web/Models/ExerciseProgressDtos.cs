namespace GF2Learn.Web.Models;

public sealed record SaveExercisePartRequest(
    string ContentSlug,
    int PartIndex,
    string? AnswerText);

public sealed record SaveExerciseVerificationRequest(
    string ContentSlug,
    int PartIndex,
    bool IsSolved);

public sealed record ExercisePartAnswerDto(
    int PartIndex,
    string? AnswerText,
    DateTimeOffset CompletedAt);

public sealed record ExercisePartVersionDto(
    long Id,
    string? AnswerText,
    DateTimeOffset SavedAt);

public sealed record ExercisePartVerificationDto(
    int PartIndex,
    bool IsSolved,
    DateTimeOffset VerifiedAt);

public sealed record ExerciseChapterProgress(
    string ContentSlug,
    int TotalParts,
    int CompletedParts,
    bool IsComplete);
