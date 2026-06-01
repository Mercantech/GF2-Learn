---
title: "Variabler"
order: 1
difficulty: begynder
related_pensum: [02-variabler-og-datatyper]
kompetencemaal:
  - "Kan erklære og bruge grundlæggende datatyper (int, double, string, bool, float, char, decimal)"
  - "Kan tildele værdier til variabler og udskrive dem"
  - "Kan kombinere strenge med string interpolation"
---

# Kapitel 1 — Variabler

I dette kapitel arbejder I i et konsolprojekt med mappen **Opgaver**. Opret filen `Variabler.cs` (namespace `Opgaver`) og kald `Variabler.Run()` fra jeres `Program.cs` — eller kør hver metode enkeltvis mens I arbejder.

:::callout type="tip"
Læs pensum [Variabler og datatyper](/curriculum/02-variabler-og-datatyper) først. I `Int1()` er der allerede en vejledende løsning — brug den som skabelon, men **prøv selv** i de øvrige opgaver før I kigger i løsningsforslaget.
:::

## Startkode

Opret klassen nedenfor i jeres projekt. Udfyld alle `// TODO`-felter.

```csharp
using System;

namespace Opgaver
{
    public class Variabler
    {
        public static void Run()
        {
            Console.WriteLine("------------------------------------------");
            Console.WriteLine("Velkommen til opgaver omkring Variabler!");

            Int1();
            Double1();
            Strings1();
            Bool1();
            StringInterpolation();
            StringInterpolation2();
            Float1();
            Char1();
            Decimal1();
        }

        public static void Int1()
        {
            Console.WriteLine("Opgave 1: ");
            Console.WriteLine("Lav en variabel af typen int og tildel den en værdi af ti!");
            Console.WriteLine("Udskriv variablen til konsollen.");
            // TODO: Lav opgave 1 herunder! (Her er en vejledende løsning – prøv selv først!)
            int number = 10;
            Console.WriteLine(number);
        }

        public static void Double1()
        {
            Console.WriteLine("Opgave 2: ");
            Console.WriteLine("Lav en variabel af typen double og tildel den en decimalværdi svarende til 5 og 1/4 (5,25).");
            Console.WriteLine("Udskriv variablen til konsollen.");
            // TODO: Lav opgave 2 herunder!

        }

        public static void Strings1()
        {
            Console.WriteLine("Opgave 3: ");
            Console.WriteLine("Lav en variabel af typen string og tildel den en værdi - den skal indeholde teksten 'Hello, World' med et udråbstegn til sidst!");
            Console.WriteLine("Udskriv variablen til konsollen.");
            // TODO: Lav opgave 3 herunder!
        }

        public static void Bool1()
        {
            Console.WriteLine("Opgave 4: ");
            Console.WriteLine("Lav en variabel af typen bool og tildel den en sandhedsværdi (true/false).");
            Console.WriteLine("Udskriv variablen til konsollen.");
            // TODO: Lav opgave 4 herunder!

        }

        public static void StringInterpolation()
        {
            Console.WriteLine("Opgave 5: ");
            Console.WriteLine("Lav to string-variabler med 'Hello, ' og 'World!' og udskriv dem samlet med string interpolation.");
            // TODO: Lav opgave 5 herunder! Tip: brug $"{variabel1}{variabel2}"
        }

        public static void StringInterpolation2()
        {
            Console.WriteLine("Opgave 6: ");
            Console.WriteLine("Her er fire strenge. Din opgave er at kombinere dem til én sætning ved brug af string interpolation. Sætningen skal blive: Hej med dig!");
            Console.WriteLine("Strengene er: ");
            string del1 = "Hej";
            string del4 = "med";
            string del3 = "dig";
            string del2 = "!";
            Console.WriteLine($"del1: {del1}");
            Console.WriteLine($"del2: {del2}");
            Console.WriteLine($"del3: {del3}");
            Console.WriteLine($"del4: {del4}");
            Console.WriteLine("Kombiner dem nu til én sætning:");

            // TODO: Lav opgave 6 herunder!
        }

        public static void Float1()
        {
            Console.WriteLine("Opgave 7: ");
            Console.WriteLine("Lav en variabel af typen float og tildel den en værdi af 3 + 0,14 (brug f-suffix: 3.14f).");
            Console.WriteLine("Udskriv variablen til konsollen.");
            // TODO: Lav opgave 7 herunder!
        }

        public static void Char1()
        {
            Console.WriteLine("Opgave 8: ");
            Console.WriteLine("Lav en variabel af typen char og tildel den en værdi af det første bogstav i alfabetet (Det skal være stort!)");
            Console.WriteLine("Udskriv variablen til konsollen.");
            // TODO: Lav opgave 8 herunder!
        }

        public static void Decimal1()
        {
            Console.WriteLine("Opgave 9: ");
            Console.WriteLine("Lav en variabel af typen decimal og tildel den en værdi af 100,5 (brug m-suffix: 100.5m).");
            Console.WriteLine("Udskriv variablen til konsollen.");
            // TODO: Lav opgave 9 herunder!
        }
    }
}
```

---

## Opgave 1 — `int`

:::exercise level="begynder"

Erklær en **int**-variabel med værdien **10** og udskriv den med `Console.WriteLine`.

:::

---

## Opgave 2 — `double`

:::exercise level="begynder"

Erklær en **double** med værdien **5,25** (5 og en kvart) og udskriv den.

:::

---

## Opgave 3 — `string`

:::exercise level="begynder"

Erklær en **string** med teksten **Hello, World!** (med udråbstegn) og udskriv den.

:::

---

## Opgave 4 — `bool`

:::exercise level="begynder"

Erklær en **bool** (`true` eller `false`) og udskriv værdien.

:::

---

## Opgave 5 — String interpolation (to strenge)

:::exercise level="begynder"

Opret to strenge: `"Hello, "` og `"World!"`. Udskriv dem **samlet** med string interpolation (`$"..."`).

:::

---

## Opgave 6 — String interpolation (fire dele)

:::exercise level="begynder"

Kombinér `del1`, `del2`, `del3` og `del4` til én sætning: **Hej med dig!** (bemærk rækkefølgen af variablerne i startkoden).

:::

---

## Opgave 7 — `float`

:::exercise level="begynder"

Erklær en **float** med værdien **3,14** — brug suffix **`f`**: `3.14f`. Udskriv variablen.

:::

---

## Opgave 8 — `char`

:::exercise level="begynder"

Erklær en **char** med det **store** første bogstav i alfabetet (`'A'`). Udskriv den.

:::

---

## Opgave 9 — `decimal`

:::exercise level="begynder"

Erklær en **decimal** med værdien **100,5** — brug suffix **`m`**: `100.5m`. Udskriv variablen.

:::

:::git-step
commit: "feat: opgaver kapitel 1 variabler"
branch: main
:::

:::solution

```csharp
public static void Double1()
{
    Console.WriteLine("Opgave 2: ");
    Console.WriteLine("Lav en variabel af typen double og tildel den en decimalværdi svarende til 5 og 1/4 (5,25).");
    Console.WriteLine("Udskriv variablen til konsollen.");
    double tal = 5.25;
    Console.WriteLine(tal);
}

public static void Strings1()
{
    Console.WriteLine("Opgave 3: ");
    Console.WriteLine("Lav en variabel af typen string og tildel den en værdi - den skal indeholde teksten 'Hello, World' med et udråbstegn til sidst!");
    Console.WriteLine("Udskriv variablen til konsollen.");
    string greeting = "Hello, World!";
    Console.WriteLine(greeting);
}

public static void Bool1()
{
    Console.WriteLine("Opgave 4: ");
    Console.WriteLine("Lav en variabel af typen bool og tildel den en sandhedsværdi (true/false).");
    Console.WriteLine("Udskriv variablen til konsollen.");
    bool erAktiv = true;
    Console.WriteLine(erAktiv);
}

public static void StringInterpolation()
{
    Console.WriteLine("Opgave 5: ");
    Console.WriteLine("Lav to string-variabler med 'Hello, ' og 'World!' og udskriv dem samlet med string interpolation.");
    string del1 = "Hello, ";
    string del2 = "World!";
    Console.WriteLine($"{del1}{del2}");
}

public static void StringInterpolation2()
{
    Console.WriteLine("Opgave 6: ");
    Console.WriteLine("Her er fire strenge. Din opgave er at kombinere dem til én sætning ved brug af string interpolation. Sætningen skal blive: Hej med dig!");
    Console.WriteLine("Strengene er: ");
    string del1 = "Hej";
    string del4 = "med";
    string del3 = "dig";
    string del2 = "!";
    Console.WriteLine($"del1: {del1}");
    Console.WriteLine($"del2: {del2}");
    Console.WriteLine($"del3: {del3}");
    Console.WriteLine($"del4: {del4}");
    Console.WriteLine("Kombiner dem nu til én sætning:");
    Console.WriteLine($"{del1} {del4} {del3}{del2}");
}

public static void Float1()
{
    Console.WriteLine("Opgave 7: ");
    Console.WriteLine("Lav en variabel af typen float og tildel den en værdi af 3 + 0,14 (brug f-suffix: 3.14f).");
    Console.WriteLine("Udskriv variablen til konsollen.");
    float piApprox = 3.14f;
    Console.WriteLine(piApprox);
}

public static void Char1()
{
    Console.WriteLine("Opgave 8: ");
    Console.WriteLine("Lav en variabel af typen char og tildel den en værdi af det første bogstav i alfabetet (Det skal være stort!)");
    Console.WriteLine("Udskriv variablen til konsollen.");
    char bogstav = 'A';
    Console.WriteLine(bogstav);
}

public static void Decimal1()
{
    Console.WriteLine("Opgave 9: ");
    Console.WriteLine("Lav en variabel af typen decimal og tildel den en værdi af 100,5 (brug m-suffix: 100.5m).");
    Console.WriteLine("Udskriv variablen til konsollen.");
    decimal beloeb = 100.5m;
    Console.WriteLine(beloeb);
}
```

:::

:::related-pensum
- 02-variabler-og-datatyper
:::
