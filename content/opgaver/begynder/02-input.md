---
title: "Input"
order: 2
difficulty: begynder
category: grundlaeg
related_pensum: [03-operatorer-og-udtryk, 04-betingelser]
kompetencemaal:
  - "Kan læse brugerinput med Console.ReadLine"
  - "Kan konvertere input til string, int, double og bool"
  - "Kan kombinere flere inputs i et lille program"
---

# Kapitel 2 — Input

Seks opgaver om at læse fra «konsollen» og gemme input i variabler. Skriv din kode **efter `// TODO`** i editoren under hver opgave.

:::callout type="tip"
Læs pensum [Betingelser og brugerinput](/curriculum/04-betingelser) og [Operatorer og udtryk](/curriculum/03-operatorer-og-udtryk). I browseren simuleres tastaturet med linjerne `// gf2-input:` — du kan ændre testværdierne og trykke **Kør** igen.
:::

---

## Opgave 1 — `string`

:::exercise level="begynder"

Lav et program, der gemmer et input som en **string** og skriver strengen ud i konsollen.

:::

:::code-playground
```csharp
// gf2-input: Hej fra tastaturet
Console.WriteLine("Lav et program som gemmer et input som en string og skriver strengen ud i konsollen");
Console.WriteLine("Indtast en streng: ");
// TODO: Lav opgave 1 herunder! Tip: string s = Console.ReadLine();
```
:::

:::solution

```csharp
string s = Console.ReadLine()!;
Console.WriteLine(s);
```

:::

---

## Opgave 2 — `int`

:::exercise level="begynder"

Lav et program, der gemmer et input som et **heltal** og skriver tallet ud.

:::

:::code-playground
```csharp
// gf2-input: 42
Console.WriteLine("Lav et program som gemmer et input som et tal og skriver tallet ud i konsollen");
Console.WriteLine("Indtast et tal: ");
// TODO: Lav opgave 2 herunder! Tip: int number = int.Parse(Console.ReadLine());
```
:::

:::solution

```csharp
int number = int.Parse(Console.ReadLine()!);
Console.WriteLine(number);
```

:::

---

## Opgave 3 — `double`

:::exercise level="begynder"

Lav et program, der gemmer et input som et **decimaltal** og skriver tallet ud. Brug punktum i tallet (fx `5.25`).

:::

:::code-playground
```csharp
Console.WriteLine("Lav et program som gemmer et input som et decimaltal og skriver tallet ud i konsollen");
Console.WriteLine("Indtast et decimaltal: ");
// TODO: Lav opgave 3 herunder! Tip: double d = double.Parse(Console.ReadLine());
```
:::

:::solution

```csharp
double d = double.Parse(Console.ReadLine()!);
Console.WriteLine(d);
```

:::

---

## Opgave 4 — `bool`

:::exercise level="begynder"

Lav et program, der gemmer et input som en **sandhedsværdi** (`true` eller `false`) og skriver værdien ud.

:::

:::code-playground
```csharp
// gf2-input: true
Console.WriteLine("Lav et program som gemmer et input som en sandhedsværdi og skriver værdien ud i konsollen");
Console.WriteLine("Indtast true eller false: ");
// TODO: Lav opgave 4 herunder! Tip: bool b = bool.Parse(Console.ReadLine());
```
:::

:::solution

```csharp
bool b = bool.Parse(Console.ReadLine()!);
Console.WriteLine(b);
```

:::

---

## Opgave 5 — Mini-projekt: Personlig profil

:::exercise level="begynder"

Lav et program, hvor brugeren indtaster **navn**, **alder** og **hjemby**. Gem oplysningerne i variabler og udskriv en præsentation, fx: *Hej, jeg hedder X, er X år gammel og kommer fra X!*

:::

:::code-playground
```csharp
// gf2-input: navn: Mads
// gf2-input: alder: 17
// gf2-input: hjemby: Aalborg
Console.WriteLine("Mini-projekt: Personlig profil");
Console.WriteLine("Opgave:");
Console.WriteLine("Lav et program, hvor brugeren indtaster sit navn, alder og hjemby.");
Console.WriteLine("Gem oplysningerne i variabler og udskriv en præsentationstekst, der bruger alle oplysningerne.");
Console.WriteLine("Eksempel: Hej, jeg hedder X, er X år gammel og kommer fra X!");
// TODO: Lav opgave 5 herunder!
```
:::

:::solution

```csharp
Console.Write("Navn: ");
string name = Console.ReadLine()!;
Console.Write("Alder: ");
int age = int.Parse(Console.ReadLine()!);
Console.Write("Hjemby: ");
string hometown = Console.ReadLine()!;
Console.WriteLine($"Hej, jeg hedder {name}, er {age} år gammel og kommer fra {hometown}!");
```

:::

---

## Opgave 6 — Mini-projekt: BMI-beregner

:::exercise level="begynder"

Lav et program, hvor brugeren indtaster **vægt (kg)** og **højde (meter)**. Beregn og udskriv **BMI** = vægt / (højde × højde).

:::

:::code-playground
```csharp
// gf2-input: vaegt: 70
// gf2-input: hoejde: 1.75
Console.WriteLine("Mini-projekt 2: BMI-beregner");
Console.WriteLine("Opgave:");
Console.WriteLine("Lav et program, hvor brugeren indtaster sin vægt (i kg) og højde (i meter).");
Console.WriteLine("Programmet skal beregne brugerens BMI og udskrive resultatet.");
Console.WriteLine("Tip: BMI beregnes som vægt divideret med højde i anden (BMI = vægt / (højde * højde)).");
// TODO: Lav opgave 6 herunder!
```
:::

:::solution

```csharp
Console.Write("Vægt (kg): ");
double weight = double.Parse(Console.ReadLine()!);
Console.Write("Højde (m): ");
double height = double.Parse(Console.ReadLine()!);
double bmi = weight / (height * height);
Console.WriteLine($"Din BMI er {bmi:F2}");
```

:::

:::git-step
commit: "feat: opgaver kapitel 2 input"
branch: main
:::

:::related-pensum
- 03-operatorer-og-udtryk
- 04-betingelser
:::
