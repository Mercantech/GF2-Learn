using GF2Learn.Web.Services;

namespace GF2Learn.Tests;

public sealed class ExerciseAiContextServiceTests
{
    [Fact]
    public void ParseParts_ExtractsExercisePlaygroundAndSolutionContext()
    {
        var markdown = """
            :::exercise
            **Opgave 1:** Lav en variabel `age`.
            :::

            :::code-playground
            ```csharp
            int age = 0;
            ```
            :::

            :::solution
            ```csharp
            int age = 10;
            Console.WriteLine(age);
            ```
            :::
            """;
        var service = new ExerciseAiContextService(content: null!);

        var part = Assert.Single(service.ParseParts(markdown));

        Assert.Equal(0, part.PartIndex);
        Assert.Equal("Opgave 1: Lav en variabel  age .", part.TaskDescription);
        Assert.Equal("int age = 0;", part.StarterCode);
        Assert.Equal("int age = 10;\nConsole.WriteLine(age);", part.ReferenceSolutionCode);
    }

    [Fact]
    public void ParseParts_IgnoresExerciseWithoutFollowingPlayground()
    {
        var markdown = """
            :::exercise
            Mangler editor.
            :::

            :::solution
            Console.WriteLine("ok");
            :::
            """;
        var service = new ExerciseAiContextService(content: null!);

        Assert.Empty(service.ParseParts(markdown));
    }
}
