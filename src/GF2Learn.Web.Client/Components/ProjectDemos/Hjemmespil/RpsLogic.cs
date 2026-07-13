namespace GF2Learn.Web.Client.Components.ProjectDemos.Hjemmespil;

public static class RpsLogic
{
    public static readonly string[] Choices = ["sten", "saks", "papir"];

    public static string Emoji(string choice) => choice switch
    {
        "sten" => "🪨",
        "saks" => "✂️",
        "papir" => "📄",
        _ => "❓"
    };

    public static string Resolve(string player, string computer)
    {
        if (player == computer) return "uafgjort";
        return (player, computer) switch
        {
            ("sten", "saks") or ("saks", "papir") or ("papir", "sten") => "vundet",
            _ => "tabt"
        };
    }

    public static string ResultLabel(string result) => result switch
    {
        "vundet" => "Du vandt!",
        "tabt" => "Computeren vandt",
        _ => "Uafgjort"
    };
}
