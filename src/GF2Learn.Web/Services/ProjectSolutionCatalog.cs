using GF2Learn.Web.Client.Components.ProjectDemos.Extended;
using GF2Learn.Web.Client.Components.ProjectDemos.Mvp;
using GF2Learn.Web.Models;

namespace GF2Learn.Web.Services;

public sealed class ProjectSolutionCatalog(IWebHostEnvironment env)
{
    private readonly IReadOnlyDictionary<string, ProjectSolutionInfo> _bySlug = Build(env);

    public ProjectSolutionInfo? Get(string projectSlug) =>
        _bySlug.TryGetValue(projectSlug, out var info) ? info : null;

    private static IReadOnlyDictionary<string, ProjectSolutionInfo> Build(IWebHostEnvironment hostEnv)
    {
        string? Console(string slug) => LoadConsoleDemo(hostEnv, slug);

        return new Dictionary<string, ProjectSolutionInfo>(StringComparer.Ordinal)
        {
            ["projekt-1-hjemmespil"] = new(
                "projekt-1-hjemmespil",
                Deliverables(
                    ("GitHub-repo med README", "Beskriv spil, hvordan man bygger og kører.", true),
                    ("Konsol-app", "Mindst tre spil (gæt tal, sten/saks/papir, tic-tac-toe) + gerne eget spil.", true),
                    ("Blazor", "Valgfri ekstra — UI-version af ét eller flere spil.", false)),
                Console("projekt-1-hjemmespil"),
                typeof(HjemmespilMvpDemo),
                typeof(HjemmespilExtendedDemo)),

            ["projekt-huskeliste"] = new(
                "projekt-huskeliste",
                Deliverables(
                    ("GitHub-repo med README", "Menu og funktioner dokumenteret.", true),
                    ("Konsol-app", "Huskeliste med tilføj, vis, markér/slet og lister.", true),
                    ("Blazor", "Valgfri ekstra — samme funktioner i browser.", false)),
                Console("projekt-huskeliste"),
                typeof(HuskelisteMvpDemo),
                typeof(HuskelisteExtendedDemo)),

            ["projekt-jobsoegning"] = new(
                "projekt-jobsoegning",
                Deliverables(
                    ("GitHub-repo med README", "Eksempel på input/output.", true),
                    ("Konsol-app", "Stillinger med firma, titel og status; søg/filter.", true),
                    ("Blazor", "Anbefalet ekstra — tabel og oversigt.", false)),
                Console("projekt-jobsoegning"),
                typeof(JobsoegningMvpDemo),
                typeof(JobsoegningExtendedDemo)),

            ["projekt-2-binaer"] = new(
                "projekt-2-binaer",
                Deliverables(
                    ("GitHub-repo med README", "Begge eksempler fra opgaven (8-bit og 4 oktetter).", true),
                    ("Konsol-app", "Binær ↔ decimal uden Convert; begge veje.", true),
                    ("Blazor", "Valgfri ekstra — UI til omregning (stadig egne metoder).", false)),
                Console("projekt-2-binaer"),
                typeof(BinaerMvpDemo),
                typeof(BinaerExtendedDemo)),

            ["projekt-bibliotek"] = new(
                "projekt-bibliotek",
                Deliverables(
                    ("GitHub-repo med README", "Klasser og flow beskrevet.", true),
                    ("Konsol-app", "Book/Library, udlån og retur med OOP.", true),
                    ("Blazor", "Valgfri ekstra — visuelt bibliotek.", false)),
                Console("projekt-bibliotek"),
                typeof(BibliotekMvpDemo),
                typeof(BibliotekExtendedDemo)),

            ["projekt-log-parser"] = new(
                "projekt-log-parser",
                Deliverables(
                    ("GitHub-repo med README", "Med eksempel-logfil (uden persondata).", true),
                    ("Konsol-app", "Læs fil, tæl ERROR/WARN/INFO, fejlhåndtering.", true),
                    ("Blazor", "Valgfri ekstra — vis rapport i UI.", false)),
                Console("projekt-log-parser"),
                typeof(LogParserMvpDemo),
                typeof(LogParserExtendedDemo)),

            ["projekt-3-enterprise"] = new(
                "projekt-3-enterprise",
                new DeliverableExpectations(
                [
                    new("GitHub-repo med README", "LDAP-opsætning uden secrets; hvordan programmet køres.", true),
                    new("Konsol-app med LDAP", "Hent brugere og grupper fra AD (testmiljø).", true),
                    new("Blazor-UI", "Valgfri — bruger-/gruppeoversigt.", false),
                    new("Stempel ind/ud", "Valgfri ekstra.", false)
                ],
                    "Live Active Directory kan ikke køres i browseren. Afprøv mod skolens AD eller testmiljø."),
                null,
                null,
                null)
        };
    }

    private static DeliverableExpectations Deliverables(
        params (string title, string desc, bool required)[] items) =>
        new(items.Select(i => new DeliverableItem(i.title, i.desc, i.required)).ToList());

    private static string? LoadConsoleDemo(IWebHostEnvironment env, string slug)
    {
        foreach (var root in new[] { env.ContentRootPath, AppContext.BaseDirectory })
        {
            if (string.IsNullOrEmpty(root)) continue;
            var path = Path.Combine(root, "ProjectReferences", slug, "console-demo.cs");
            if (File.Exists(path))
                return File.ReadAllText(path);
        }

        return null;
    }
}
