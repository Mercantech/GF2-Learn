---
title: "Try/catch"
order: 9
difficulty: begynder
category: fejl
related_pensum: [14-fejlfinding, 02-variabler-og-datatyper]
kompetencemaal:
  - "Kan genkende runtime-fejl når programmet ikke håndterer dem"
  - "Kan bruge try/catch til at håndtere runtime-fejl"
  - "Kan fange specifikke exceptions (fx division med nul)"
  - "Kan kombinere try/catch med TryParse til robust input"
  - "Kan skrive brugervenlige fejlbeskeder i stedet for crash"
---

# Kapitel 8 — Try/catch

Ti opgaver om **exceptions** og **try/catch** — først uden fejlhåndtering, så du kan *se* programmet fejle; derefter lærer du at fange fejlene. Skriv din kode **efter `// TODO`** i editoren.

:::callout type="tip"
Læs pensum [Fejlfinding](/curriculum/14-fejlfinding). I **opgave 1–2** skriver du bevidst **uden** try/catch — kør programmet og læg mærke til fejlbeskeden. Fra **opgave 3** tilføjer du try/catch. Test med `// gf2-input:` — skift værdierne og tryk **Kør** igen.
:::

---

## Opgave 1 — Se fejlen ved ugyldigt input

:::exercise level="begynder"

Lav et program, der dividerer **10** med et tal fra brugeren — **uden try/catch**. Kør med testinput **`abc`** og se, hvad der sker når `int.Parse` ikke kan læse tallet.

:::

:::code-playground
```csharp
// gf2-input: abc
Console.WriteLine("Opgave 1:");
Console.WriteLine("Divider 10 med et tal — UDEN try/catch. Kør og se fejlen ved ugyldigt input.");
// TODO: Lav opgave 1 herunder! Tip: int.Parse og division
```
:::

:::solution

```csharp
Console.Write("Tal: ");
int n = int.Parse(Console.ReadLine()!);
Console.WriteLine(10 / n);
```

:::

---

## Opgave 2 — Se fejlen ved division med nul

:::exercise level="begynder"

Lav division **uden try/catch**. Brug testinput **`0`** og kør — programmet fejler med en **division-med-nul**-fejl.

:::

:::code-playground
```csharp
// gf2-input: 0
Console.WriteLine("Opgave 2:");
Console.WriteLine("Divider 100 med et tal — UDEN try/catch. Kør med 0 og se fejlen.");
// TODO: Lav opgave 2 herunder!
```
:::

:::solution

```csharp
Console.Write("Tal: ");
int n = int.Parse(Console.ReadLine()!);
Console.WriteLine(100 / n);
```

:::

---

## Opgave 3 — Grundlæggende try/catch

:::exercise level="begynder"

Tag udgangspunkt i opgave 1: divider **10** med et tal fra brugeren. Brug nu **try/catch**, så programmet ikke crasher ved ugyldigt input.

:::

:::code-playground
```csharp
// gf2-input: abc
Console.WriteLine("Opgave 3:");
Console.WriteLine("Divider 10 med et tal — nu med try/catch.");
// TODO: Lav opgave 3 herunder!
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

## Opgave 4 — Division med nul

:::exercise level="begynder"

Tag udgangspunkt i opgave 2: fang **`DivideByZeroException`** når brugeren indtaster **0**, og udskriv en pæn fejlbesked.

:::

:::code-playground
```csharp
// gf2-input: 0
Console.WriteLine("Opgave 4:");
Console.WriteLine("Fang DivideByZeroException ved division med 0.");
// TODO: Lav opgave 4 herunder!
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

## Opgave 5 — Flere catch-blokke

:::exercise level="begynder"

Lav **to catch-blokke**: én til **division med nul** og én til **andre fejl** (fx ugyldigt tal).

:::

:::code-playground
```csharp
// gf2-input: abc
Console.WriteLine("Opgave 5:");
Console.WriteLine("Brug flere catch-blokke til nul og andre fejl.");
// TODO: Lav opgave 5 herunder!
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

## Opgave 6 — `ex.Message`

:::exercise level="begynder"

Fang en exception og udskriv **`ex.Message`** i en generel **catch (Exception ex)**-blok.

:::

:::code-playground
```csharp
// gf2-input: xyz
Console.WriteLine("Opgave 6:");
Console.WriteLine("Udskriv ex.Message i catch (Exception ex).");
// TODO: Lav opgave 6 herunder!
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

## Opgave 7 — `int.TryParse`

:::exercise level="begynder"

Lav robust input med **`int.TryParse`**. Ved fejl: udskriv **«Ugyldigt tal»** — uden try/catch.

:::

:::code-playground
```csharp
// gf2-input: 12
// gf2-input: hej
Console.WriteLine("Opgave 7:");
Console.WriteLine("Brug int.TryParse — spørg indtil brugeren indtaster et gyldigt tal.");
// TODO: Lav opgave 7 herunder!
```
:::

:::solution

```csharp
int tal;
while (true)
{
    Console.Write("Indtast et tal: ");
    if (int.TryParse(Console.ReadLine(), out tal))
        break;
    Console.WriteLine("Ugyldigt tal");
}
Console.WriteLine($"Du indtastede {tal}");
```

:::

---

## Opgave 8 — `finally`

:::exercise level="begynder"

Brug **try/catch/finally**. I **finally** udskrives altid **«Færdig med beregning»**.

:::

:::code-playground
```csharp
// gf2-input: 5
Console.WriteLine("Opgave 8:");
Console.WriteLine("Brug try/catch/finally og udskriv i finally.");
// TODO: Lav opgave 8 herunder!
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

## Opgave 9 — Metode med try/catch

:::exercise level="begynder"

Lav en metode **`UdskrivKvotient(int a, int b)`** med try/catch omkring divisionen. Kald metoden med to input-tal.

:::

:::code-playground
```csharp
// gf2-input: 20
// gf2-input: 4
Console.WriteLine("Opgave 9:");
Console.WriteLine("Lav en metode med try/catch omkring division.");
// TODO: Lav opgave 9 herunder!
```
:::

:::solution

```csharp
void UdskrivKvotient(int a, int b)
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
UdskrivKvotient(a, b);
```

:::

---

## Opgave 10 — Robust lommeregner

:::exercise level="begynder"

Lav en lille **lommeregner**: to tal og en operation (`+` eller `/`). Brug try/catch så **ugyldig operation** og **division med nul** håndteres pænt.

:::

:::code-playground
```csharp
// gf2-input: 10
// gf2-input: 0
// gf2-input: /
Console.WriteLine("Opgave 10:");
Console.WriteLine("Lommeregner med + og / og try/catch.");
// TODO: Lav opgave 10 herunder!
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
