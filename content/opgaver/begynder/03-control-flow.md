---
title: "Control flow"
order: 3
difficulty: begynder
category: logik
related_pensum: [04-betingelser]
kompetencemaal:
  - "Kan bruge if, else if og else til at styre programflow"
  - "Kan bruge switch og ternary operator (? :) i passende situationer"
  - "Kan kombinere betingelser med brugerinput i små programmer"
---

# Kapitel 3 — Control flow

Seks opgaver om **if/else**, **switch**, **ternary** og to mini-projekter. Skriv din kode **efter `// TODO`** i editoren under hver opgave.

:::callout type="tip"
Læs pensum [Betingelser og brugerinput](/curriculum/04-betingelser). Ved opgaver med input kan du ændre testværdier i `// gf2-input:`-linjerne og trykke **Kør** igen.
:::

---

## Opgave 1 — `if` / `else` (18+)

:::exercise level="begynder"

Lav et program, der tjekker om en værdi er **højere eller lavere end 18**. Brug **if/else**. Værdien kan du skrive fast i koden eller hente med `Console.ReadLine()`.

:::

:::code-playground
```csharp
Console.WriteLine("Lav et program som tjekker om en værdi er højere eller lavere end 18. Brug if/else.");
// TODO: Lav opgave 1 herunder!
```
:::

:::solution

```csharp
int alder = 20;
if (alder >= 18)
    Console.WriteLine("Myndig");
else
    Console.WriteLine("Ikke myndig");
```

:::

---

## Opgave 2 — Lige eller ulige (`if` / `else`)

:::exercise level="begynder"

Lav et program, der tjekker om en værdi er **lige eller ulige**. Brug **if** og **else**.

:::

:::code-playground
```csharp
Console.WriteLine("Lav et program som tjekker om en værdi er lige eller ulige. Brug if og else.");
int tal = 7;
// TODO: Lav opgave 2 herunder!
```
:::

:::solution

```csharp
int tal = 7;
if (tal % 2 == 0)
    Console.WriteLine("Lige");
else
    Console.WriteLine("Ulige");
```

:::

---

## Opgave 3 — Lige eller ulige (`switch`)

:::exercise level="begynder"

Lav et program, der tjekker om en værdi er **lige eller ulige** — denne gang med **switch** (fx på `tal % 2`).

:::

:::code-playground
```csharp
Console.WriteLine("Lav et program som tjekker om en værdi er lige eller ulige – men denne gang ved at bruge switch.");
int tal = 8;
// TODO: Lav opgave 3 herunder!
```
:::

:::solution

```csharp
int tal = 8;
switch (tal % 2)
{
    case 0:
        Console.WriteLine("Lige");
        break;
    default:
        Console.WriteLine("Ulige");
        break;
}
```

:::

---

## Opgave 4 — Ternary (`? :`)

:::exercise level="begynder"

Lav et program, der tjekker om en værdi er **lige eller ulige** — brug **ternary operator** (`? :`).

:::

:::code-playground
```csharp
Console.WriteLine("Lav et program som tjekker om en værdi er lige eller ulige – brug ternary operator ( ? : ).");
int tal = 5;
// TODO: Lav opgave 4 herunder!
```
:::

:::solution

```csharp
int tal = 5;
string resultat = tal % 2 == 0 ? "Lige" : "Ulige";
Console.WriteLine(resultat);
```

:::

---

## Opgave 5 — Mini-projekt: Quiz

:::exercise level="begynder"

Lav et program, der stiller brugeren **tre spørgsmål** (du vælger selv spørgsmål og rigtige svar). Tjek hvert svar med **if/else** og udskriv til sidst, hvor mange rigtige brugeren fik.

:::

:::code-playground
```csharp
// gf2-input: svar1: København
// gf2-input: svar2: 4
// gf2-input: svar3: true
Console.WriteLine("Mini-projekt: Simpelt quiz-spil");
Console.WriteLine("Opgave:");
Console.WriteLine("Lav et program, der stiller brugeren tre spørgsmål (du vælger selv spørgsmål og svar).");
Console.WriteLine("Brugeren skal indtaste sit svar til hvert spørgsmål.");
Console.WriteLine("Programmet skal tjekke, om svaret er rigtigt eller forkert, og til sidst udskrive, hvor mange rigtige brugeren fik.");
Console.WriteLine("Tip: Brug variabler til at gemme point og svar, og if/else til at tjekke svarene.");
// TODO: Lav opgave 5 herunder!
```
:::

:::solution

```csharp
int point = 0;

Console.WriteLine("Hovedstad i Danmark?");
string s1 = Console.ReadLine()!;
if (s1.Trim().Equals("København", StringComparison.OrdinalIgnoreCase))
    point++;

Console.WriteLine("Hvor mange er 2 + 2?");
if (int.TryParse(Console.ReadLine(), out int s2) && s2 == 4)
    point++;

Console.WriteLine("Er C# et programmeringssprog? (true/false)");
if (bool.TryParse(Console.ReadLine(), out bool s3) && s3)
    point++;

Console.WriteLine($"Du fik {point} rigtige ud af 3.");
```

:::

---

## Opgave 6 — Mini-projekt: Karakter-feedback

:::exercise level="begynder"

Lav et program, hvor brugeren indtaster en **karakter** (fx 12, 10, 7, 4, 02, 00 eller -3). Giv passende feedback med **if/else** eller **switch** (fx «Super flot!», «Godt klaret», «Du kan gøre det bedre»).

:::

:::code-playground
```csharp
// gf2-input: 10
Console.WriteLine("Mini-projekt: Karakter-feedback");
Console.WriteLine("Opgave:");
Console.WriteLine("Lav et program, hvor brugeren indtaster en karakter (fx 12, 10, 7, 4, 02, 00 eller -3).");
Console.WriteLine("Programmet skal give en passende feedback baseret på karakteren.");
Console.WriteLine("Brug if/else eller switch til at vælge feedbacken.");
Console.WriteLine("Ekstra: Indtast flere karakterer og regn gennemsnittet ud.");
// TODO: Lav opgave 6 herunder!
```
:::

:::solution

```csharp
Console.Write("Indtast karakter: ");
int karakter = int.Parse(Console.ReadLine()!);

if (karakter >= 10)
    Console.WriteLine("Super flot!");
else if (karakter >= 7)
    Console.WriteLine("Godt klaret!");
else if (karakter >= 4)
    Console.WriteLine("Du kan gøre det bedre.");
else
    Console.WriteLine("Brug for ekstra støtte — spørg læreren.");

// Ekstra: gennemsnit af flere karakterer
// int sum = 0, antal = 0;
// while (antal < 3) { ... sum += karakter; antal++; }
// Console.WriteLine($"Gennemsnit: {(double)sum / antal:F2}");
```

:::

:::git-step
commit: "feat: opgaver kapitel 3 control flow"
branch: main
:::

:::related-pensum
- 04-betingelser
:::
