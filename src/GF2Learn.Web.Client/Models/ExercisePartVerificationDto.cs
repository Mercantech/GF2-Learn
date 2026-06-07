using System.Text.Json.Serialization;

namespace GF2Learn.Web.Client.Models;

public sealed record ExercisePartVerificationDto(
    [property: JsonPropertyName("partIndex")] int PartIndex,
    [property: JsonPropertyName("isSolved")] bool IsSolved,
    [property: JsonPropertyName("verifiedAt")] DateTimeOffset VerifiedAt);
