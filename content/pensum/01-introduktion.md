---
title: "Getting Started"
order: 1
topics: [intro, setup, csharp, main]
kompetencemaal:
  - "Kan installere Visual Studio, GitHub Desktop og VS Code"
  - "Kan oprette et GitHub-konto og klone GF2-CSharp repoet"
  - "Kan oprette et simpelt C# konsolprojekt"
  - "Kan forklare strukturen i et standard C# konsolprogram (namespace, class, Main)"
timer: 2
---

# Kom i gang

Velkommen til **GF2 Programmering**. Du lærer **C#** og **Git** i studiegrupper — med opgaver, projekter og pensum du kan arbejde dig igennem i dit eget tempo.

## Installation og værktøjer

Som udvikler bruger du flere værktøjer, der hver især løser en bestemt opgave. **Visual Studio** er din primære IDE (Integrated Development Environment) til C# — her skriver, kører og debugger du kode. **GitHub Desktop** giver dig et grafisk interface til Git, så du kan committe og pushe uden at huske alle kommandoer. **VS Code** er en let editor, der er praktisk til scripts, markdown og hurtige ændringer.

Hent disse værktøjer inden første undervisningsdag:

- [Visual Studio](https://visualstudio.microsoft.com/) — primær IDE til C#
- [GitHub Desktop](https://desktop.github.com/download/) — nem Git-klient
- [VS Code](https://code.visualstudio.com/) — let editor til scripts og markdown
- [.NET SDK](https://dotnet.microsoft.com/download) — følger typisk med Visual Studio

Opret en konto på [GitHub](https://github.com/) hvis du ikke har en. GitHub er en cloud-tjeneste, hvor du gemmer og deler din kode — tænk på det som Google Drive, men til programmeringsprojekter.


## GitHub og kursus-repoet

**GitHub** er verdens største platform til versionsstyring af kode. Når du **kloner** et repository, kopierer du hele projektet ned på din egen computer — så du kan arbejde lokalt og synce ændringer tilbage.

Klon kursus-repoet — her ligger opgaver og projekter:

[github.com/Mercantech/GF2-CSharp](https://github.com/Mercantech/GF2-CSharp)

I GitHub Desktop: **File → Clone repository** → vælg URL'en ovenfor. Nu har du en lokal kopi, du kan åbne i Visual Studio.


:::callout type="info"
Pensum er designet til begyndere. Har du erfaring, kan du arbejde forud — der er ekstra projekter at tage fat i.
:::

:::callout type="warning"
Opgaverne kan løses med AI i én prompt. Brug **ikke** AI til at løse opgaverne for dig — men det er fint at bruge AI til at forstå pensum bedre.
:::

## Dit første C# konsolprojekt

Et **konsolprogram** kører i en terminal og skriver tekst — perfekt til at lære grundlæggende C# uden at tænke på grafik.

Sådan opretter du et projekt i Visual Studio:

1. **Create a new project** → vælg **Console App** (C#)
2. Giv projektet et navn, fx `MitFoersteProjekt`
3. Gennemgå `Program.cs` — det er her din kode lever
4. Tryk **F5** for at køre

:::callout type="info"
Nyere C#-projekter kan bruge **top-level statements** (kode uden `class` og `Main`). Det er kortere, men den klassiske struktur nedenfor hjælper dig med at forstå, hvad der sker under overfladen.
:::

## Anatomi af et C#-program — Main(string[] args)

En standard konsolapp i C# har en fast struktur. Du skal forstå programmet, før du kan skrive i det!

```csharp
namespace FirstProgram
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hej og velkommen til C#-bogen!");
        }
    }
}
```

Her er kodeordene, der starter programmet, forklaret ét ad gangen:

### namespace

[Namespaces (Microsoft Docs)](https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/namespace)

Et **namespace** organiserer din kode og adskiller den fra andre dele af programmet. Det står i toppen af stort set alle filer og kan indeholde klasser — og endda indlejrede namespaces.

### Tuborg-klammer — `{ }`

[Formatting (Microsoft Docs)](https://learn.microsoft.com/dotnet/csharp/fundamentals/coding-style/coding-conventions)

Du kender sikkert `{` og `}` fra andre sprog (Python bruger dem normalt ikke). Klammerne definerer, hvor et namespace, en klasse eller en metode **starter** og **stopper**.

### public

[public (Microsoft Docs)](https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/public)

`public` betyder, at den efterfølgende klasse eller variabel er **offentligt tilgængelig** — altså at den kan bruges uden for sin egen klasse eller namespace. Emner som `public`, `private` og `protected` dækker vi senere i forløbet.

### class

`class` er et nøgleord, du lærer meget mere om i **OOP-kapitlet**. Klasser bruges til at holde **variabler** (attributter) og **metoder**. Objekter hænger tæt sammen med klasser — det kommer vi tilbage til der.

### static

`Main`-metoden skal altid være `static`. Det betyder, at metoden kan kaldes **uden** at oprette en instans af klassen. Du behøver ikke dyb viden om `static` lige nu — det bliver tydeligere, når vi arbejder med objekter.

### void

`void` betyder, at metoden **ikke returnerer** noget. Hvis en metode skal sende et resultat tilbage, erstatter du `void` med den rette type — fx `int` eller `string`. Det bruger vi meget senere i kurset.

### Main(string[] args)

`Main()` er **indgangspunktet** til programmet. Når du starter appen, leder runtime efter en `Main`-metode og begynder der. I større projekter ligger det ofte i en fil kaldet `Program.cs` — præcis det samme princip.

`string[] args` er **kommandolinjeargumenter** (input fra terminalen). Dem bruger vi sjældent i starten, men de er en del af signaturen.

### Console

`Console` er en del af **System**-biblioteket. Den giver dig et sted at skrive tekst til (og senere læse input fra) konsollen.

```csharp
Console.WriteLine("Hej GF2!");  // Skriver en linje og springer ned
Console.Write("Hej ");          // Skriver uden linjeskift
```

`WriteLine` er den du bruger mest i starten.


## Versionsstyring fra dag ét

Selv som begynder bør du vænne dig til at gemme din kode med **Git**. Hver gang du har lavet en logisk ændring, laver du en **commit** — et snapshot af din kode på det tidspunkt.

En god commit-besked forklarer *hvad* du har ændret: `"init: opret GF2 projekt"` er bedre end `"fix"` eller `"asdf"`. Du lærer mere om Git i senere pensum-emner.


:::git-step
commit: "init: opret GF2 projekt"
branch: main
Opret et GitHub-repo og push dit projekt efter første commit.
:::

## Sådan arbejder du med pensum

Pensum er opdelt i kapitler, der følger undervisningen. Hvert kapitel har **kompetencemål** — det er det, du skal kunne, når du er færdig. Brug dem som tjekliste.

Arbejdsgang:

1. Læs kapitlet og gennemgå kodeeksemplerne
2. Prøv koden selv i Visual Studio
3. Løs opgaverne i repoet — start med begynder-niveau
4. Commit dine løsninger med tydelige beskeder

Studiegrupper er til at diskutere og hjælpe hinanden — ikke til at kopiere løsninger. Det er helt normalt at sidde fast; det er en del af at lære programmering.


:::knowledge-check
---
q: Hvad er **indgangspunktet** i et standard C# konsolprogram?
- `namespace`-blokken
- `Main(string[] args)`-metoden
- Den første `Console.WriteLine`-linje
correct: 1
explain: Når programmet starter, leder runtime efter en **`Main`-metode** og begynder der. Det er derfor `Main` kaldes indgangspunktet — resten af koden kører, fordi `Main` kalder den.
---
q: Hvad betyder `void` i signaturen `static void Main(string[] args)`?
- Metoden returnerer en string
- Metoden **returnerer ingenting**
- Metoden kan kun kaldes én gang
correct: 1
explain: **`void`** betyder, at metoden ikke sender en værdi tilbage. Hvis den skulle returnere fx et tal, ville returtypen være `int` i stedet for `void`.
---
q: Hvad er forskellen på `Console.WriteLine` og `Console.Write`?
- `WriteLine` skriver en linje og **springer ned**; `Write` skriver uden linjeskift
- `Write` er hurtigere end `WriteLine`
- `WriteLine` kan kun bruges med strings
correct: 0
explain: **`WriteLine`** afslutter outputtet med et linjeskift. **`Write`** skriver teksten og bliver på samme linje — begge kan bruges med forskellige datatyper via konvertering.
---
q: Hvad gør et **namespace** i C#?
- Det starter programmet automatisk
- Det **organiserer kode** og adskiller den fra andre dele af programmet
- Det er det samme som en klasse
correct: 1
explain: Et **namespace** grupperer relateret kode (klasser, metoder) og hjælper med at undgå navnekonflikter. Det er ikke det samme som en klasse — en namespace kan indeholde mange klasser.
---
q: Hvad er formålet med en **Git commit**?
- At slette gamle filer fra projektet
- At gemme et **snapshot** af koden på et bestemt tidspunkt
- At uploade kode direkte til Visual Studio
correct: 1
explain: En **commit** er et snapshot af projektet med en besked om, hvad der blev ændret. Det gør det muligt at gå tilbage i historikken og se præcis, hvad der skete hvornår.
---
q: Hvilket værktøj er den **primære IDE** til C#-udvikling i GF2?
- VS Code
- **Visual Studio**
- GitHub Desktop
correct: 1
explain: **Visual Studio** er den fulde IDE til C# — her opretter, kører og debugger du projekter. VS Code er en let editor til scripts og markdown. GitHub Desktop håndterer Git, ikke selve kodningen.
---
q: Hvad betyder `static` på `Main`-metoden?
- Metoden kan kun kaldes fra internettet
- Metoden kan kaldes **uden** at oprette et objekt af klassen
- Metoden gemmer data permanent på disken
correct: 1
explain: **`static`** betyder, at metoden hører til klassen selv — ikke til et specifikt objekt. Derfor kan runtime kalde `Main` direkte uden `new Program()`.
:::
