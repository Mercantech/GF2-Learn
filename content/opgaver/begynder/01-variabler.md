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

Ni små opgaver om grundlæggende datatyper. Arbejd dem igennem én ad gangen — skriv din kode **efter `// TODO`** i editoren under hver opgave.

:::callout type="tip"
Læs pensum [Variabler og datatyper](/curriculum/02-variabler-og-datatyper) først. Tryk **Kør** for at teste, og **Gem som løst** når du er logget ind.
:::

---

## Opgave 1 — `int`

:::exercise level="begynder"

Lav en variabel af typen **int** og tildel den værdien **10**. Udskriv variablen til konsollen.

:::

:::code-playground
```csharp
Console.WriteLine("Opgave 1: ");
Console.WriteLine("Lav en variabel af typen int og tildel den en værdi af ti!");
Console.WriteLine("Udskriv variablen til konsollen.");
// TODO: Lav opgave 1 herunder!
```
:::

:::solution

```csharp
int number = 10;
Console.WriteLine(number);
```

:::

---

## Opgave 2 — `double`

:::exercise level="begynder"

Lav en variabel af typen **double** med decimalværdien **5,25** (5 og en kvart). Udskriv variablen.

:::

:::code-playground
```csharp
Console.WriteLine("Opgave 2: ");
Console.WriteLine("Lav en variabel af typen double og tildel den en decimalværdi svarende til 5 og 1/4 (5,25).");
Console.WriteLine("Udskriv variablen til konsollen.");
// TODO: Lav opgave 2 herunder!
```
:::

:::solution

```csharp
double tal = 5.25;
Console.WriteLine(tal);
```

:::

---

## Opgave 3 — `string`

:::exercise level="begynder"

Lav en **string** med teksten **Hello, World!** (med udråbstegn). Udskriv variablen.

:::

:::code-playground
```csharp
Console.WriteLine("Opgave 3: ");
Console.WriteLine("Lav en variabel af typen string og tildel den en værdi - den skal indeholde teksten 'Hello, World' med et udråbstegn til sidst!");
Console.WriteLine("Udskriv variablen til konsollen.");
// TODO: Lav opgave 3 herunder!
```
:::

:::solution

```csharp
string greeting = "Hello, World!";
Console.WriteLine(greeting);
```

:::

---

## Opgave 4 — `bool`

:::exercise level="begynder"

Lav en **bool** og tildel den `true` eller `false`. Udskriv værdien.

:::

:::code-playground
```csharp
Console.WriteLine("Opgave 4: ");
Console.WriteLine("Lav en variabel af typen bool og tildel den en sandhedsværdi (true/false).");
Console.WriteLine("Udskriv variablen til konsollen.");
// TODO: Lav opgave 4 herunder!
```
:::

:::solution

```csharp
bool erAktiv = true;
Console.WriteLine(erAktiv);
```

:::

---

## Opgave 5 — String interpolation

:::exercise level="begynder"

Lav to string-variabler: `"Hello, "` og `"World!"`. Udskriv dem **samlet** med string interpolation. Tip: `$"{variabel1}{variabel2}"`

:::

:::code-playground
```csharp
Console.WriteLine("Opgave 5: ");
Console.WriteLine("Lav to string-variabler med 'Hello, ' og 'World!' og udskriv dem samlet med string interpolation.");
// TODO: Lav opgave 5 herunder!
```
:::

:::solution

```csharp
string del1 = "Hello, ";
string del2 = "World!";
Console.WriteLine(del1 + del2);
// eller: Console.WriteLine($"{del1}{del2}");
```

:::

---

## Opgave 6 — String interpolation, fire dele

:::exercise level="begynder"

Kombinér de fire strenge til én sætning med interpolation: **Hej med dig!** (variabelnavnene er bevidst «blandet» — tænk over rækkefølgen).

:::

:::code-playground
```csharp
Console.WriteLine("Opgave 6: ");
Console.WriteLine("Her er fire strenge. Din opgave er at kombinere dem til én sætning ved brug af string interpolation. Sætningen skal blive: Hej med dig!");
Console.WriteLine("Strengene er: ");
string del1 = "Hej";
string del4 = "med";
string del3 = "dig";
string del2 = "!";
Console.WriteLine("del1: " + del1);
Console.WriteLine("del2: " + del2);
Console.WriteLine("del3: " + del3);
Console.WriteLine("del4: " + del4);
Console.WriteLine("Kombiner dem nu til én sætning:");

// TODO: Lav opgave 6 herunder!
```
:::

:::solution

```csharp
Console.WriteLine(del1 + " " + del4 + " " + del3 + del2);
// eller: Console.WriteLine($"{del1} {del4} {del3}{del2}");
```

:::

---

## Opgave 7 — `float`

:::exercise level="begynder"

Lav en **float** med værdien **3,14**. Brug suffix **`f`**: `3.14f`. Udskriv variablen.

:::

:::code-playground
```csharp
Console.WriteLine("Opgave 7: ");
Console.WriteLine("Lav en variabel af typen float og tildel den en værdi af 3 + 0,14 (brug f-suffix: 3.14f).");
Console.WriteLine("Udskriv variablen til konsollen.");
// TODO: Lav opgave 7 herunder!
```
:::

:::solution

```csharp
float piApprox = 3.14f;
Console.WriteLine(piApprox);
```

:::

---

## Opgave 8 — `char`

:::exercise level="begynder"

Lav en **char** med det **store** første bogstav i alfabetet (`'A'`). Udskriv den.

:::

:::code-playground
```csharp
Console.WriteLine("Opgave 8: ");
Console.WriteLine("Lav en variabel af typen char og tildel den en værdi af det første bogstav i alfabetet (Det skal være stort!)");
Console.WriteLine("Udskriv variablen til konsollen.");
// TODO: Lav opgave 8 herunder!
```
:::

:::solution

```csharp
char bogstav = 'A';
Console.WriteLine(bogstav);
```

:::

---

## Opgave 9 — `decimal`

:::exercise level="begynder"

Lav en **decimal** med værdien **100,5**. Brug suffix **`m`**: `100.5m`. Udskriv variablen.

:::

:::code-playground
```csharp
Console.WriteLine("Opgave 9: ");
Console.WriteLine("Lav en variabel af typen decimal og tildel den en værdi af 100,5 (brug m-suffix: 100.5m).");
Console.WriteLine("Udskriv variablen til konsollen.");
// TODO: Lav opgave 9 herunder!
```
:::

:::solution

```csharp
decimal beloeb = 100.5m;
Console.WriteLine(beloeb);
```

:::

:::git-step
commit: "feat: opgaver kapitel 1 variabler"
branch: main
:::

:::related-pensum
- 02-variabler-og-datatyper
:::
