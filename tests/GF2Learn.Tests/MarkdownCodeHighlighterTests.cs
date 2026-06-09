using System.Net;
using GF2Learn.Web.Services;

namespace GF2Learn.Tests;

public sealed class MarkdownCodeHighlighterTests
{
    [Fact]
    public void Apply_AddsHljsSpansToCSharpBlock()
    {
        var highlighter = new MarkdownCodeHighlighter();
        var html = $"""
            <pre><code class="language-csharp">{WebUtility.HtmlEncode("bool ok = true;\nConsole.WriteLine(\"Hi\");")}</code></pre>
            """;

        var result = highlighter.Apply(html);

        Assert.Contains("class=\"hljs language-csharp\"", result, StringComparison.Ordinal);
        Assert.Contains("data-highlighted=\"server\"", result, StringComparison.Ordinal);
        Assert.Contains("<span class=\"hljs-keyword\">bool</span>", result, StringComparison.Ordinal);
        Assert.Contains("<span class=\"hljs-built_in\">Console.WriteLine</span>", result, StringComparison.Ordinal);
        Assert.Contains("<span class=\"hljs-string\">&quot;Hi&quot;</span>", result, StringComparison.Ordinal);
    }

    [Fact]
    public void Apply_LeavesUnknownLanguagesUntouched()
    {
        var highlighter = new MarkdownCodeHighlighter();
        var html = "<pre><code class=\"language-python\">print(1)</code></pre>";

        var result = highlighter.Apply(html);

        Assert.Equal(html, result);
    }

    [Fact]
    public void HighlightSource_HighlightsInterpolatedStrings()
    {
        var highlighter = new MarkdownCodeHighlighter();
        var result = highlighter.HighlightSource("Console.WriteLine($\"{i + 1}.\");");

        Assert.Contains("<span class=\"hljs-built_in\">Console.WriteLine</span>", result, StringComparison.Ordinal);
        Assert.Contains("<span class=\"hljs-string\">$&quot;{i + 1}.&quot;</span>", result, StringComparison.Ordinal);
    }
}
