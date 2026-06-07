using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using GF2Learn.Web.Models;
using GF2Learn.Web.Options;
using Microsoft.Extensions.Options;

namespace GF2Learn.Web.Services;

public interface IExerciseAiService
{
    bool IsConfigured { get; }

    Task<ExerciseAiHintResponse> GetHintAsync(
        ExercisePartAiContext context,
        string studentCode,
        string? consoleOutput,
        CancellationToken cancellationToken = default);

    Task<ExerciseAiCheckResponse> CheckSolutionAsync(
        ExercisePartAiContext context,
        string studentCode,
        string? consoleOutput,
        CancellationToken cancellationToken = default);
}

public sealed class ExerciseAiService(
    HttpClient http,
    IOptions<OpenAiOptions> options,
    ILogger<ExerciseAiService> logger) : IExerciseAiService
{
    private const int MaxCodeLength = 12_000;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private readonly OpenAiOptions _options = options.Value;

    public bool IsConfigured => !string.IsNullOrWhiteSpace(_options.ApiKey?.Trim());

    public Task<ExerciseAiHintResponse> GetHintAsync(
        ExercisePartAiContext context,
        string studentCode,
        string? consoleOutput,
        CancellationToken cancellationToken = default)
    {
        EnsureConfigured();

        var system = """
            Du er en venlig GF2-programmeringsvejleder på dansk.
            Giv korte hints — aldrig den fulde færdige løsning eller færdig kode som elev kan copy-paste.
            Peg på næste logiske skridt, fejltyper eller koncepter (fx datatyper, Console.WriteLine).
            Hold svaret under 120 ord.
            """;

        var user = BuildUserPrompt(
            context,
            studentCode,
            consoleOutput,
            """
            Eleven sidder fast. Giv 1–2 konkrete hints på dansk.
            """);

        return CompleteHintAsync(system, user, cancellationToken);
    }

    public async Task<ExerciseAiCheckResponse> CheckSolutionAsync(
        ExercisePartAiContext context,
        string studentCode,
        string? consoleOutput,
        CancellationToken cancellationToken = default)
    {
        EnsureConfigured();

        var system = """
            Du vurderer om en GF2-elev har løst en lille C#-opgave korrekt.
            Svar KUN med valid JSON i dette format (ingen markdown):
            {"solved":true|false,"feedback":"kort feedback på dansk"}

            Vær large og elevvenlig i vurderingen:
            - solved=true hvis opgavens kernkrav er opfyldt (korrekt logik, datatyper, loops, betingelser osv.).
            - Det er OK hvis eleven har sjov med opgaven: fx sjove eller kreative tekster i Console.WriteLine,
              ekstra cw-linjer, humor eller personlige detaljer — så længe den faglige opgave faktisk er løst.
            - Kræv ikke ordret samme output som reference-løsningen. Forskellig formulering er fint.
            - Hvis løsningen er rigtig men kreativ, roser du gerne det kort i feedback.
            - solved=false kun ved reelle mangler: forkert logik, vigtige krav ikke opfyldt, kode der ikke kompilerer/kører,
              eller kode der stadig kun er skabelon/TODO uden egentlig løsning.
            - Afvis ikke bare fordi eleven skrev noget sjovt i cw, så længe opgaven ellers er korrekt.

            feedback: 1-3 sætninger — rolig, opmuntrende tone. Nævn evt. hvad der mangler (uden hele løsningen).
            """;

        var reference = string.IsNullOrWhiteSpace(context.ReferenceSolutionCode)
            ? "(intet officielt løsningsforslag — vurder ud fra opgaveteksten.)"
            : context.ReferenceSolutionCode;

        var user = BuildUserPrompt(
            context,
            studentCode,
            consoleOutput,
            $"""
            Reference-løsning (kun til din vurdering — nævn den ikke direkte for eleven):
            ```csharp
            {reference}
            ```

            Vurder om elevens kode løser opgaven. Husk at være large: kreativt eller sjovt output i cw er OK,
            hvis den faglige del er korrekt — og ros gerne kreativiteten kort.
            """);

        var raw = await CompleteRawAsync(system, user, _options.TemperatureCheck, jsonMode: true, cancellationToken);
        return ParseCheckResponse(raw);
    }

    private async Task<ExerciseAiHintResponse> CompleteHintAsync(
        string system,
        string user,
        CancellationToken cancellationToken)
    {
        var raw = await CompleteRawAsync(system, user, _options.TemperatureHint, jsonMode: false, cancellationToken);
        return new ExerciseAiHintResponse(raw.Trim());
    }

    private async Task<string> CompleteRawAsync(
        string system,
        string user,
        double temperature,
        bool jsonMode,
        CancellationToken cancellationToken)
    {
        var payload = new OpenAiChatRequest
        {
            Model = _options.Model,
            Temperature = temperature,
            MaxTokens = _options.MaxTokens,
            ResponseFormat = jsonMode ? new OpenAiResponseFormat { Type = "json_object" } : null,
            Messages =
            [
                new OpenAiChatMessage("system", system),
                new OpenAiChatMessage("user", user)
            ]
        };

        using var request = new HttpRequestMessage(HttpMethod.Post, "v1/chat/completions");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.ApiKey);
        request.Content = new StringContent(
            JsonSerializer.Serialize(payload, JsonOptions),
            Encoding.UTF8,
            "application/json");

        using var response = await http.SendAsync(request, cancellationToken);
        var body = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            logger.LogWarning("OpenAI API fejl {Status}: {Body}", (int)response.StatusCode, Truncate(body, 500));
            throw new InvalidOperationException("AI-tjenesten svarede med en fejl. Prøv igen om lidt.");
        }

        var parsed = JsonSerializer.Deserialize<OpenAiChatResponse>(body, JsonOptions);
        var content = parsed?.Choices?.FirstOrDefault()?.Message?.Content;
        if (string.IsNullOrWhiteSpace(content))
            throw new InvalidOperationException("AI returnerede et tomt svar.");

        return content;
    }

    private static string BuildUserPrompt(
        ExercisePartAiContext context,
        string studentCode,
        string? consoleOutput,
        string instruction)
    {
        studentCode = Truncate(studentCode.Trim(), MaxCodeLength);
        var outputBlock = string.IsNullOrWhiteSpace(consoleOutput)
            ? "(eleven har ikke kørt koden endnu, eller der er intet output)"
            : Truncate(consoleOutput.Trim(), 4000);

        return $"""
            {instruction}

            Opgavetekst:
            {context.TaskDescription}

            Startkode (skabelon):
            ```csharp
            {context.StarterCode}
            ```

            Elevens kode:
            ```csharp
            {studentCode}
            ```

            Konsol-output efter "Kør":
            {outputBlock}
            """;
    }

    private static ExerciseAiCheckResponse ParseCheckResponse(string raw)
    {
        try
        {
            var json = ExtractJsonObject(raw);
            var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;
            var solved = (root.TryGetProperty("solved", out var s) && s.ValueKind == JsonValueKind.True)
                || (root.TryGetProperty("isSolved", out var isSolved) && isSolved.ValueKind == JsonValueKind.True);
            var feedback = root.TryGetProperty("feedback", out var f)
                ? f.GetString() ?? ""
                : "";
            if (string.IsNullOrWhiteSpace(feedback))
                feedback = solved ? "Godt klaret — opgaven ser løst ud." : "Opgaven ser ikke helt løst ud endnu.";

            return new ExerciseAiCheckResponse(solved, feedback.Trim());
        }
        catch (Exception)
        {
            return new ExerciseAiCheckResponse(
                false,
                "Kunne ikke læse AI-vurderingen. Prøv at køre koden og tjek igen.");
        }
    }

    private static string ExtractJsonObject(string raw)
    {
        var start = raw.IndexOf('{');
        var end = raw.LastIndexOf('}');
        if (start >= 0 && end > start)
            return raw[start..(end + 1)];
        return raw;
    }

    private void EnsureConfigured()
    {
        if (!IsConfigured)
            throw new InvalidOperationException("OpenAI API-nøgle er ikke konfigureret på serveren.");
    }

    private static string Truncate(string value, int max) =>
        value.Length <= max ? value : value[..max] + "\n// … (afkortet)";

    private sealed class OpenAiChatRequest
    {
        public string Model { get; set; } = "";
        public List<OpenAiChatMessage> Messages { get; set; } = [];
        public double Temperature { get; set; }
        [JsonPropertyName("max_tokens")]
        public int MaxTokens { get; set; }
        [JsonPropertyName("response_format")]
        public OpenAiResponseFormat? ResponseFormat { get; set; }
    }

    private sealed record OpenAiChatMessage(string Role, string Content);

    private sealed class OpenAiResponseFormat
    {
        public string Type { get; set; } = "json_object";
    }

    private sealed class OpenAiChatResponse
    {
        [JsonPropertyName("choices")]
        public List<OpenAiChoice>? Choices { get; set; }
    }

    private sealed class OpenAiChoice
    {
        [JsonPropertyName("message")]
        public OpenAiMessage? Message { get; set; }
    }

    private sealed class OpenAiMessage
    {
        [JsonPropertyName("content")]
        public string? Content { get; set; }
    }
}
