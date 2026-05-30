---
title: "Hello World"
order: 1
difficulty: begynder
related_pensum: [01-introduktion]
kompetencemaal:
  - "Kan skrive og køre et simpelt C# program"
---

:::exercise level="begynder"

Skriv et program der skriver **Hej GF2!** og dit navn på næste linje.

:::

:::code-playground
```csharp
Console.WriteLine("Hej GF2!");
Console.WriteLine("Mit navn er ...");
```
expected: Hej GF2!
:::

:::git-step
commit: "feat: hello world opgave"
branch: main
:::

:::solution

```csharp
Console.WriteLine("Hej GF2!");
Console.WriteLine("Mit navn er Ada");
```

:::

:::related-pensum
- 01-introduktion
:::
