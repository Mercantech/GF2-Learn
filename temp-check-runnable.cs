using System.Text.RegularExpressions;
using GF2Learn.Web.Client.Services;
using GF2Learn.Web.Services;

var files = new[] {
    "content/pensum/15-kode-stil-og-navngivning.md",
    "content/pensum/16-kiss.md",
    "content/pensum/17-dry.md"
};
var parser = new RunnableCodeParser();
foreach (var file in files)
{
    var md = File.ReadAllText(file);
    var blocks = Regex.Matches(md, @"```csharp\n([\s\S]*?)```", RegexOptions.Multiline);
    var i = 0;
    foreach (Match m in blocks)
    {
        i++;
        var code = m.Groups[1].Value.Trim();
        if (!code.Contains("Console.")) continue;
        var runnable = RunnableCodeParser.IsRunnableSnippet(code);
        if (!runnable) continue;
        Console.WriteLine($"=== {Path.GetFileName(file)} block {i} (RUNNABLE) ===");
        Console.WriteLine(code.Length > 120 ? code[..120] + "..." : code);
        Console.WriteLine();
    }
}
