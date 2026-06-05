---
title: "Arrays, lister og dictionaries"
order: 6
difficulty: begynder
category: samlinger
related_pensum: [07-datastrukturer, 05-loekker]
kompetencemaal:
  - "Kan oprette og bruge arrays med fast størrelse"
  - "Kan bruge List til dynamiske samlinger med Add og Remove"
  - "Kan bruge Dictionary til key-value data"
  - "Kan kombinere loops og input med arrays, lister og dictionaries"
---

# Kapitel 5 — Arrays, lister og dictionaries

Tolv opgaver om **arrays**, **lister** og **dictionaries** — plus to mini-projekter. Skriv din kode **efter `// TODO`** i editoren under hver opgave.

:::callout type="tip"
Læs pensum [Arrays, lister og dictionaries](/curriculum/07-datastrukturer). Du må gerne bruge **loops** (fx `for`) og **input** fra de tidligere kapitler. Ved input-opgaver kan du ændre testværdier i `// gf2-input:` og trykke **Kør** igen.
:::

---

## Array — Opgave 1

:::exercise level="begynder"

Lav et program, der gemmer **5 fornavne** som brugeren indtaster i et **array**. Udskriv navnene til sidst.

:::

:::code-playground
```csharp
// gf2-input: Anna
// gf2-input: Bo
// gf2-input: Clara
// gf2-input: David
// gf2-input: Eva
Console.WriteLine("Opgave 1 (Array):");
Console.WriteLine("Lav et program som gemmer 5 fornavne som brugeren indtaster i et array. Udskriv navnene til sidst.");
string[] navne = new string[5];
// TODO: Lav opgave 1 herunder! Tip: brug et for-loop med navne[i] = Console.ReadLine();
```
:::

:::solution

```csharp
for (int i = 0; i < navne.Length; i++)
{
    Console.Write($"Fornavn {i + 1}: ");
    navne[i] = Console.ReadLine()!;
}
Console.WriteLine("Alle navne:");
for (int i = 0; i < navne.Length; i++)
    Console.WriteLine(navne[i]);
```

:::

---

## Array — Opgave 2

:::exercise level="begynder"

Lav et program, der gemmer **5 tal** i et array og udskriver det **største** tal.

:::

:::code-playground
```csharp
// gf2-input: 12
// gf2-input: 45
// gf2-input: 7
// gf2-input: 89
// gf2-input: 23
Console.WriteLine("Opgave 2 (Array):");
Console.WriteLine("Lav et program som gemmer 5 tal i et array og udskriver det største tal.");
// TODO: Lav opgave 2 herunder!
```
:::

:::solution

```csharp
int[] tal = new int[5];
for (int i = 0; i < tal.Length; i++)
{
    Console.Write($"Tal {i + 1}: ");
    tal[i] = int.Parse(Console.ReadLine()!);
}
int max = tal[0];
for (int i = 1; i < tal.Length; i++)
{
    if (tal[i] > max)
        max = tal[i];
}
Console.WriteLine($"Største tal: {max}");
```

:::

---

## Array — Opgave 3

:::exercise level="begynder"

Lav et program, der gemmer **5 bynavne** i et array og udskriver dem alle i **omvendt rækkefølge**.

:::

:::code-playground
```csharp
// gf2-input: København
// gf2-input: Aarhus
// gf2-input: Odense
// gf2-input: Aalborg
// gf2-input: Esbjerg
Console.WriteLine("Opgave 3 (Array):");
Console.WriteLine("Lav et program som gemmer 5 bynavne i et array og udskriver dem alle i omvendt rækkefølge.");
// TODO: Lav opgave 3 herunder!
```
:::

:::solution

```csharp
string[] byer = new string[5];
for (int i = 0; i < byer.Length; i++)
{
    Console.Write($"By {i + 1}: ");
    byer[i] = Console.ReadLine()!;
}
Console.WriteLine("Byer i omvendt rækkefølge:");
for (int i = byer.Length - 1; i >= 0; i--)
    Console.WriteLine(byer[i]);
```

:::

---

## List — Opgave 1

:::exercise level="begynder"

Lav et program, der gemmer **5 fornavne** som brugeren indtaster i en **liste**. Udskriv listen til sidst.

:::

:::code-playground
```csharp
// gf2-input: Kim
// gf2-input: Alex
// gf2-input: Sam
// gf2-input: Jo
// gf2-input: Pat
Console.WriteLine("Opgave 1 (List):");
Console.WriteLine("Lav et program som gemmer 5 fornavne som brugeren indtaster i en liste. Udskriv listen til sidst.");
List<string> navne = new List<string>();
// TODO: Lav opgave 4 herunder! Tip: brug navne.Add(Console.ReadLine()) i et loop.
```
:::

:::solution

```csharp
for (int i = 0; i < 5; i++)
{
    Console.Write($"Fornavn {i + 1}: ");
    navne.Add(Console.ReadLine()!);
}
Console.WriteLine("Alle navne:");
foreach (string navn in navne)
    Console.WriteLine(navn);
```

:::

---

## List — Opgave 2

:::exercise level="begynder"

Lav et program, hvor brugeren kan blive ved med at indtaste **navne**, indtil de skriver **`stop`**. Udskriv alle navnene til sidst.

:::

:::code-playground
```csharp
// gf2-input: Line
// gf2-input: Mads
// gf2-input: stop
Console.WriteLine("Opgave 2 (List):");
Console.WriteLine("Lav et program hvor brugeren kan blive ved med at indtaste navne indtil de skriver 'stop'. Udskriv alle navnene til sidst.");
// TODO: Lav opgave 5 herunder!
```
:::

:::solution

```csharp
List<string> navne = new List<string>();
while (true)
{
    Console.Write("Navn (eller 'stop'): ");
    string input = Console.ReadLine()!;
    if (input.Equals("stop", StringComparison.OrdinalIgnoreCase))
        break;
    navne.Add(input);
}
Console.WriteLine("Alle navne:");
foreach (string navn in navne)
    Console.WriteLine(navn);
```

:::

---

## List — Opgave 3

:::exercise level="begynder"

Lav et program, hvor brugeren indtaster **5 tal** i en liste, og programmet udskriver **gennemsnittet**.

:::

:::code-playground
```csharp
// gf2-input: 10
// gf2-input: 20
// gf2-input: 30
// gf2-input: 40
// gf2-input: 50
Console.WriteLine("Opgave 3 (List):");
Console.WriteLine("Lav et program hvor brugeren indtaster 5 tal i en liste og programmet udskriver gennemsnittet.");
// TODO: Lav opgave 6 herunder!
```
:::

:::solution

```csharp
List<int> tal = new List<int>();
for (int i = 0; i < 5; i++)
{
    Console.Write($"Tal {i + 1}: ");
    tal.Add(int.Parse(Console.ReadLine()!));
}
double sum = 0;
foreach (int t in tal)
    sum += t;
double gennemsnit = sum / tal.Count;
Console.WriteLine($"Gennemsnit: {gennemsnit}");
```

:::

---

## List — Opgave 4

:::exercise level="begynder"

Lav et program, hvor brugeren indtaster navne på ting, de skal købe, og kan **fjerne** ting fra listen igen. Udskriv listen til sidst.

:::

:::code-playground
```csharp
// gf2-input: mælk
// gf2-input: brød
// gf2-input: æg
// gf2-input: fjern brød
// gf2-input: stop
Console.WriteLine("Opgave 4 (List):");
Console.WriteLine("Lav et program hvor brugeren indtaster navne på ting de skal købe, og kan fjerne ting fra listen igen. Udskriv listen til sidst.");
Console.WriteLine("Tip: skriv 'fjern <ting>' for at fjerne, og 'stop' når du er færdig.");
// TODO: Lav opgave 7 herunder!
```
:::

:::solution

```csharp
List<string> indkoeb = new List<string>();
while (true)
{
    Console.Write("Tilføj ting, 'fjern <ting>' eller 'stop': ");
    string input = Console.ReadLine()!;
    if (input.Equals("stop", StringComparison.OrdinalIgnoreCase))
        break;
    if (input.StartsWith("fjern ", StringComparison.OrdinalIgnoreCase))
    {
        string ting = input.Substring(6).Trim();
        indkoeb.Remove(ting);
    }
    else
    {
        indkoeb.Add(input);
    }
}
Console.WriteLine("Indkøbsliste:");
foreach (string ting in indkoeb)
    Console.WriteLine(ting);
```

:::

---

## List — Opgave 5

:::exercise level="begynder"

Lav et program, hvor brugeren indtaster navne på sine **venner** i en liste, og programmet udskriver, hvor mange navne der **starter med «A»** (stort eller småt).

:::

:::code-playground
```csharp
// gf2-input: Anna
// gf2-input: Bo
// gf2-input: Anders
// gf2-input: Clara
// gf2-input: stop
Console.WriteLine("Opgave 5 (List):");
Console.WriteLine("Lav et program hvor brugeren indtaster navne på sine venner i en liste og programmet udskriver hvor mange navne der starter med 'A'.");
Console.WriteLine("Skriv 'stop' når du er færdig.");
// TODO: Lav opgave 8 herunder!
```
:::

:::solution

```csharp
List<string> venner = new List<string>();
while (true)
{
    Console.Write("Vennenavn (eller 'stop'): ");
    string navn = Console.ReadLine()!;
    if (navn.Equals("stop", StringComparison.OrdinalIgnoreCase))
        break;
    venner.Add(navn);
}
int antalA = 0;
foreach (string v in venner)
{
    if (v.StartsWith("A", StringComparison.OrdinalIgnoreCase))
        antalA++;
}
Console.WriteLine($"Navne der starter med 'A': {antalA}");
```

:::

---

## Dictionary — Opgave 1

:::exercise level="begynder"

Lav et program, hvor du gemmer **navn og alder** på **3 personer** i en **dictionary** og udskriver dem alle.

:::

:::code-playground
```csharp
Console.WriteLine("Opgave 1 (Dictionary):");
Console.WriteLine("Lav et program hvor du gemmer navne og alder på 3 personer i en dictionary og udskriver dem alle.");
// TODO: Lav opgave 9 herunder!
// Husk: Dictionary<string, int> personer = new Dictionary<string, int>();
```
:::

:::solution

```csharp
Dictionary<string, int> personer = new Dictionary<string, int>
{
    { "Anna", 17 },
    { "Bo", 18 },
    { "Clara", 16 }
};
foreach (var par in personer)
    Console.WriteLine($"{par.Key} er {par.Value} år");
```

:::

---

## Dictionary — Opgave 2

:::exercise level="begynder"

Lav et program, hvor brugeren kan indtaste et **navn** og få **alderen** på personen ud fra dictionaryen fra opgave 9.

:::

:::code-playground
```csharp
// gf2-input: Bo
Console.WriteLine("Opgave 2 (Dictionary):");
Console.WriteLine("Lav et program hvor brugeren kan indtaste et navn og få alderen på personen ud fra dictionaryen.");
// TODO: Lav opgave 10 herunder! Brug samme 3 personer som i opgave 9.
```
:::

:::solution

```csharp
Dictionary<string, int> personer = new Dictionary<string, int>
{
    { "Anna", 17 },
    { "Bo", 18 },
    { "Clara", 16 }
};
Console.Write("Indtast navn: ");
string navn = Console.ReadLine()!;
if (personer.TryGetValue(navn, out int alder))
    Console.WriteLine($"{navn} er {alder} år");
else
    Console.WriteLine("Personen findes ikke i listen.");
```

:::

---

## Mini-projekt — Klasseliste

:::exercise level="begynder"

Lav et program, hvor brugeren indtaster navnene på alle elever i en klasse (fx **5 navne**). Gem navnene i en **liste** og udskriv hele klasselisten i konsollen.

:::

:::code-playground
```csharp
// gf2-input: Emma
// gf2-input: Noah
// gf2-input: Olivia
// gf2-input: Lucas
// gf2-input: Sofia
Console.WriteLine("Mini-projekt: Klasseliste");
Console.WriteLine("Lav et program, hvor brugeren indtaster navnene på alle elever i en klasse (fx 5 navne).");
Console.WriteLine("Gem navnene i en liste og udskriv hele klasselisten i konsollen.");
// TODO: Lav opgave 11 herunder!
```
:::

:::solution

```csharp
List<string> klasse = new List<string>();
for (int i = 0; i < 5; i++)
{
    Console.Write($"Elev {i + 1}: ");
    klasse.Add(Console.ReadLine()!);
}
Console.WriteLine("Klasseliste:");
for (int i = 0; i < klasse.Count; i++)
    Console.WriteLine($"{i + 1}. {klasse[i]}");
```

:::

---

## Mini-projekt — Indkøbsliste

:::exercise level="begynder"

Lav et program, hvor brugeren indtaster **navn og pris** på **tre ting**, de skal købe. Gem tingene i et **key-value par** (dictionary) og udskriv en indkøbsliste med **total pris**.

:::

:::code-playground
```csharp
// gf2-input: mælk
// gf2-input: 12.50
// gf2-input: brød
// gf2-input: 25
// gf2-input: æg
// gf2-input: 28.95
Console.WriteLine("Mini-projekt: Indkøbsliste");
Console.WriteLine("Lav et program, hvor brugeren indtaster navnet på tre ting og deres pris.");
Console.WriteLine("Gem tingene i et key-value par med navn og pris, og udskriv en indkøbsliste med total pris.");
// TODO: Lav opgave 12 herunder!
```
:::

:::solution

```csharp
Dictionary<string, double> indkoeb = new Dictionary<string, double>();
for (int i = 0; i < 3; i++)
{
    Console.Write($"Vare {i + 1} navn: ");
    string navn = Console.ReadLine()!;
    Console.Write($"Pris for {navn}: ");
    double pris = double.Parse(Console.ReadLine()!);
    indkoeb[navn] = pris;
}
double total = 0;
Console.WriteLine("Indkøbsliste:");
foreach (var linje in indkoeb)
{
    Console.WriteLine($"{linje.Key}: {linje.Value:F2} kr.");
    total += linje.Value;
}
Console.WriteLine($"Total: {total:F2} kr.");
```

:::
