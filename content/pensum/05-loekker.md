---
title: "Loops"
order: 5
topics: [loekker, for, while, foreach, iteration]
kompetencemaal:
  - "Kan forklare forskellen på for, while, do-while og foreach"
  - "Kan bruge loops til at gentage kode baseret på en betingelse"
  - "Kan anvende break og continue til at styre et loop"
  - "Kan iterere over arrays, lister og dictionaries med for og foreach"
  - "Kan bruge nested loops til at gennemløbe lister af lister"
timer: 3
---

# Løkker og iterationer

**Loops** (løkker) er grundlæggende programmeringsteknikker, der gentager en blok kode, indtil en betingelse er opfyldt. I C# findes `for`, `while`, `do-while` og `foreach` — hver med sit eget brugsscenarie.

| Løkke | Brug når |
|-------|----------|
| `for` | Du ved, hvor mange gange du skal iterere |
| `while` | Gentagelse baseret på en betingelse |
| `do-while` | Mindst én iteration skal køre |
| `foreach` | Gennemløb af en samling uden indeks |

## for-løkken

`for` bruges, når du ved, hvor mange gange koden skal gentages. Den har tre dele: **initialisering**, **betingelse** og **opdatering**.

```csharp
for (int i = 0; i < 10; i++)
{
    Console.WriteLine("i = " + i);
}
```

- `int i = 0` — startværdi (kører én gang)
- `i < 10` — betingelse (tjekkes før hver iteration)
- `i++` — opdatering (kører efter hver iteration, samme som `i = i + 1`)

Loopet udskriver tallene 0 til 9.


:::video-list
- [Branching and While Loops [Pt 10] | C# for Beginners](https://www.youtube.com/watch?v=GLzAkJb0eoA)
- [For Loops [Pt 11] | C# for Beginners](https://www.youtube.com/watch?v=yuRdCZMuak0)
:::


## while-løkken

`while` gentager en blok, **så længe** betingelsen er sand — tjekkes *før* hver iteration:

```csharp
int i = 0;
while (i < 10)
{
    Console.WriteLine("i = " + i);
    i++;
}
```

Brug `while`, når antallet af iterationer afhænger af en betingelse — fx brugerinput eller en flag-variabel.


## do-while-løkken

`do-while` minder om `while`, men kører **mindst én gang**, fordi betingelsen tjekkes *efter* koden:

```csharp
int i = 0;
do
{
    Console.WriteLine("i = " + i);
    i++;
} while (i < 10);
```

Brug `do-while`, når menuen eller prompten skal vises mindst én gang — fx "Vil du prøve igen?"


## foreach-løkken

`foreach` itererer over alle elementer i en **samling** uden at håndtere indeks:

```csharp
List<string> names = new List<string> { "Alice", "Bob", "Charlie" };

foreach (string name in names)
{
    Console.WriteLine("Navn: " + name);
}
```

`foreach` er den simpleste løkke, når du bare skal læse hvert element. Du kan **ikke** ændre samlingens størrelse under en `foreach` (fx `Add`/`Remove`) — det giver runtime-fejl.


## break og continue

**break** afslutter løkken med det samme:

```csharp
for (int i = 0; i < 10; i++)
{
    if (i == 5)
        break;
    Console.WriteLine("i = " + i);
}
// Output: 0, 1, 2, 3, 4
```

**continue** springer resten af den **nuværende** iteration over og går videre til næste:

```csharp
for (int i = 0; i < 10; i++)
{
    if (i % 2 == 0)
        continue;
    Console.WriteLine("i = " + i);
}
// Output: 1, 3, 5, 7, 9 (kun ulige tal)
```

Brug `break` når du har fundet det, du leder efter. Brug `continue` til at springe ugyldige elementer over.


## Iteration over arrays

Et **array** er en samling af elementer af samme type med fast længde.

**Med `for`** — når du har brug for indeks:

```csharp
int[] numbers = { 10, 20, 30, 40, 50 };

for (int i = 0; i < numbers.Length; i++)
{
    Console.WriteLine("Element: " + numbers[i]);
}
```

**Med `foreach`** — når du bare skal læse elementer:

```csharp
int[] numbers = { 1, 2, 3, 4, 5 };

foreach (int number in numbers)
{
    Console.WriteLine("Element: " + number);
}
```


## Iteration over lister

En **List** er dynamisk — du kan tilføje og fjerne elementer. Begge løkketyper virker:

**Med `for`:**

```csharp
List<string> names = new List<string> { "Alice", "Bob", "Charlie" };

for (int i = 0; i < names.Count; i++)
{
    Console.WriteLine("Navn: " + names[i]);
}
```

**Med `foreach`:**

```csharp
List<string> names = new List<string> { "Alice", "Bob", "Charlie" };

foreach (string name in names)
{
    Console.WriteLine("Navn: " + name);
}
```


## Nested loops — lister af lister

Når hvert element også er en liste (fx en **matrix**), bruger du **nested loops** — en løkke inde i en anden:

```csharp
List<List<int>> listOfLists = new List<List<int>>
{
    new List<int> { 1, 2, 3 },
    new List<int> { 4, 5, 6 },
    new List<int> { 7, 8, 9 }
};

for (int i = 0; i < listOfLists.Count; i++)
{
    for (int j = 0; j < listOfLists[i].Count; j++)
    {
        Console.WriteLine("Element: " + listOfLists[i][j]);
    }
}
```

**Med `foreach`:**

```csharp
List<List<int>> listOfLists = new List<List<int>>
{
    new List<int> { 1, 2, 3 },
    new List<int> { 4, 5, 6 },
    new List<int> { 7, 8, 9 }
};

foreach (List<int> list in listOfLists)
{
    foreach (int number in list)
    {
        Console.WriteLine("Element: " + number);
    }
}
```

:::callout type="info"
Nested loops bruger du i praksis til fx **IP-adresse-konvertering** og binær matematik — se kapitlet [Binary Numbers & IP Addresses](/curriculum/08-binaer-og-ip).
:::


## Iteration over Dictionary

En **Dictionary** gemmer **nøgle/værdi-par**. `foreach` er den naturlige måde at gennemløbe den:

```csharp
Dictionary<string, int> ages = new Dictionary<string, int>
{
    { "Alice", 25 },
    { "Bob", 30 },
    { "Charlie", 35 }
};

foreach (KeyValuePair<string, int> kvp in ages)
{
    Console.WriteLine("Navn: " + kvp.Key + ", Alder: " + kvp.Value);
}
```

Du kan også iterere over **nøgler** eller **værdier** separat:

```csharp
Dictionary<string, int> ages = new Dictionary<string, int>
{
    { "Alice", 25 },
    { "Bob", 30 },
    { "Charlie", 35 }
};

foreach (string key in ages.Keys)
    Console.WriteLine("Navn: " + key);

foreach (int value in ages.Values)
    Console.WriteLine("Alder: " + value);
```


## Praktisk eksempel — sum og gennemsnit

Løkker er essentielle til beregninger over mange værdier:

```csharp
int sum = 0;
int[] tal = { 10, 20, 30, 40, 50 };

for (int i = 0; i < tal.Length; i++)
    sum += tal[i];

double gennemsnit = (double)sum / tal.Length;
Console.WriteLine($"Sum: {sum}, Gennemsnit: {gennemsnit}");
```

Mønsteret **initialiser → loop → akkumuler** bruger du konstant i GF2.


:::git-step
commit: "feat: loekker og iteration"
branch: main
:::

## Opsummering

- `for` er bedst når du kender antal iterationer; `while` og `do-while` når betingelsen styrer loopet
- `foreach` er perfekt til arrays, lister og dictionaries
- `break` stopper løkken; `continue` springer til næste iteration
- Nested loops gennemløber lister af lister og 2D-data
- Dictionaries kan itereres som nøgle/værdi-par, eller kun keys/values


:::knowledge-check
---
q: Hvilken løkke bruger du, når du **kender antallet** af iterationer på forhånd?
- `while`
- `for`
- `do-while`
correct: 1
explain: **`for`** har initialisering, betingelse og opdatering i én linje — perfekt når du ved, hvor mange gange koden skal gentages, fx `for (int i = 0; i < 10; i++)`.
---
q: Hvad er forskellen på `while` og `do-while`?
- `while` tjekker betingelsen før hver iteration; `do-while` kører mindst én gang
- `do-while` kan kun bruges med arrays
- Der er ingen forskel
correct: 0
explain: **`while`** tjekker betingelsen *før* koden kører — den kan springe over helt. **`do-while`** kører koden mindst **én gang** og tjekker derefter betingelsen.
---
q: Hvad gør **`break`** i en løkke?
- Springer resten af den nuværende iteration over
- Afslutter løkken med det samme
- Genstarter løkken fra begyndelsen
correct: 1
explain: **`break`** stopper løkken helt og fortsætter koden efter løkken. Brug det, når du har fundet det, du leder efter, og ikke har brug for flere iterationer.
---
q: Hvad gør **`continue`** i en løkke?
- Afslutter hele programmet
- Springer resten af den nuværende iteration over og går til næste
- Sletter det nuværende element i listen
correct: 1
explain: **`continue`** hopper til næste iteration uden at køre resten af koden i den nuværende. Fx spring lige tal over med `if (i % 2 == 0) continue;`.
---
q: Hvorfor bør du undgå at kalde `.Add()` på en liste **under en `foreach`**?
- Det gør koden langsommere
- Det giver en runtime-fejl — samlingen må ikke ændres under foreach
- `.Add()` virker kun i `for`-løkker
correct: 1
explain: Under en **`foreach`** må samlingens størrelse ikke ændres. Tilføjelse eller fjernelse under gennemløb giver fejl. Brug **`for`** baglæns, hvis du skal fjerne elementer under iteration.
---
q: Hvad er forskellen på `.Length` på et array og `.Count` på en liste?
- De betyder det samme — brug `.Length` overalt
- `.Length` på array, `.Count` på `List<T>` — begge giver antal elementer
- `.Count` virker kun på strings
correct: 1
explain: Arrays bruger **`.Length`**, lister bruger **`.Count`**. Begge fortæller, hvor mange elementer der er — men syntaksen er forskellig, fordi det er forskellige typer.
---
q: Hvordan gennemløber du en **Dictionary** med både nøgle og værdi?
- `foreach (string key in dict)` — værdien hentes automatisk
- `foreach (KeyValuePair<K,V> kvp in dict)` og brug `kvp.Key` og `kvp.Value`
- Dictionaries kan ikke itereres — kun opslag med `[key]`
correct: 1
explain: **`foreach`** over en dictionary giver **`KeyValuePair`**-elementer med **`.Key`** og **`.Value`**. Du kan også iterere over `.Keys` eller `.Values` separat.
:::
