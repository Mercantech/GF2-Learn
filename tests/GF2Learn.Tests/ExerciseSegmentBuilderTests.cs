using GF2Learn.Web.Client.Models;
using GF2Learn.Web.Client.Services;

namespace GF2Learn.Tests;

public sealed class ExerciseSegmentBuilderTests
{
    [Fact]
    public void Build_PairsExerciseCardWithMatchingEditor()
    {
        ExerciseBlockTransfer[] blocks =
        [
            new("markdown", 0, "<p>Intro</p>"),
            new("card", 0, "<section>Opgave</section>"),
            new("editor", 0, "Console.WriteLine(10);"),
            new("solution", 0, "<p>Losning</p>")
        ];

        var segments = ExerciseSegmentBuilder.Build(blocks);

        Assert.Collection(
            segments,
            first =>
            {
                var html = Assert.IsType<HtmlSegment>(first);
                Assert.Equal("<p>Intro</p>", html.Html);
            },
            second =>
            {
                var part = Assert.IsType<ExercisePartSegment>(second);
                Assert.Equal(0, part.PartIndex);
                Assert.Equal("<section>Opgave</section>", part.CardHtml);
                Assert.Equal("Console.WriteLine(10);", part.Code);
            },
            third =>
            {
                var html = Assert.IsType<HtmlSegment>(third);
                Assert.Equal("<p>Losning</p>", html.Html);
            });
    }

    [Fact]
    public void Build_DoesNotPairCardWithDifferentEditorPart()
    {
        ExerciseBlockTransfer[] blocks =
        [
            new("card", 0, "<section>Opgave 1</section>"),
            new("editor", 1, "Console.WriteLine(20);")
        ];

        var segments = ExerciseSegmentBuilder.Build(blocks);

        Assert.Empty(segments);
    }
}
