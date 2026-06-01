namespace GF2Learn.Web.Client.Models;

public sealed record ExercisePartAnswerDto(int PartIndex, string? AnswerText, DateTimeOffset CompletedAt);
