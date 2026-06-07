using GF2Learn.Web.Services;

namespace GF2Learn.Tests;

public sealed class RunnableCodeParserTests
{
    [Fact]
    public void IsRunnableSnippet_ReturnsTrueForSimpleConsoleSnippet()
    {
        var code = """
            var number = 10;
            Console.WriteLine(number);
            """;

        Assert.True(RunnableCodeParser.IsRunnableSnippet(code));
    }

    [Fact]
    public void IsRunnableSnippet_ReturnsFalseForFullProgram()
    {
        var code = """
            class Program
            {
                static void Main()
                {
                    Console.WriteLine("Hej");
                }
            }
            """;

        Assert.False(RunnableCodeParser.IsRunnableSnippet(code));
    }

    [Fact]
    public void BuildSegments_ReplacesRunnableCSharpCodeBlockWithSnippetSegment()
    {
        var parser = new RunnableCodeParser();
        var html = """
            <p>For</p>
            <pre><code class="language-csharp">Console.WriteLine(&quot;Hej&quot;);</code></pre>
            <p>Efter</p>
            """;

        var segments = parser.BuildSegments(html);

        Assert.Equal(3, segments.Count);
        Assert.Equal("<p>For</p>", segments[0].Html);
        Assert.NotNull(segments[1].RunnableSnippet);
        Assert.Equal("Console.WriteLine(\"Hej\");", segments[1].RunnableSnippet!.Code);
        Assert.Equal("<p>Efter</p>", segments[2].Html);
    }
}
