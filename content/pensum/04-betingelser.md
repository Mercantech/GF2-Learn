---
title: "Conditionals & Input"
order: 4
topics: [betingelser, if, switch, input, scope]
kompetencemaal:
  - "Kan bruge if / else og else if til at styre programmets flow"
  - "Kan anvende switch og ternary operator i passende situationer"
  - "Kan tage input fra brugeren via Console.ReadLine"
  - "Kan validere input med TryParse for at undgå runtime-fejl"
  - "Kan forklare forskellen på lokalt og globalt scope"
timer: 3
---

# Betingelser og brugerinput

Programmer er sjældent lineære — de skal kunne tage beslutninger. **Conditionals** (betingelser) giver dig mulighed for at udføre forskellige handlinger baseret på en bestemt betingelse. Du kan bruge `if`, `else`, `switch` og **ternary operator**.

## If

`if` er den mest grundlæggende form for betingelse. Kodeblokken kører kun, hvis betingelsen er **sand** — ellers springes den over.

```csharp
if (betingelse)
{
    // Kode, der kører hvis betingelsen er sand
}
```

Eksempel — er tallet større end 10?

```csharp
int num = 15;

if (num > 10)
{
    Console.WriteLine("Tallet er større end 10.");
}
```

Her udskrives `"Tallet er større end 10"`, fordi `num > 10` er `true`.

Betingelsen i parentes skal altid evaluere til `bool`. Brug `{ }` omkring blokke med mere end én linje.


## Else

`else` definerer en **alternativ** kodeblok, der kører når `if`-betingelsen er falsk:

```csharp
if (betingelse)
{
    // Kode hvis sand
}
else
{
    // Kode hvis falsk
}
```

```csharp
Console.Write("Indtast et tal: ");
int num = int.Parse(Console.ReadLine()!);

if (num > 10)
{
    Console.WriteLine("Tallet er større end 10.");
}
else
{
    Console.WriteLine("Tallet er ikke større end 10.");
}
```


## Else if

`else if` tester flere betingelser i træk uden at lave mange indlejrede `if`-sætninger:

```csharp
if (betingelse1)
{
    // Kode hvis betingelse1 er sand
}
else if (betingelse2)
{
    // Kode hvis betingelse2 er sand
}
else if (betingelse3)
{
    // Kode hvis betingelse3 er sand
}
else
{
    // Kode hvis ingen af betingelserne er sande
}
```

Er tallet positivt, negativt eller nul?

```csharp
int num = -5;

if (num > 0)
{
    Console.WriteLine("Tallet er positivt.");
}
else if (num < 0)
{
    Console.WriteLine("Tallet er negativt.");
}
else
{
    Console.WriteLine("Tallet er nul.");
}
```

Praktisk karakter-eksempel:

```csharp
int score = 85;

if (score >= 90)
    Console.WriteLine("Karakter 12");
else if (score >= 60)
    Console.WriteLine("Bestået!");
else
    Console.WriteLine("Ikke bestået");
```


## Switch

`switch` vælger mellem forskellige handlinger baseret på **værdien** af en variabel eller et udtryk — nyttigt når du har mange faste tilfælde:

```csharp
switch (variabel)
{
    case vaerdi1:
        // Kode når variabel == vaerdi1
        break;
    case vaerdi2:
        // Kode når variabel == vaerdi2
        break;
    default:
        // Kode når intet matcher
        break;
}
```

```csharp
string dag = "fredag";

switch (dag)
{
    case "mandag":
        Console.WriteLine("Uge start");
        break;
    case "fredag":
        Console.WriteLine("Næsten weekend");
        break;
    default:
        Console.WriteLine("Hverdag");
        break;
}
```

:::callout type="info"
Moderne C# understøtter også **switch expressions** — en kortere syntaks:

```csharp
string besked = dag switch
{
    "mandag" => "Uge start",
    "fredag" => "Næsten weekend",
    _ => "Hverdag"
};
```

`_` er default-casen — den fanger alle værdier, der ikke matcher.
:::


## Ternary operator

Ternary operator (`? :`) er en kompakt **if-else** i én linje:

```csharp
resultat = (betingelse) ? vaerdiHvisSand : vaerdiHvisFalsk;
```

```csharp
int num = -5;
string result = (num >= 0) ? "Positivt" : "Negativt";
Console.WriteLine(result);   // Negativt
```

```csharp
string status = alder >= 18 ? "Voksen" : "Barn";
```

Ternary er god til **simple** valg. Brug `if/else` til kompleks logik — læsbarhed slår korthed.


## Brugerinput med Console.ReadLine

`Console.ReadLine()` læser en linje tekst fra brugeren og returnerer den som en **string**:

```csharp
Console.WriteLine("Indtast dit navn:");
string? name = Console.ReadLine();
Console.WriteLine("Hej, " + name + "!");
```

`Console.Write` skriver uden linjeskift. `Console.ReadLine()` returnerer `string?` — nullable, fordi brugeren kan trykke Enter uden at skrive noget.


## Konvertering af input

`Console.ReadLine()` returnerer altid en string. Skal du arbejde med tal, skal du **konvertere**:

```csharp
Console.WriteLine("Indtast din alder:");
string? ageInput = Console.ReadLine();
int age = int.Parse(ageInput!);
Console.WriteLine("Du er " + age + " år gammel.");
```

`int.Parse()` konverterer strengen til et heltal — men programmet **crasher**, hvis input er ugyldigt (fx `"abc"`).


## Fejlhåndtering med TryParse

Brug **TryParse** til sikker konvertering — metoden returnerer `true`/`false` i stedet for at kaste en fejl:

```csharp
Console.WriteLine("Indtast din alder:");
string? ageInput = Console.ReadLine();

if (int.TryParse(ageInput, out int age))
{
    Console.WriteLine("Du er " + age + " år gammel.");
}
else
{
    Console.WriteLine("Ugyldigt input! Sørg for at indtaste et heltal.");
}
```

Samme mønster virker for andre typer — **altid** brug TryParse på brugerinput.


## Input med forskellige datatyper

**Dobbelt (decimaltal)** — `double.TryParse`:

```csharp
Console.WriteLine("Indtast din højde i meter:");
string? heightInput = Console.ReadLine();

if (double.TryParse(heightInput, out double height))
{
    Console.WriteLine("Din højde er " + height + " meter.");
}
else
{
    Console.WriteLine("Ugyldigt input! Sørg for at indtaste et decimaltal.");
}
```

**Dato** — `DateTime.TryParse`:

```csharp
Console.WriteLine("Indtast din fødselsdato (dd-mm-yyyy):");
string? dateInput = Console.ReadLine();

if (DateTime.TryParse(dateInput, out DateTime birthDate))
{
    Console.WriteLine("Din fødselsdato er " + birthDate.ToShortDateString() + ".");
}
else
{
    Console.WriteLine("Ugyldigt input! Sørg for at indtaste en gyldig dato.");
}
```

**Boolean** — `bool.TryParse`:

```csharp
Console.WriteLine("Er du studerende? (true/false):");
string? isStudentInput = Console.ReadLine();

if (bool.TryParse(isStudentInput, out bool isStudent))
{
    if (isStudent)
        Console.WriteLine("Du er studerende.");
    else
        Console.WriteLine("Du er ikke studerende.");
}
else
{
    Console.WriteLine("Ugyldigt input! Sørg for at indtaste 'true' eller 'false'.");
}
```

:::callout type="info"
Mønsteret **læs → valider → beslut** genbruger du i næsten alle konsolprogrammer. Det er kernen i interaktiv programmering.
:::


## Scope — lokalt og globalt

Variabler og metoder har et **scope** (synlighedsområde) — de er kun tilgængelige inden for bestemte dele af koden.

### Lokalt scope

Variabler erklæret **inde i en metode eller blok** har lokalt scope. De forsvinner, når blokken slutter:

```csharp
void MyMethod()
{
    int x = 10;   // Lokal variabel — kun synlig i MyMethod
    Console.WriteLine(x);
}
// x findes ikke her
```

### Globalt scope

Variabler erklæret **uden for metoder** (på klasseniveau eller filniveau) har globalt scope og kan tilgås fra flere steder:

```csharp
int x = 10;   // Global / felt-variabel

void MyMethod()
{
    Console.WriteLine(x);   // Kan tilgås her
}
```

:::callout type="warning"
Globale variabler kan være **farlige** — de er synlige overalt og sværere at spore:

1. **Utilsigtet ændring** — flere metoder kan ændre samme variabel uden at du opdager det
2. **Navnekonflikter** — en lokal variabel kan skygge for en global med samme navn
3. **Sværere at teste** — logik spredt over hele programmet

```csharp
int x = 10;   // Global

void MyMethod()
{
    int x = 20;   // Lokal — skygger for global x
    Console.WriteLine(x);   // Udskriver 20, ikke 10
}
```

Behold variabler **lokale** så langt det er muligt — det giver mere struktureret og vedligeholdelig kode.
:::


## Kombinere betingelser og input

Et komplet mini-program, der samler det du har lært:

```csharp
Console.Write("Indtast score (0-100): ");
if (int.TryParse(Console.ReadLine(), out int score))
{
    if (score >= 90)
        Console.WriteLine("Fremragende!");
    else if (score >= 60)
        Console.WriteLine("Bestået");
    else
        Console.WriteLine("Ikke bestået");
}
else
{
    Console.WriteLine("Ugyldig score.");
}
```


:::git-step
commit: "feat: betingelser og input"
branch: main
:::

## Opsummering

- `if`, `else if` og `else` styrer programflow baseret på betingelser
- `switch` håndterer mange faste værdier; ternary er en kort if-else
- `Console.ReadLine()` læser brugerinput som string
- Brug `TryParse` til sikker konvertering — undgå crash ved ugyldigt input
- Hold variabler lokale så langt det er muligt — undgå globale variabler


:::knowledge-check
---
q: Hvilken type skal betingelsen i en `if`-sætning evaluere til?
- `int`
- **`bool`** (true eller false)
- `string`
correct: 1
explain: Betingelsen i parentes efter `if` skal altid være **boolsk** — enten `true` eller `false`. Fx `num > 10` eller `score >= 60`.
---
q: Hvad returnerer `Console.ReadLine()`?
- Et heltal
- En **string** (eller null)
- En bool
correct: 1
explain: **`Console.ReadLine()`** læser brugerens input som tekst og returnerer en **`string?`**. Skal du bruge tal, skal du konvertere — fx med `TryParse`.
---
q: Hvorfor er `int.TryParse` bedre end `int.Parse` på brugerinput?
- `TryParse` er hurtigere
- **`TryParse` crasher ikke** ved ugyldigt input — den returnerer true/false
- `Parse` kan kun bruges med positive tal
correct: 1
explain: **`int.Parse`** kaster en exception og crasher programmet, hvis brugeren skriver fx `"abc"`. **`TryParse`** returnerer `false` i stedet — så du kan vise en fejlbesked og spørge igen.
---
q: Hvornår er `switch` særligt velegnet?
- Når du skal sammenligne floating-point-tal med `==`
- Når du har **mange faste værdier** at vælge imellem
- Når betingelsen skal være boolsk
correct: 1
explain: **`switch`** vælger handling baseret på **værdien** af en variabel — fx ugedage eller menuvalg. Til simple sand/falsk-beslutninger er `if/else` ofte bedre.
---
q: Hvad gør ternary-operatoren `? :`?
- Den laver en løkke med tre iterationer
- Den er en **kompakt if-else** i én linje
- Den konverterer tekst til tal
correct: 1
explain: `(betingelse) ? vaerdiHvisSand : vaerdiHvisFalsk` er en **if-else i én linje**. Fx `alder >= 18 ? "Voksen" : "Barn"`. Brug den kun til simple valg — kompleks logik hører i `if/else`.
---
q: Hvad er **lokalt scope**?
- Variabler der kan bruges overalt i programmet
- Variabler der kun er synlige **inde i den blok**, de er erklæret i
- Variabler der kun kan være strings
correct: 1
explain: En variabel erklæret **inde i en metode eller `{ }`-blok** har **lokalt scope** — den forsvinder, når blokken slutter. Det er den anbefalede tilgang frem for globale variabler.
---
q: Hvad sker der, hvis du erklærer `int x = 20;` **inde i en metode**, når der allerede findes en global `int x = 10;`?
- Compile-fejl — du må ikke have to variabler med samme navn
- Den **lokale x skygger** for den globale — `x` inde i metoden er 20
- Den globale x overskrives automatisk til 20
correct: 1
explain: Den **lokale variabel skygger** (shadowing) for den globale. Inde i metoden refererer `x` til den lokale værdi (20). Uden for metoden er det stadig den globale (10).
:::
