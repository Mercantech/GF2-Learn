using System.Net;
using System.Text.RegularExpressions;

namespace GF2Learn.Web.Services;

/// <summary>
/// Server-side syntax highlighting for fenced code blocks in markdown HTML.
/// Outputs highlight.js-compatible spans so pensum code is colored on first paint.
/// </summary>
public sealed partial class MarkdownCodeHighlighter
{
    private static readonly string[] CSharpKeywords =
    [
        "bool", "byte", "char", "class", "const", "decimal", "double", "else", "false", "for",
        "foreach", "if", "in", "int", "long", "new", "null", "public", "private", "protected",
        "return", "static", "string", "switch", "this", "true", "try", "catch", "finally",
        "throw", "using", "var", "void", "while", "do", "break", "continue", "namespace"
    ];

    [GeneratedRegex(
        @"<pre><code(\s+class=""([^""]*)"")?>([\s\S]*?)</code></pre>",
        RegexOptions.IgnoreCase)]
    private static partial Regex FencedCodeRegex();

    public string Apply(string html)
    {
        if (string.IsNullOrEmpty(html))
            return html;

        return FencedCodeRegex().Replace(html, static match =>
        {
            var classes = match.Groups[2].Success ? match.Groups[2].Value : "";
            var lang = ParseLanguage(classes);
            if (lang is null)
                return match.Value;

            var source = WebUtility.HtmlDecode(match.Groups[3].Value);
            var body = lang switch
            {
                "csharp" => HighlightCSharp(source),
                "bash" => HighlightBash(source),
                _ => WebUtility.HtmlEncode(source)
            };

            return $"<pre><code class=\"hljs language-{lang}\" data-highlighted=\"server\">{body}</code></pre>";
        });
    }

    private static string? ParseLanguage(string classAttr)
    {
        var match = Regex.Match(classAttr, @"language-([\w+#-]+)", RegexOptions.IgnoreCase);
        if (!match.Success)
            return null;

        return match.Groups[1].Value.ToLowerInvariant() switch
        {
            "cs" => "csharp",
            "shell" => "bash",
            "csharp" or "bash" or "json" => match.Groups[1].Value.ToLowerInvariant(),
            _ => null
        };
    }

    private static string HighlightCSharp(string code)
    {
        var encoded = WebUtility.HtmlEncode(code);
        var placeholders = new List<(string Key, string Html)>();
        var index = 0;

        string Reserve(string html)
        {
            var key = PlaceholderKey(index++);
            placeholders.Add((key, html));
            return key;
        }

        encoded = CommentRegex().Replace(encoded, m => Reserve(Span(m.Value, "hljs-comment")));
        encoded = StringRegex().Replace(encoded, m => Reserve(Span(m.Value, "hljs-string")));
        encoded = NumberRegex().Replace(encoded, m => Reserve(Span(m.Value, "hljs-number")));
        encoded = BuiltInRegex().Replace(encoded, m => Reserve(Span(m.Value, "hljs-built_in")));

        foreach (var keyword in CSharpKeywords)
        {
            var pattern = $@"\b{Regex.Escape(keyword)}\b";
            encoded = Regex.Replace(encoded, pattern, m => Reserve(Span(m.Value, "hljs-keyword")));
        }

        foreach (var (key, html) in placeholders)
            encoded = encoded.Replace(key, html, StringComparison.Ordinal);

        return encoded;
    }

    private static string HighlightBash(string code)
    {
        var encoded = WebUtility.HtmlEncode(code);
        var placeholders = new List<(string Key, string Html)>();
        var index = 0;

        string Reserve(string html)
        {
            var key = PlaceholderKey(index++);
            placeholders.Add((key, html));
            return key;
        }

        encoded = BashCommentRegex().Replace(encoded, m => Reserve(Span(m.Value, "hljs-comment")));
        encoded = StringRegex().Replace(encoded, m => Reserve(Span(m.Value, "hljs-string")));
        encoded = BashFlagRegex().Replace(encoded, m => Reserve(Span(m.Value, "hljs-keyword")));

        foreach (var (key, html) in placeholders)
            encoded = encoded.Replace(key, html, StringComparison.Ordinal);

        return encoded;
    }

    private static string PlaceholderKey(int index)
    {
        const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        var a = alphabet[index % alphabet.Length];
        var b = alphabet[(index / alphabet.Length) % alphabet.Length];
        return $"@HLPH{a}{b}{index}@";
    }

    private static string Span(string text, string cssClass) =>
        $"<span class=\"{cssClass}\">{text}</span>";

    [GeneratedRegex(@"(?m)//[^\n]*")]
    private static partial Regex CommentRegex();

    [GeneratedRegex(@"(?m)#(?!!)[^\n]*")]
    private static partial Regex BashCommentRegex();

    [GeneratedRegex(@"""([^""\\]|\\.)*""")]
    private static partial Regex StringRegex();

    [GeneratedRegex(@"\b\d+(?:\.\d+)?\b")]
    private static partial Regex NumberRegex();

    [GeneratedRegex(@"\bConsole\.(?:Write(?:Line)?|Read(?:Line)?)\b")]
    private static partial Regex BuiltInRegex();

    [GeneratedRegex(@"(?<=\s)(-[a-zA-Z]+|--[\w-]+)\b")]
    private static partial Regex BashFlagRegex();
}
