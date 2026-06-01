namespace GF2Learn.Web.Options;

public sealed class OpenAiOptions
{
    public const string SectionName = "OpenAi";

    public string ApiKey { get; set; } = "";
    public string Model { get; set; } = "gpt-4o-mini";
    public int MaxTokens { get; set; } = 600;
    public double TemperatureHint { get; set; } = 0.55;
    public double TemperatureCheck { get; set; } = 0.2;
}
