---
title: "Methods"
order: 6
topics: [metoder, funktioner, overloading]
kompetencemaal:
  - "Kan definere og kalde metoder i C#"
  - "Kan bruge parametre og returværdier korrekt"
  - "Kan forklare forskellen på void-metoder og metoder med returværdi"
  - "Kan strukturere min kode bedre ved hjælp af metoder"
  - "Kan anvende overloading, valgfrie parametre og rekursion"
timer: 3
---

# Metoder

En **metode** er en blok kode, der udfører en specifik opgave og kan kaldes fra andre dele af programmet. Metoder kan tage **parametre** som input og **returnere** en værdi — de gør koden genbrugelig, læsbar og nemmere at vedligeholde.

## Metodens dele

En metode i C# består af:

1. **Adgangsmodifikator** — synlighed, fx `public` eller `private`
2. **Returtype** — typen metoden returnerer (`void` hvis ingen returværdi)
3. **Metodenavn** — navnet du kalder metoden med
4. **Parametre** — input til metoden (kan være tom)
5. **Metodekrop** — koden der udfører opgaven

```csharp
public void SayHello()
{
    Console.WriteLine("Hello, World!");
}
```

:::callout type="info"
I moderne C# konsolprogrammer kan metoder være **static** og kaldes direkte uden `new Program()` — det bruger vi ofte i GF2:

```csharp
static void SigHej(string navn)
{
    Console.WriteLine($"Hej {navn}!");
}

SigHej("GF2");
```

**Top-level statements** tillader metoder i samme fil som `Program.cs`.
:::


## Simpel metode uden parametre

```csharp
class Program
{
    public void SayHello()
    {
        Console.WriteLine("Hello, World!");
    }

    static void Main()
    {
        Program program = new Program();
        program.SayHello();
    }
}
```

`static void Main()` er indgangspunktet. Instansmetoder som `SayHello()` kaldes på et **objekt** (`program`).


## Metoder med parametre

Parametre giver metoden input at arbejde med:

```csharp
class Program
{
    public int Add(int a, int b)
    {
        return a + b;
    }

    static void Main()
    {
        Program program = new Program();
        int result = program.Add(5, 3);
        Console.WriteLine("Resultat: " + result);   // 8
    }
}
```

Flere parametre adskilles med komma. Typen skal matche det, metoden forventer.


## Metoder med returværdier

Metoder kan returnere en værdi med `return`:

```csharp
public string GetGreeting(string name)
{
    return "Hello, " + name + "!";
}

// Kald
string greeting = program.GetGreeting("Alice");
Console.WriteLine(greeting);   // Hello, Alice!
```

Korte metoder kan bruge **expression body** (`=>`):

```csharp
static int Fordobl(int x) => x * 2;

static double BeregnGennemsnit(int a, int b, int c)
{
    return (a + b + c) / 3.0;
}
```


## void vs. metoder med returværdi

**void** — metoden returnerer ingenting, den udfører en handling:

```csharp
static void PrintSkillelinje()
{
    Console.WriteLine("----------");
}
```

**Returværdi** — metoden beregner og sender et resultat tilbage:

```csharp
static bool ErPositiv(int tal)
{
    return tal > 0;
}

static string KarakterFraScore(int score)
{
    if (score >= 90) return "12";
    if (score >= 60) return "Bestået";
    return "Ikke bestået";
}
```

Brug `void` til sideeffekter (print, gem data). Brug returværdi, når metoden *producerer* et resultat, du skal bruge videre.


## Overloading

**Overloading** betyder flere metoder med **samme navn**, men **forskellige parametre**. Compileren vælger den rigtige ud fra antal og type:

```csharp
public void Print(int number)
{
    Console.WriteLine("Nummer: " + number);
}

public void Print(string message)
{
    Console.WriteLine("Besked: " + message);
}

program.Print(42);              // Kalder int-versionen
program.Print("Hello, World!"); // Kalder string-versionen
```

```csharp
static int Add(int a, int b) => a + b;
static double Add(double a, double b) => a + b;
static int Add(int a, int b, int c) => a + b + c;
```


## Valgfrie parametre

Parametre kan have **standardværdier** — de er valgfrie ved kald:

```csharp
public void PrintMessage(string message, int times = 1)
{
    for (int i = 0; i < times; i++)
        Console.WriteLine(message);
}

program.PrintMessage("Hello!");    // Udskriver én gang
program.PrintMessage("Hello!", 3); // Udskriver tre gange
```

```csharp
static void SigHej(string navn, string hilsen = "Hej")
{
    Console.WriteLine($"{hilsen} {navn}!");
}

SigHej("Ada");              // Hej Ada!
SigHej("Ada", "Velkommen"); // Velkommen Ada!
```


## Rekursive metoder

En metode kan **kalde sig selv** — det kaldes **rekursion**. Det bruges når et problem kan opdeles i mindre delproblemer:

```csharp
public int Factorial(int n)
{
    if (n <= 1)
        return 1;
    return n * Factorial(n - 1);
}

int result = program.Factorial(5);
Console.WriteLine("Fakultet af 5 er: " + result);   // 120
```

Rekursion kræver altid en **base case** (`n <= 1`) — ellers kører metoden for evigt.


## Metoder som strukturering

Metoder er dit vigtigste værktøj til at organisere kode. Opdel i logiske enheder i stedet for én lang `Main`:

```csharp
static int LaesScore()
{
    Console.Write("Indtast score: ");
    int.TryParse(Console.ReadLine(), out int score);
    return score;
}

static void VisResultat(int score)
{
    string karakter = score >= 60 ? "Bestået" : "Ikke bestået";
    Console.WriteLine($"Score {score}: {karakter}");
}

int score = LaesScore();
VisResultat(score);
```

Hver metode gør **én ting** — princippet kaldes **Single Responsibility**.


## Hvornår er metoder smarte? — især i UI

Metoder er afgørende i UI-udvikling (fx **Blazor**), fordi de strukturerer og genbruger kode:

**Modularitet og genbrug**
- Skriv logik én gang, kald den mange steder
- Ret fejl ét sted — ikke i ti kopier af samme kode

**Læselighed**
- Gode metodenavne (`VisBrugerProfil`, `BeregnTotal`) forklarer *hvad* koden gør uden at læse implementeringen

Det gælder konsolprogrammer og web-apps — metoder er grundlaget for vedligeholdelig kode.


:::git-step
commit: "feat: metoder og strukturering"
branch: main
:::

## Opsummering

- Metoder organiserer og genbruger kode med navn, parametre og returtype
- `void` returnerer ingenting; andre typer kræver `return`
- **Overloading** — samme navn, forskellige parametre
- **Valgfrie parametre** — standardværdier giver fleksibilitet
- **Rekursion** — metoden kalder sig selv med en base case
- Metoder kan være **static** (på klassen) eller instansmetoder (på et objekt)


:::knowledge-check
---
q: Hvad betyder **`void`** som returtype på en metode?
- Metoden returnerer en tom string
- Metoden returnerer ingen værdi
- Metoden kan kun kaldes én gang
correct: 1
explain: **`void`** betyder, at metoden udfører en handling (fx udskriver tekst) men **sender intet resultat tilbage**. Metoder med andre returtyper skal bruge `return`.
---
q: Hvad er **overloading**?
- At kalde en metode flere gange i træk
- Flere metoder med samme navn men forskellige parametre
- At overskrive en metode i en underklasse
correct: 1
explain: **Overloading** lader dig definere fx `Add(int a, int b)` og `Add(double a, double b)` — compileren vælger den rigtige ud fra **antal og type** af parametre.
---
q: Hvad gør et **valgfrit parameter** med standardværdi?
- Det gør parameteren til en global variabel
- Kaldet kan udelade parameteren — standardværdien bruges
- Det tvinger kalderen til at sende argumentet
correct: 1
explain: Med `int times = 1` kan du kalde `PrintMessage("Hej")` **uden** andet argument — `times` bliver automatisk 1. Send `3` for at overskrive standardværdien.
---
q: Hvad er **base case** i en rekursiv metode?
- Den første linje i metoden
- Det stop-scenario, der afslutter rekursionen
- En fejl, der stopper programmet
correct: 1
explain: Rekursion kræver altid en **base case** — ellers kalder metoden sig selv for evigt. I `Factorial`: `if (n <= 1) return 1;` stopper rekursionen.
---
q: Hvornår bør du bruge en metode med **returværdi** frem for `void`?
- Når metoden kun printer til konsollen
- Når metoden beregner et resultat, du skal bruge videre i koden
- Når metoden har mere end tre parametre
correct: 1
explain: Brug **returværdi**, når metoden *producerer* data — fx `ErPositiv(int tal)` returnerer `bool`. Brug **`void`**, når den kun har en sideeffekt som print.
---
q: Hvad er forskellen på en **static** metode og en **instansmetode**?
- Static metoder er langsommere
- Static kaldes på klassen; instans kaldes på et objekt (`new Program()`)
- Instansmetoder kan ikke have parametre
correct: 1
explain: **`static void SigHej(...)`** kaldes direkte: `SigHej("Ada")`. Instansmetoder som `program.SayHello()` kræver et **objekt** oprettet med `new`.
---
q: Hvad er **Single Responsibility** i forbindelse med metoder?
- Hver klasse må kun have én metode
- Hver metode gør én ting — det gør koden læsbar og vedligeholdelig
- Metoder må kun have ét parameter
correct: 1
explain: Opdel kode i metoder, der hver løser **ét klart problem** — fx `LaesScore()` og `VisResultat()`. Det gør det nemmere at finde fejl og genbruge logik.
:::
