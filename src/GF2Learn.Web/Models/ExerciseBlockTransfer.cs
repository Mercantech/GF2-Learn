namespace GF2Learn.Web.Models;

/// <summary>Serializable exercise page block for passing to WASM interactive host.</summary>
public sealed record ExerciseBlockTransfer(string Kind, int PartIndex, string Primary, string? Secondary = null);
