namespace GF2Learn.Web.Models;

public sealed record ExerciseAiRequest(
    string ContentSlug,
    int PartIndex,
    string StudentCode,
    string? ConsoleOutput);

public sealed record ExerciseAiHintResponse(string Message);

public sealed record ExerciseAiCheckResponse(bool IsSolved, string Feedback);

public sealed record ExerciseAiStatusResponse(bool Enabled, string? Message);

public sealed record ExercisePartAiContext(
    int PartIndex,
    string TaskDescription,
    string StarterCode,
    string? ReferenceSolutionCode);
