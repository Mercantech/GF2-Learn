---
title: "Lokal udvikling"
order: 18
category: naeste-skridt
topics: [lokal-udvikling, dotnet, csharp, vscode, visual-studio]
youtube_id: ZVGutgqBMUM
kompetencemaal:
  - "Kan forklare forskellen på C#, .NET SDK og en editor eller IDE"
  - "Kan installere og kontrollere .NET SDK på sin egen computer"
  - "Kan oprette, køre og debugge et lokalt C#-konsolprojekt"
  - "Kan arbejde med C# i enten VS Code eller Visual Studio"
timer: 2
---

# Lokal udvikling

Når du er færdig med dagens pensum — eller er foran — er næste skridt at flytte arbejdet fra browseren til din egen computer. Lokalt får du rigtige projektfiler, en debugger, Git og den samme arbejdsgang, som bruges på en arbejdsplads.

:::callout type="info"
Denne side er et **ekstra spor**. Du behøver ikke skynde dig hertil, men du må gerne begynde, så snart du er klar til at arbejde videre på din lokale PC.
:::

Videoen øverst viser hele opsætningen med VS Code på under ti minutter:

[Getting Started with C# & .NET in VS Code — officiel begynderguide](https://www.youtube.com/watch?v=ZVGutgqBMUM)


## Hvad skal installeres?

De tre dele har forskellige opgaver:

| Del | Hvad er det? | Hvad bruger du den til? |
|---|---|---|
| **C#** | Programmeringssproget | Koden du skriver |
| **.NET SDK** | Værktøjskassen til udvikling | Opretter, bygger og kører C#-projekter |
| **VS Code eller Visual Studio** | Editor eller IDE | Her skriver og debugger du koden |

:::callout type="warning"
Installér **.NET SDK** — ikke kun .NET Runtime. Runtime kan køre færdige programmer, mens SDK'et også kan bygge dine egne.
:::


## 1. Installér .NET SDK

GF2 Learn bruger **.NET 10**, som er en LTS-version. Hent .NET 10 SDK fra Microsoft:

[Download .NET SDK](https://dotnet.microsoft.com/download)

Vælg den udgave, der passer til din computer, og gennemfør installationen. Luk og åbn derefter terminalen igen.

Kontrollér installationen i PowerShell, Terminal eller kommandoprompt:

```bash
dotnet --version
dotnet --list-sdks
```

Den første kommando skal vise et versionsnummer. I listen fra den anden kommando skal du kunne se en version, der begynder med `10.`.


## 2. Vælg dit udviklingsværktøj

Du behøver kun vælge **én** af de to veje nedenfor.

### Mulighed A — VS Code

VS Code er let, hurtigt og virker på Windows, macOS og Linux.

1. Installér [Visual Studio Code](https://code.visualstudio.com/).
2. Åbn **Extensions** med `Ctrl+Shift+X`.
3. Søg efter **C# Dev Kit**.
4. Kontrollér, at udvidelsen er udgivet af **Microsoft**, og installér den.
5. Genstart VS Code, hvis den beder om det.

Microsofts vejledning kan bruges, hvis opsætningen driller:

[Kom i gang med C# i VS Code](https://code.visualstudio.com/docs/csharp/get-started)

:::callout type="tip"
Åbn altid **hele projektmappen** i VS Code — ikke kun `Program.cs`. C# Dev Kit bruger projektets `.csproj`-fil til IntelliSense, kørsel og debugging.
:::


### Mulighed B — Visual Studio

Visual Studio er en komplet IDE til Windows. Den installerer både editor, debugger og de nødvendige .NET-værktøjer samlet.

1. Hent [Visual Studio Community](https://visualstudio.microsoft.com/vs/community/).
2. Åbn **Visual Studio Installer**.
3. Markér workloaden **.NET desktop development**.
4. Vælg **Install** eller **Modify**.
5. Start Visual Studio, når installationen er færdig.

Hvis Visual Studio allerede er installeret, kan workloaden tilføjes via **Tools → Get Tools and Features**.

[Microsofts installationsguide til Visual Studio](https://learn.microsoft.com/visualstudio/install/install-visual-studio)


## 3. Opret dit første lokale projekt

### Med VS Code og terminalen

Opret en mappe og et nyt konsolprojekt:

```bash
mkdir FirstLocalApp
cd FirstLocalApp
dotnet new console
dotnet run
```

Åbn derefter projektmappen i VS Code:

```bash
code .
```

Hvis kommandoen `code` ikke findes, vælger du **File → Open Folder** i VS Code og åbner mappen `FirstLocalApp`.


### Med Visual Studio

1. Vælg **Create a new project**.
2. Søg efter **Console App** og vælg C#-versionen.
3. Kald projektet `FirstLocalApp`.
4. Vælg **.NET 10** som framework.
5. Tryk **Create**.


## 4. Skriv og kør programmet

Erstat indholdet i `Program.cs` med:

```csharp
Console.Write("Hvad hedder du? ");
string name = Console.ReadLine() ?? "ukendt";

Console.WriteLine($"Hej {name} — nu kører din C#-kode lokalt!");
```

Du kan køre programmet på flere måder:

- Skriv `dotnet run` i terminalen.
- Tryk `Ctrl+F5` for at køre uden debugger.
- Tryk `F5` for at starte med debugger.


## 5. Prøv debuggeren

En debugger lader dig stoppe programmet og undersøge værdierne undervejs.

1. Klik i venstre side ud for linjen med den sidste `Console.WriteLine` for at sætte et **breakpoint**.
2. Start programmet med `F5`.
3. Indtast dit navn.
4. Hold musen over variablen `name`, når programmet stopper.
5. Fortsæt programmet med `F5`.

Det er en af de største fordele ved at arbejde lokalt: Du kan se programmets tilstand i stedet for kun at gætte ud fra outputtet.


## Din første lokale udfordring

Vælg en opgave, du allerede har løst i GF2 Learn, og genskab den lokalt:

1. Opret et nyt konsolprojekt.
2. Skriv løsningen igen uden at kopiere den direkte.
3. Kør og test programmet med flere værdier.
4. Sæt mindst ét breakpoint og undersøg en variabel.
5. Gem projektet i et Git-repository og lav dit første commit.

```bash
git init
git add .
git commit -m "feat: add first local C# project"
```


## Hvis noget ikke virker

### `dotnet` bliver ikke fundet

- Kontrollér, at du installerede **SDK** og ikke kun Runtime.
- Luk alle terminaler, og åbn en ny.
- Genstart computeren, hvis installationen stadig ikke findes.
- Kør `dotnet --list-sdks` igen.

### IntelliSense virker ikke i VS Code

- Åbn hele mappen med `.csproj`-filen.
- Kontrollér, at **C# Dev Kit** fra Microsoft er installeret og aktiveret.
- Vent på, at projektets restore bliver færdig.
- Åbn kommandopaletten med `Ctrl+Shift+P`, og vælg **Developer: Reload Window**.

### Projektet kan ikke bruge .NET 10

Kør `dotnet --list-sdks`. Hvis der ikke står en `10.x`-version, skal .NET 10 SDK installeres fra [.NET-downloadsiden](https://dotnet.microsoft.com/download).


## Når det virker

Du er klar til at arbejde videre lokalt, når du kan sætte flueben ved alle punkterne:

- [ ] `dotnet --version` viser et versionsnummer
- [ ] Et konsolprojekt kan oprettes med `dotnet new console`
- [ ] Programmet kan køres med `dotnet run`
- [ ] IntelliSense foreslår C#-kode
- [ ] Et breakpoint stopper programmet
- [ ] Projektet har sit første Git-commit

Når det hele virker, kan du fortsætte med et af projekterne fra GF2 Learn på din egen computer.
