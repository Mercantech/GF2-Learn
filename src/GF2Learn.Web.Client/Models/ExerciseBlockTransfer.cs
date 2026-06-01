namespace GF2Learn.Web.Client.Models;

public sealed record ExerciseBlockTransfer(string Kind, int PartIndex, string Primary, string? Secondary = null);
