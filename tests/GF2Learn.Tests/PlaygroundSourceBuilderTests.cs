using GF2Learn.Web.Client.Services;

namespace GF2Learn.Tests;

public sealed class PlaygroundSourceBuilderTests
{
    [Fact]
    public void ExtractSetup_RemovesSetupDirectivesAndReturnsSetupLines()
    {
        var code = """
            // gf2-setup: using System.Globalization;
            Console.WriteLine("Hej");
            """;

        var (setupLines, body) = PlaygroundSourceBuilder.ExtractSetup(code);

        Assert.Equal(["using System.Globalization;"], setupLines);
        Assert.Equal("Console.WriteLine(\"Hej\");", body);
    }

    [Fact]
    public void Prepare_ParsesNamedSimulatedInputAndPrompt()
    {
        var code = """
            // gf2-input: name: Ada
            Console.Write("Navn?");
            var name = Console.ReadLine();
            Console.WriteLine(name);
            """;

        var fields = PlaygroundSourceBuilder.GetSimulatedStdinFields(code);
        var prepared = PlaygroundSourceBuilder.Prepare(code);

        var field = Assert.Single(fields);
        Assert.Equal("Ada", field.Value);
        Assert.Equal("name", field.VariableName);
        Assert.Equal("Navn?", field.Prompt);
        Assert.Equal(["Ada"], prepared.StdinLines);
        Assert.True(prepared.UsesReadLine);
        Assert.Contains("__ReadLine()", prepared.ExecutableCode);
        Assert.DoesNotContain("Console.ReadLine", prepared.ExecutableCode);
    }

    [Fact]
    public void Prepare_DefaultsReadLineInputWhenNoDirectiveIsProvided()
    {
        var prepared = PlaygroundSourceBuilder.Prepare("var age = Console.ReadLine();");

        Assert.Equal(["15"], prepared.StdinLines);
        Assert.True(prepared.UsesReadLine);
    }
}
