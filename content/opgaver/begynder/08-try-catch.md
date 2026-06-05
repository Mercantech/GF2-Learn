---
title: "Try/catch"
order: 9
difficulty: begynder
category: fejl
related_pensum: [14-fejlfinding, 02-variabler-og-datatyper]
kompetencemaal:
  - "Kan genkende runtime-fejl og rette koden med try/catch"
  - "Kan fange specifikke exceptions (fx division med nul)"
  - "Kan kombinere try/catch med TryParse til robust input"
  - "Kan skrive brugervenlige fejlbeskeder i stedet for crash"
---

# Kapitel 8 — Try/catch

Otte opgaver om **exceptions** og **try/catch**. I **opgave 1 og 2** starter du med kode **uden** try/catch — kør, se fejlen, og **ret selv koden**. Fra opgave 3 bygger du videre på de samme teknikker.

:::callout type="tip"
Læs pensum [Fejlfinding](/curriculum/14-fejlfinding). I opgave 1–2: tryk **Kør** med det medfølgende testinput, læs fejlbeskeden, og tilføj derefter try/catch i editoren. Skift værdier i `// gf2-input:` for at teste igen.
:::

---

## Opgave 1 — Grundlæggende try/catch

:::exercise level="begynder"

Lav et program, der dividerer **10** med et tal fra brugeren.

Koden i editoren er **uden try/catch**. Tryk **Kør** med testinput **`abc`** — programmet fejler. **Ret koden** med **try/catch**, så det ikke crasher ved ugyldigt input, og udskriv en pæn fejlbesked.

:::

:::code-playground
```csharp
// gf2-input: abc
Console.WriteLine("Opgave 1:");
Console.WriteLine("Kør programmet — det fejler ved ugyldigt input.");
Console.WriteLine("Ret koden med try/catch, så det ikke crasher.");

Console.Write("Tal: ");
int n = int.Parse(Console.ReadLine()!);
Console.WriteLine(10 / n);
```
:::

:::solution

```csharp
try
{
    Console.Write("Tal: ");
    int n = int.Parse(Console.ReadLine()!);
    Console.WriteLine(10 / n);
}
catch (Exception)
{
    Console.WriteLine("Noget gik galt — tjek dit input.");
}
```

:::

---

## Opgave 2 — Division med nul

:::exercise level="begynder"

Lav et program, der dividerer **100** med et tal fra brugeren.

Koden er igen **uden try/catch**. Tryk **Kør** med testinput **`0`** — programmet fejler med **division med nul**. **Ret koden** så du fanger **`DivideByZeroException`** og udskriver en pæn fejlbesked.

:::

:::code-playground
```csharp
// gf2-input: 0
Console.WriteLine("Opgave 2:");
Console.WriteLine("Kør programmet — det fejler ved division med 0.");
Console.WriteLine("Ret koden med try/catch og fang DivideByZeroException.");

Console.Write("Tal: ");
int n = int.Parse(Console.ReadLine()!);
Console.WriteLine(100 / n);
```
:::

:::solution

```csharp
try
{
    Console.Write("Tal: ");
    int n = int.Parse(Console.ReadLine()!);
    Console.WriteLine(100 / n);
}
catch (DivideByZeroException)
{
    Console.WriteLine("Du kan ikke dividere med nul.");
}
```

:::

---

## Opgave 3 — Flere catch-blokke

:::exercise level="begynder"

Lav **to catch-blokke**: én til **division med nul** og én til **andre fejl** (fx ugyldigt tal).

:::

:::code-playground
```csharp
// gf2-input: abc
Console.WriteLine("Opgave 3:");
Console.WriteLine("Brug flere catch-blokke til nul og andre fejl.");
// TODO: Lav opgave 3 herunder!
```
:::

:::solution

```csharp
try
{
    Console.Write("Tal: ");
    int n = int.Parse(Console.ReadLine()!);
    Console.WriteLine(50 / n);
}
catch (DivideByZeroException)
{
    Console.WriteLine("Du kan ikke dividere med nul.");
}
catch (Exception)
{
    Console.WriteLine("Ugyldigt tal.");
}
```

:::

---

## Opgave 4 — `ex.Message`

:::exercise level="begynder"

Fang en exception og udskriv **`ex.Message`** i en generel **catch (Exception ex)**-blok.

:::

:::code-playground
```csharp
// gf2-input: xyz
Console.WriteLine("Opgave 4:");
Console.WriteLine("Udskriv ex.Message i catch (Exception ex).");
// TODO: Lav opgave 4 herunder!
```
:::

:::solution

```csharp
try
{
    Console.Write("Tal: ");
    int n = int.Parse(Console.ReadLine()!);
    Console.WriteLine(n * 2);
}
catch (Exception ex)
{
    Console.WriteLine($"Fejl: {ex.Message}");
}
```

:::

---

## Opgave 5 — `int.TryParse`

:::exercise level="begynder"

Lav robust input med **`int.TryParse`**. Ved fejl: udskriv **«Ugyldigt tal»** — uden try/catch.

:::

:::code-playground
```csharp
// gf2-input: 12
// gf2-input: hej
Console.WriteLine("Opgave 5:");
Console.WriteLine("Brug int.TryParse — spørg indtil brugeren indtaster et gyldigt tal.");
// TODO: Lav opgave 5 herunder!
```
:::

:::solution

```csharp
int number;
while (true)
{
    Console.Write("Indtast et tal: ");
    if (int.TryParse(Console.ReadLine(), out number))
        break;
    Console.WriteLine("Ugyldigt tal");
}
Console.WriteLine($"Du indtastede {number}");
```

:::

---

## Opgave 6 — `finally`

:::exercise level="begynder"

Brug **try/catch/finally**. I **finally** udskrives altid **«Færdig med beregning»**.

:::

:::code-playground
```csharp
// gf2-input: 5
Console.WriteLine("Opgave 6:");
Console.WriteLine("Brug try/catch/finally og udskriv i finally.");
// TODO: Lav opgave 6 herunder!
```
:::

:::solution

```csharp
try
{
    Console.Write("Tal: ");
    int n = int.Parse(Console.ReadLine()!);
    Console.WriteLine($"Kvadrat: {n * n}");
}
catch (Exception)
{
    Console.WriteLine("Kunne ikke beregne kvadrat.");
}
finally
{
    Console.WriteLine("Færdig med beregning");
}
```

:::

---

## Opgave 7 — Metode med try/catch

:::exercise level="begynder"

Lav en metode **`PrintQuotient(int a, int b)`** med try/catch omkring divisionen. Kald metoden med to input-tal.

:::

:::code-playground
```csharp
// gf2-input: 20
// gf2-input: 4
Console.WriteLine("Opgave 7:");
Console.WriteLine("Lav en metode med try/catch omkring division.");
// TODO: Lav opgave 7 herunder!
```
:::

:::solution

```csharp
void PrintQuotient(int a, int b)
{
    try
    {
        Console.WriteLine(a / b);
    }
    catch (DivideByZeroException)
    {
        Console.WriteLine("Kan ikke dividere med nul.");
    }
}

Console.Write("Tæller: ");
int a = int.Parse(Console.ReadLine()!);
Console.Write("Nævner: ");
int b = int.Parse(Console.ReadLine()!);
PrintQuotient(a, b);
```

:::

---

## Opgave 8 — Robust lommeregner

:::exercise level="begynder"

Lav en lille **lommeregner**: to tal og en operation (`+` eller `/`). Brug try/catch så **ugyldig operation** og **division med nul** håndteres pænt.

:::

:::code-playground
```csharp
// gf2-input: 10
// gf2-input: 0
// gf2-input: /
Console.WriteLine("Opgave 8:");
Console.WriteLine("Lommeregner med + og / og try/catch.");
// TODO: Lav opgave 8 herunder!
```
:::

:::solution

```csharp
try
{
    Console.Write("Tal 1: ");
    int a = int.Parse(Console.ReadLine()!);
    Console.Write("Tal 2: ");
    int b = int.Parse(Console.ReadLine()!);
    Console.Write("Operation (+ eller /): ");
    string op = Console.ReadLine()!.Trim();

    if (op == "+")
        Console.WriteLine(a + b);
    else if (op == "/")
        Console.WriteLine(a / b);
    else
        Console.WriteLine("Ukendt operation");
}
catch (DivideByZeroException)
{
    Console.WriteLine("Division med nul er ikke tilladt.");
}
catch (Exception)
{
    Console.WriteLine("Ugyldigt input.");
}
```

:::
