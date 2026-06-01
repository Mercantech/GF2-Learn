using GF2Learn.Web.Client.Models;

namespace GF2Learn.Web.Client.Services;

public static class ExerciseSegmentBuilder
{
    public static IReadOnlyList<ExercisePageSegment> Build(IReadOnlyList<ExerciseBlockTransfer> blocks)
    {
        var segments = new List<ExercisePageSegment>();

        for (var i = 0; i < blocks.Count; i++)
        {
            var block = blocks[i];
            if (block.Kind == "card"
                && i + 1 < blocks.Count
                && blocks[i + 1].Kind == "editor"
                && blocks[i + 1].PartIndex == block.PartIndex)
            {
                segments.Add(new ExercisePartSegment(block.Primary, block.PartIndex, blocks[i + 1].Primary));
                i++;
                continue;
            }

            if (block.Kind is "markdown" or "solution")
                segments.Add(new HtmlSegment(block.Primary));
        }

        return segments;
    }
}
