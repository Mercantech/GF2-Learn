namespace GF2Learn.Web.Models;

public abstract record ExercisePageBlock;

public sealed record ExerciseMarkdownBlock(string Html) : ExercisePageBlock;

public sealed record ExerciseCardBlock(int PartIndex, string Level, string Html) : ExercisePageBlock;

public sealed record ExerciseEditorBlock(int PartIndex, string Code) : ExercisePageBlock;

public sealed record ExerciseSolutionBlock(string Html) : ExercisePageBlock;
