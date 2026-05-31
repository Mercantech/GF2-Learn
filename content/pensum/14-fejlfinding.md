---
title: "Debugging"
order: 14
topics: [debug, visual-studio, breakpoints]
kompetencemaal:
  - "Kan bruge breakpoints til at stoppe programkørslen på udvalgte steder"
  - "Kan anvende Step Into, Step Over og Step Out til at analysere kodeflow"
  - "Kan læse og ændre variabelværdier i Locals og Autos under debugging"
  - "Kan identificere og rette simple logiske fejl ved hjælp af debuggeren"
  - "Forstår debugging som et aktivt værktøj til problemløsning — ikke kun fejlretning"
timer: 2
---

# Fejlfinding og debugging

**Debugging** er kunsten at finde og rette fejl i kode. I GF2 bruger I primært **Visual Studio** på Windows — her er en introduktion til debuggeren og de vigtigste teknikker.

Debugging handler om at **forstå**, hvad koden faktisk gør under kørsel — ikke kun at fikse røde fejlbeskeder.


## Kom i gang — seks trin

### 1. Sæt breakpoints

Klik i den **grå bjælke** ved siden af linjenummeret. Et **rødt stoppunkt** (breakpoint) vises. Når programmet kører, **stopper** det på den linje.

### 2. Kør din kode

Klik **Start Debugging** eller tryk **F5**. Programmet kører indtil det rammer dit breakpoint.

### 3. Gennemgå koden trin for trin

Brug step-knapperne eller genveje:

| Tast | Navn | Gør |
|------|------|-----|
| **F11** | Step Into | Gå **ind** i metodekald |
| **F10** | Step Over | Kør næste linje — **spring over** metodekald |
| **Shift+F11** | Step Out | Færdiggør nuværende metode, stop i kaldende kode |
| **F5** | Continue | Kør videre til næste breakpoint |

Fra venstre i værktøjslinjen: **Step Into (F11)**, **Step Over (F10)**, **Step Out (Shift+F11)**.

**Eksempel:** Du er på `VisResultat(score);`

- **F10** — kører hele `VisResultat` og stopper på næste linje
- **F11** — går ind i `VisResultat` og stopper på første linje der

### 4. Se variabler

I **Autos** og **Locals** fanerne ser du aktuelle værdier for variabler i scope. **Watch** lader dig følge specifikke udtryk (fx `score >= 60`).

**Call Stack** viser kæden af metodekald, der førte til breakpointet — nyttigt til at forstå, *hvorfor* du er endt her.

Hover over variabler i editoren for hurtigt at se værdier uden at åbne vinduer.

### 5. Ændre variabelværdier

Højreklik på en variabel i **Autos** eller **Locals** → **Modify Value**. Test, hvordan andre værdier påvirker programmets adfærd — uden at genstarte.

### 6. Fortsæt kørsel

Tryk **Continue** eller **F5** for at køre videre til næste breakpoint eller programmets afslutning.

:::callout type="info"
Debugging er en proces — det tager tid at finde fejl. Med praksis og tålmodighed bliver du dygtigere til det.
:::


## Breakpoint-typer

Visual Studio understøtter flere typer breakpoints:

1. **Breakpoint** — stopper på en bestemt linje (den du bruger mest)
2. **Condition breakpoint** — stopper kun hvis en betingelse er opfyldt (fx `i == 42` eller `score < 0`)
3. **Tracepoint** — logger en besked uden at stoppe programmet — god til at spore flow
4. **Temporary breakpoint** — aktiv **én gang**, fjernes automatisk efter stop
5. **Dependent breakpoint** — aktiveres først efter et andet breakpoint er ramt

I praksis bruger du næsten altid det **normale breakpoint**. Højreklik på breakpointet for betingelser og avancerede indstillinger.


## Logiske fejl vs. compile-fejl

**Compile-fejl** (røde understregninger) — programmet kører slet ikke. **Logiske fejl** er værre: programmet kører, men giver forkert resultat.

```csharp
// Logisk fejl — gennemsnit bliver forkert ved heltalsdivision
double gennemsnit = (a + b + c) / 3;

// Rettelse
double gennemsnit = (a + b + c) / 3.0;
```

Ved logiske fejl er der ingen rød fejlbesked — sæt breakpoint, kør med F5, og tjek variabler trin for trin.


## Exceptions og try/catch

En **exception** er en runtime-fejl — programmet crasher, medmindre du fanger den:

```csharp
try
{
    Console.Write("Indtast tal: ");
    int n = int.Parse(Console.ReadLine()!);
    int resultat = 10 / n;
    Console.WriteLine(resultat);
}
catch (DivideByZeroException)
{
    Console.WriteLine("Du kan ikke dividere med nul.");
}
catch (FormatException)
{
    Console.WriteLine("Det var ikke et gyldigt tal.");
}
catch (Exception ex)
{
    Console.WriteLine($"Uventet fejl: {ex.Message}");
}
```

Uden `catch` stopper programmet med fejlbesked — brug debuggeren til at læse **stack trace** og finde linjen.


## Unit tests og videre læring

Visual Studio har også værktøjer til **softwaretest** — fx **unit tests**. Det er et naturligt næste skridt efter manuel debugging.

Microsofts læringsforløb:

[Use Visual Studio for modern development (Microsoft Learn)](https://learn.microsoft.com/en-us/training/paths/visual-studio/)

Generel C#-testdokumentation:

[Unit testing in .NET](https://learn.microsoft.com/en-us/dotnet/core/testing/)


:::git-step
commit: "docs: debugging og breakpoints"
branch: main
:::

## Opsummering

- **F5** starter debugging; breakpoints stopper på valgte linjer
- **F10/F11/Shift+F11** styrer gennemløb af kode
- **Locals**, **Autos** og **Watch** viser variabelværdier — du kan også ændre dem under kørsel
- Condition breakpoints og tracepoints giver finere kontrol
- Debugging er aktiv problemløsning — især vigtig ved logiske fejl uden compile-errors


:::knowledge-check
---
q: Hvordan sætter du et **breakpoint** i Visual Studio?
- Højreklik på filen i Solution Explorer
- **Klik i den grå bjælke** ved siden af linjenummeret
- Tryk Ctrl+B i editoren uden markering
correct: 1
explain: Et **rødt stoppunkt** i marginen stopper programmet på den linje, når du kører med **F5 (Start Debugging)**.
---
q: Hvad er forskellen på **F10 (Step Over)** og **F11 (Step Into)**?
- De gør præcis det samme
- **F10** kører næste linje uden at gå ind i metodekald; **F11** går **ind i** metoden
- F11 stopper programmet; F10 fortsætter til slut
correct: 1
explain: På `VisResultat(score);` — **F10** kører hele metoden og stopper på næste linje. **F11** hopper ind i `VisResultat` og stopper på første linje der.
---
q: Hvor ser du **aktuelle variabelværdier** under debugging?
- Kun i Output-vinduet
- I **Locals**, **Autos** og **Watch** — plus hover over variabler i editoren
- I Git History
correct: 1
explain: **Locals** viser variabler i nuværende scope. **Watch** lader dig følge specifikke udtryk. **Call Stack** viser, hvilke metoder der ledte til breakpointet.
---
q: Hvad er en **logisk fejl**?
- Rød understregning — programmet kompilerer ikke
- Programmet **kører**, men giver **forkert resultat**
- En fejl i `.gitignore`
correct: 1
explain: **Compile-fejl** stopper kørslen. **Logiske fejl** er værre — koden kører, men fx `(a+b+c)/3` giver forkert gennemsnit ved heltalsdivision. Debuggeren hjælper her.
---
q: Hvad gør **Shift+F11 (Step Out)**?
- Sletter breakpointet
- **Færdiggør nuværende metode** og stopper i kaldende kode
- Genstarter debugging
correct: 1
explain: **Step Out** er nyttig, når du er gået **ind i** en metode med F11 og vil tilbage til kalderen uden at steppe linje for linje gennem resten.
---
q: Hvad er et **condition breakpoint**?
- Et breakpoint der kun virker én gang
- Et breakpoint der **kun stopper**, når en betingelse er opfyldt — fx `i == 42`
- Et breakpoint der automatisk retter koden
correct: 1
explain: **Condition breakpoints** sparer tid i løkker — programmet kører, indtil fx `score < 0` bliver sand. Højreklik på breakpointet for at sætte betingelsen.
---
q: Hvorfor er **`try/catch`** relevant ved debugging?
- Det erstatter alle breakpoints
- Det **fanger runtime-exceptions** — så programmet ikke crasher, og du kan læse fejlbeskeden
- Det gør koden hurtigere
correct: 1
explain: Uden **`catch`** stopper programmet ved fx `DivideByZeroException`. Med **`try/catch`** kan du vise brugervenlig besked og bruge debuggeren til at læse **stack trace**.
:::
