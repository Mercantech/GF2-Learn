namespace GF2Learn.Web.Client.Models;

public abstract record ExercisePageSegment;

public sealed record HtmlSegment(string Html) : ExercisePageSegment;

public sealed record ExercisePartSegment(string CardHtml, int PartIndex, string Code) : ExercisePageSegment;
