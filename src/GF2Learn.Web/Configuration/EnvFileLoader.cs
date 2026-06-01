namespace GF2Learn.Web.Configuration;

/// <summary>
/// Loads a .env file into process environment variables (Docker Compose does this automatically).
/// ASP.NET Core maps <c>OpenAi__ApiKey</c> to <c>OpenAi:ApiKey</c>.
/// </summary>
public static class EnvFileLoader
{
    public static string? LoadedPath { get; private set; }

    public static void TryLoadFromAncestors(string? startDirectory = null, int maxLevels = 6)
    {
        var dir = startDirectory ?? Directory.GetCurrentDirectory();
        for (var i = 0; i < maxLevels; i++)
        {
            var path = Path.Combine(dir, ".env");
            if (File.Exists(path))
            {
                Load(path);
                return;
            }

            var parent = Directory.GetParent(dir);
            if (parent is null)
                break;
            dir = parent.FullName;
        }
    }

    public static void Load(string path)
    {
        if (!File.Exists(path))
            return;

        foreach (var rawLine in File.ReadAllLines(path))
        {
            var line = rawLine.Trim();
            if (line.Length == 0 || line.StartsWith('#'))
                continue;

            var eq = line.IndexOf('=');
            if (eq <= 0)
                continue;

            var key = line[..eq].Trim();
            var value = line[(eq + 1)..].Trim();

            if (value.Length >= 2
                && ((value.StartsWith('"') && value.EndsWith('"'))
                    || (value.StartsWith('\'') && value.EndsWith('\''))))
            {
                value = value[1..^1];
            }

            if (key.Length > 0)
                Environment.SetEnvironmentVariable(key, value);
        }

        LoadedPath = Path.GetFullPath(path);
    }
}
