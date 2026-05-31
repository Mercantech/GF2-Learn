using Markdig;

namespace GF2Learn.Web.Services;

public sealed class MarkdownService(ContentPreprocessor preprocessor)
{
    private readonly MarkdownPipeline _pipeline = new MarkdownPipelineBuilder()
        .UseAdvancedExtensions()
        .Build();

    public string ToHtml(string markdown, string? contentSlug = null)
    {
        var processed = preprocessor.Process(markdown, contentSlug);
        return Markdown.ToHtml(processed, _pipeline);
    }
}
