---
title: "Strings"
order: 10
difficulty: begynder
category: strings
related_pensum: [02-variabler-og-datatyper, 07-datastrukturer]
kompetencemaal:
  - "Kan bruge Length, ToUpper, ToLower og Trim på strenge"
  - "Kan søge i tekst med Contains, StartsWith og IndexOf"
  - "Kan dele og sammensætte tekst med Split og Substring"
  - "Kan lave simpel validering af brugerinput"
---

# Kapitel 9 — Strings

Otte opgaver om **strenge** — læsning, søgning, opdeling og validering. Skriv din kode **efter `// TODO`** i editoren.

:::callout type="tip"
En **string** er en sekvens af tegn. Mange metoder findes på `string`-objektet — se også pensum [Variabler og datatyper](/curriculum/02-variabler-og-datatyper). Ved input: `// gf2-input:`.
:::

---

## Opgave 1 — Længde og case

:::exercise level="begynder"

Bed om en **streng** fra brugeren. Udskriv **længden**, strengen i **STORE** og **små** bogstaver.

:::

:::code-playground
```csharp
// gf2-input: Hej Verden
Console.WriteLine("Opgave 1:");
Console.WriteLine("Udskriv længde, ToUpper og ToLower.");
// TODO: Lav opgave 1 herunder!
```
:::

:::solution

```csharp
Console.Write("Tekst: ");
string text = Console.ReadLine()!;
Console.WriteLine($"Længde: {text.Length}");
Console.WriteLine(text.ToUpper());
Console.WriteLine(text.ToLower());
```

:::

---

## Opgave 2 — `Contains` og `StartsWith`

:::exercise level="begynder"

Tjek om en tekst **indeholder** ordet «C#» og om den **starter med** «GF».

:::

:::code-playground
```csharp
// gf2-input: GF2 lærer C# og Git
Console.WriteLine("Opgave 2:");
Console.WriteLine("Brug Contains og StartsWith.");
// TODO: Lav opgave 2 herunder!
```
:::

:::solution

```csharp
Console.Write("Tekst: ");
string text = Console.ReadLine()!;
Console.WriteLine(text.Contains("C#") ? "Indeholder C#" : "Indeholder ikke C#");
Console.WriteLine(text.StartsWith("GF") ? "Starter med GF" : "Starter ikke med GF");
```

:::

---

## Opgave 3 — `Substring`

:::exercise level="begynder"

Udskriv de **første 3 tegn** og **de sidste 4 tegn** af en streng (antag strengen er lang nok).

:::

:::code-playground
```csharp
// gf2-input: Programmering
Console.WriteLine("Opgave 3:");
Console.WriteLine("Brug Substring til start og slut af streng.");
// TODO: Lav opgave 3 herunder!
```
:::

:::solution

```csharp
Console.Write("Tekst: ");
string text = Console.ReadLine()!;
Console.WriteLine(text.Substring(0, 3));
Console.WriteLine(text.Substring(text.Length - 4));
```

:::

---

## Opgave 4 — `Split`

:::exercise level="begynder"

Bed om en linje med **navne adskilt af komma**. Split strengen og udskriv **hvert navn** på sin egen linje.

:::

:::code-playground
```csharp
// gf2-input: Anna,Bo,Clara
Console.WriteLine("Opgave 4:");
Console.WriteLine("Split på komma og udskriv hvert navn.");
// TODO: Lav opgave 4 herunder!
```
:::

:::solution

```csharp
Console.Write("Navne (komma-separeret): ");
string line = Console.ReadLine()!;
string[] names = line.Split(',');
foreach (string name in names)
    Console.WriteLine(name.Trim());
```

:::

---

## Opgave 5 — `Replace`

:::exercise level="begynder"

Erstat alle **mellemrum** med **underscore** (`_`) i en streng.

:::

:::code-playground
```csharp
// gf2-input: GF2 Learn platform
Console.WriteLine("Opgave 5:");
Console.WriteLine("Erstat mellemrum med underscore.");
// TODO: Lav opgave 5 herunder!
```
:::

:::solution

```csharp
Console.Write("Tekst: ");
string text = Console.ReadLine()!;
Console.WriteLine(text.Replace(" ", "_"));
```

:::

---

## Opgave 6 — `Trim`

:::exercise level="begynder"

Fjern **whitespace** for og efter input med **`Trim()`**, og udskriv den trimmede streng.

:::

:::code-playground
```csharp
// gf2-input:    Hej   
Console.WriteLine("Opgave 6:");
Console.WriteLine("Brug Trim på input.");
// TODO: Lav opgave 6 herunder!
```
:::

:::solution

```csharp
Console.Write("Tekst: ");
string rawInput = Console.ReadLine()!;
string trimmed = rawInput.Trim();
Console.WriteLine($"'{trimmed}' har længde {trimmed.Length}");
```

:::

---

## Opgave 7 — Tæl et bogstav

:::exercise level="begynder"

Bed om en tekst og tæl, hvor mange gange bogstavet **«a»** optræder (store og små).

:::

:::code-playground
```csharp
// gf2-input: Abrakadabra
Console.WriteLine("Opgave 7:");
Console.WriteLine("Tæl bogstavet 'a' (case-insensitive).");
// TODO: Lav opgave 7 herunder!
```
:::

:::solution

```csharp
Console.Write("Tekst: ");
string text = Console.ReadLine()!.ToLower();
int count = 0;
for (int i = 0; i < text.Length; i++)
{
    if (text[i] == 'a')
        count++;
}
Console.WriteLine($"Antal 'a': {count}");
```

:::

---

## Opgave 8 — Simpel e-mail-tjek

:::exercise level="begynder"

Lav simpel **validering**: e-mail skal **indeholde `@`** og **mindst ét punktum efter @**. Udskriv om den er gyldig.

:::

:::code-playground
```csharp
// gf2-input: elev@skole.dk
Console.WriteLine("Opgave 8:");
Console.WriteLine("Tjek om e-mail indeholder @ og . efter @.");
// TODO: Lav opgave 8 herunder!
```
:::

:::solution

```csharp
Console.Write("E-mail: ");
string email = Console.ReadLine()!.Trim();
int atIndex = email.IndexOf('@');
bool isValid = atIndex > 0 && email.IndexOf('.', atIndex + 1) > atIndex;
Console.WriteLine(isValid ? "Gyldig e-mail" : "Ugyldig e-mail");
```

:::
