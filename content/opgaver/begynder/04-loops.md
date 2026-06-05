---
title: "Loops"
order: 4
difficulty: begynder
category: logik
related_pensum: [05-loekker]
kompetencemaal:
  - "Kan bruge for-, while- og do-while-løkker til gentagne opgaver"
  - "Kan kombinere løkker med if-betingelser og brugerinput"
  - "Kan løse små problemer med summering, tælling og gentagen udskrift"
---

# Kapitel 4 — Loops

Tolv opgaver om løkker — med og uden brugerinput. Skriv din kode **efter `// TODO`** i editoren under hver opgave.

:::callout type="tip"
Læs pensum [Loops](/curriculum/05-loekker). Ved input-opgaver kan du ændre testværdier i `// gf2-input:` og trykke **Kør** igen.
:::

---

## Opgave 1 — 1 til 10

:::exercise level="begynder"

Brug et loop til at udskrive tallene **fra 1 til 10**.

:::

:::code-playground
```csharp
Console.WriteLine("Opgave 1:");
Console.WriteLine("Brug et loop til at udskrive tallene fra 1 til 10.");
// TODO: Lav opgave 1 herunder!
```
:::

:::solution

```csharp
for (int i = 1; i <= 10; i++)
    Console.WriteLine(i);
```

:::

---

## Opgave 2 — Lige tal 2–20

:::exercise level="begynder"

Brug et loop og en **if**-betingelse til at udskrive alle **lige tal fra 2 til 20**.

:::

:::code-playground
```csharp
Console.WriteLine("Opgave 2:");
Console.WriteLine("Brug et loop og en if-betingelse til at udskrive alle lige tal fra 2 til 20.");
// TODO: Lav opgave 2 herunder!
```
:::

:::solution

```csharp
for (int i = 2; i <= 20; i++)
{
    if (i % 2 == 0)
        Console.WriteLine(i);
}
```

:::

---

## Opgave 3 — Sum 1–100

:::exercise level="begynder"

Brug et loop til at lægge alle tal fra **1 til 100** sammen og udskriv resultatet.

:::

:::code-playground
```csharp
Console.WriteLine("Opgave 3:");
Console.WriteLine("Brug et loop til at lægge alle tal fra 1 til 100 sammen og udskriv resultatet.");
// TODO: Lav opgave 3 herunder!
```
:::

:::solution

```csharp
int sum = 0;
for (int i = 1; i <= 100; i++)
    sum += i;
Console.WriteLine(sum);
```

:::

---

## Opgave 4 — Navn gentaget

:::exercise level="begynder"

Bed brugeren om at indtaste **navn** og et **tal**. Udskriv navnet det antal gange med et loop.

:::

:::code-playground
```csharp
// gf2-input: navn: Anna
// gf2-input: antal: 3
Console.WriteLine("Opgave 4:");
Console.WriteLine("Bed brugeren om at indtaste sit navn og et tal. Udskriv navnet det antal gange ved hjælp af et loop.");
// TODO: Lav opgave 4 herunder!
```
:::

:::solution

```csharp
Console.Write("Navn: ");
string navn = Console.ReadLine()!;
Console.Write("Antal: ");
int antal = int.Parse(Console.ReadLine()!);
for (int i = 0; i < antal; i++)
    Console.WriteLine(navn);
```

:::

---

## Opgave 5 — Nedtælling

:::exercise level="begynder"

Bed brugeren om et tal. Brug et loop til at udskrive alle tal fra det indtastede tal **ned til 1**.

:::

:::code-playground
```csharp
// gf2-input: 5
Console.WriteLine("Opgave 5:");
Console.WriteLine("Bed brugeren om at indtaste et tal. Brug et loop til at udskrive alle tal fra det indtastede tal og ned til 1.");
// TODO: Lav opgave 5 herunder!
```
:::

:::solution

```csharp
Console.Write("Tal: ");
int start = int.Parse(Console.ReadLine()!);
for (int i = start; i >= 1; i--)
    Console.WriteLine(i);
```

:::

---

## Opgave 6 — Bogstaver i navn

:::exercise level="begynder"

Brug et loop til at udskrive **alle bogstaver** i dit navn (ét bogstav pr. linje). Navnet skal være gemt i en **string**-variabel.

:::

:::code-playground
```csharp
Console.WriteLine("Opgave 6:");
Console.WriteLine("Brug et loop til at udskrive alle bogstaverne i dit navn (ét bogstav pr. linje).");
Console.WriteLine("Navnet skal være gemt i en string variabel.");
string navn = "Mads";
// TODO: Lav opgave 6 herunder!
```
:::

:::solution

```csharp
string navn = "Mads";
for (int i = 0; i < navn.Length; i++)
    Console.WriteLine(navn[i]);
```

:::

---

## Opgave 7 — Tæl bogstavet «a»

:::exercise level="begynder"

Brug et loop til at tælle, hvor mange gange bogstavet **«a»** optræder i en tekst, som brugeren indtaster.

:::

:::code-playground
```csharp
// gf2-input: abrakadabra
Console.WriteLine("Opgave 7:");
Console.WriteLine("Brug et loop til at tælle, hvor mange gange bogstavet 'a' optræder i en tekst, som brugeren indtaster.");
// TODO: Lav opgave 7 herunder!
```
:::

:::solution

```csharp
Console.Write("Tekst: ");
string tekst = Console.ReadLine()!.ToLower();
int antal = 0;
for (int i = 0; i < tekst.Length; i++)
{
    if (tekst[i] == 'a')
        antal++;
}
Console.WriteLine($"Bogstavet 'a' forekommer {antal} gang(e).");
```

:::

---

## Opgave 8 — Ulige 1–50

:::exercise level="begynder"

Brug et loop til at udskrive alle **ulige tal mellem 1 og 50**.

:::

:::code-playground
```csharp
Console.WriteLine("Opgave 8:");
Console.WriteLine("Brug et loop til at udskrive alle ulige tal mellem 1 og 50.");
// TODO: Lav opgave 8 herunder!
```
:::

:::solution

```csharp
for (int i = 1; i <= 50; i++)
{
    if (i % 2 != 0)
        Console.WriteLine(i);
}
```

:::

---

## Opgave 9 — Sum af 5 tal

:::exercise level="begynder"

Bed brugeren om at indtaste **5 tal** (ét ad gangen). Brug et loop til at lægge dem sammen og udskriv summen.

:::

:::code-playground
```csharp
// gf2-input: 10
// gf2-input: 20
// gf2-input: 5
// gf2-input: 15
// gf2-input: 0
Console.WriteLine("Opgave 9:");
Console.WriteLine("Bed brugeren om at indtaste 5 tal (ét ad gangen). Brug et loop til at lægge dem sammen og udskriv summen til sidst.");
// TODO: Lav opgave 9 herunder!
```
:::

:::solution

```csharp
int sum = 0;
for (int i = 1; i <= 5; i++)
{
    Console.Write($"Tal {i}: ");
    sum += int.Parse(Console.ReadLine()!);
}
Console.WriteLine($"Summen er {sum}");
```

:::

---

## Opgave 10 — Gæt et tal

:::exercise level="begynder"

Lav et program, hvor brugeren skal **gætte et hemmeligt tal mellem 1 og 10**. Brug et loop, så brugeren kan gætte, indtil det rigtige tal er fundet.

:::

:::code-playground
```csharp
// gf2-input: 3
// gf2-input: 7
// gf2-input: 5
Console.WriteLine("Opgave 10:");
Console.WriteLine("Lav et program, hvor brugeren skal gætte et hemmeligt tal mellem 1 og 10.");
Console.WriteLine("Brug et loop, så brugeren kan gætte indtil det rigtige tal er fundet.");
// TODO: Lav opgave 10 herunder!
```
:::

:::solution

```csharp
int hemmeligt = 7;
int gæt;
do
{
    Console.Write("Gæt et tal (1-10): ");
    gæt = int.Parse(Console.ReadLine()!);
    if (gæt != hemmeligt)
        Console.WriteLine("Forkert — prøv igen!");
} while (gæt != hemmeligt);
Console.WriteLine("Rigtigt!");
```

:::

---

## Opgave 11 — BankeBøf

:::exercise level="begynder"

Lav et program med et loop, der udskriver **1 til 30**. Udskriv **Banke** ved delelig med 3, **Bøf** ved delelig med 5, og **BankeBøf** ved delelig med både 3 og 5.

:::

:::code-playground
```csharp
Console.WriteLine("Lav et program med et loop, som udskriver tallene fra 1 til 30.");
Console.WriteLine("Udskriv 'Banke' hvis tallet er deleligt med 3, 'Bøf' hvis tallet er deleligt med 5");
Console.WriteLine("og 'BankeBøf' hvis tallet er deleligt med både 3 og 5.");
// TODO: Lav opgave 11 herunder!
```
:::

:::solution

```csharp
for (int i = 1; i <= 30; i++)
{
    if (i % 3 == 0 && i % 5 == 0)
        Console.WriteLine("BankeBøf");
    else if (i % 3 == 0)
        Console.WriteLine("Banke");
    else if (i % 5 == 0)
        Console.WriteLine("Bøf");
    else
        Console.WriteLine(i);
}
```

:::

---

## Opgave 12 — Mini-projekt: Lommeregner

:::exercise level="begynder"

Lav et program, hvor brugeren indtaster **to tal** og vælger en **regneart** (`+`, `-`, `*`, `/`). Udregn og udskriv resultatet (brug if/else eller switch).

:::

:::code-playground
```csharp
// gf2-input: tal1: 10
// gf2-input: tal2: 4
// gf2-input: tegn: +
Console.WriteLine("Mini-projekt: Simpel lommeregner");
Console.WriteLine("Lav et program, hvor brugeren indtaster to tal og vælger en regneart (+, -, * eller /).");
Console.WriteLine("Programmet skal udregne og udskrive resultatet.");
Console.WriteLine("Tip: Brug if/else eller switch til at vælge regnearten.");
// TODO: Lav opgave 12 herunder!
```
:::

:::solution

```csharp
Console.Write("Første tal: ");
double a = double.Parse(Console.ReadLine()!);
Console.Write("Andet tal: ");
double b = double.Parse(Console.ReadLine()!);
Console.Write("Regneart (+, -, *, /): ");
string tegn = Console.ReadLine()!.Trim();

switch (tegn)
{
    case "+":
        Console.WriteLine(a + b);
        break;
    case "-":
        Console.WriteLine(a - b);
        break;
    case "*":
        Console.WriteLine(a * b);
        break;
    case "/":
        Console.WriteLine(a / b);
        break;
    default:
        Console.WriteLine("Ukendt regneart");
        break;
}
```

:::

:::git-step
commit: "feat: opgaver kapitel 4 loops"
branch: main
:::

:::related-pensum
- 05-loekker
:::
