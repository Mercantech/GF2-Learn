---
title: "Iterating Collections"
order: 10
topics: [iteration, lister, arrays, dictionaries]
kompetencemaal:
  - "Kan iterere over lister, arrays og dictionaries"
  - "Kan bruge nested loops til komplekse datastrukturer"
  - "Kan forklare forskellen på iteration over keys og values"
  - "Kan kombinere datastrukturer og loops i en samlet løsning"
timer: 2
---

# Iteration over collections

At **iterere** betyder at gennemløbe en samling element for element. Du har lært `for`, `while`, `do-while` og `foreach` i kapitlet [Loops](/curriculum/05-loekker) — her fokuserer vi på at anvende dem på **arrays**, **lister** og **dictionaries**.

`foreach` er særligt velegnet til samlinger — du gennemløber alle elementer uden at håndtere indeks. Brug `for` med indeks, når du skal kende **positionen** eller ændre elementer på bestemte pladser.

:::callout type="info"
**Vælg loop:** `foreach` til simple gennemløb. `for` når du har brug for indeks eller nested strukturer. Se også [Arrays, Lists & Dictionaries](/curriculum/07-datastrukturer).
:::


## Iteration over arrays

Et **array** er en samling af samme type med fast længde.

**Med `for`** — når du har brug for indeks eller `.Length`:

```csharp
int[] numbers = { 10, 20, 30, 40, 50 };

for (int i = 0; i < numbers.Length; i++)
{
    Console.WriteLine("Element: " + numbers[i]);
}
```

**Med `foreach`** — når du bare skal læse hvert element:

```csharp
int[] numbers = { 1, 2, 3, 4, 5 };

foreach (int number in numbers)
{
    Console.WriteLine("Element: " + number);
}
```


## Iteration over lister

En **List&lt;T&gt;** er dynamisk — brug `.Count` i stedet for `.Length`.

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

:::callout type="warning"
Du kan **ikke** tilføje eller fjerne elementer i en liste under en `foreach` — det giver runtime-fejl. Brug `for` baglæns eller `ToList()` hvis du skal fjerne under gennemløb.
:::


## Nested loops — lister af lister (matrix)

Når hvert element også er en liste, bruger du **nested loops** — en løkke inde i en anden:

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

Den **ydre** løkke går gennem rækkerne; den **indre** gennem elementerne i hver række.


## Iteration i praksis — IP-omformeren

Nested loops og `while` med `break`/`continue` bruger du i det rigtige **IP/binær-projekt** — se [Binary Numbers & IP Addresses](/curriculum/08-binaer-og-ip).

Typisk mønster i omformeren:

- `while (!gyldig)` — spørg igen ved ugyldigt input
- `for (int i = 0; i < 4; i++)` — gennemløb de 4 oktetter
- `for (int j = 0; j < 8; j++)` — gennemløb de 8 bits per oktet
- `break` — stop validering ved fejl; `continue` — spring til næste forsøg

Det kombinerer loops, conditionals og strings (`Split`) i én løsning.


## Iteration over Dictionary

En **Dictionary&lt;TKey, TValue&gt;** gemmer nøgle/værdi-par. `foreach` er den naturlige måde at gennemløbe den:

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

**Kun nøgler:**

```csharp
Dictionary<string, int> ages = new Dictionary<string, int>
{
    { "Alice", 25 },
    { "Bob", 30 },
    { "Charlie", 35 }
};

foreach (string key in ages.Keys)
{
    Console.WriteLine("Navn: " + key);
}
```

**Kun værdier:**

```csharp
Dictionary<string, int> ages = new Dictionary<string, int>
{
    { "Alice", 25 },
    { "Bob", 30 },
    { "Charlie", 35 }
};

foreach (int value in ages.Values)
{
    Console.WriteLine("Alder: " + value);
}
```

**Deconstruction** (moderne, læsbar syntaks):

```csharp
Dictionary<string, int> ages = new Dictionary<string, int>
{
    { "Alice", 25 },
    { "Bob", 30 },
    { "Charlie", 35 }
};

foreach (var (navn, alder) in ages)
    Console.WriteLine($"{navn} er {alder} år");
```


## Supplerende: LINQ og IEnumerable

LINQ (**Language Integrated Query**) er en måde at filtrere, sortere og udvælge data fra collections uden at skrive alle loops selv. Du bruger det typisk ovenpå `List<T>`, arrays og andre typer, der kan gennemløbes som `IEnumerable<T>`.

:::video-list
- [Language Integrated Query (LINQ) and IEnumerable [Pt 15] | C# for Beginners](https://www.youtube.com/watch?v=4ro5UCqU0P4)
- [LINQ Query Expressions From, Where, Orderby, and Select [Pt 16] | C# for Beginners](https://www.youtube.com/watch?v=Wy1pvmcEqKw)
- [LINQ Method Syntax vs Query [Pt 17] | C# for Beginners](https://www.youtube.com/watch?v=jAPcP-QbCGA)
:::


## Filtrering og akkumulering

Iteration kombineres ofte med **if** og **sum**:

```csharp
var scores = new Dictionary<string, int>
{
    ["Ada"] = 95,
    ["Alan"] = 55,
    ["Grace"] = 88
};

// Filtrer — kun beståede
foreach (var (navn, score) in scores)
{
    if (score >= 60)
        Console.WriteLine($"{navn} bestod med {score}");
}

// Akkumuler — gennemsnit
int sum = 0;
foreach (var score in scores.Values)
    sum += score;
double gennemsnit = (double)sum / scores.Count;
Console.WriteLine($"Gennemsnit: {gennemsnit:F1}");
```


## Kombineret løsning — elever og fag

Dictionary af lister — nested `foreach` på to niveauer:

```csharp
var karakterer = new Dictionary<string, List<int>>
{
    ["Ada"] = new List<int> { 12, 10, 7 },
    ["Alan"] = new List<int> { 4, 7, 10 }
};

foreach (var (elev, fag) in karakterer)
{
    int sum = 0;
    foreach (var karakter in fag)
        sum += karakter;
    double snit = (double)sum / fag.Count;
    Console.WriteLine($"{elev}: gennemsnit {snit:F1}");
}
```

Det er præcis den slags struktur, du møder i GF2-projekter.


:::git-step
commit: "feat: iteration over collections"
branch: main
:::

## Opsummering

- **Arrays og lister** — både `for` (med indeks) og `foreach` (simpelt gennemløb)
- **Lister af lister** — nested loops (`for`/`foreach` indeni hinanden)
- **Dictionaries** — `KeyValuePair`, `.Keys`, `.Values` eller deconstruction
- Kombinér iteration med **if** (filtrering) og **sum** (akkumulering)
- Real-world: IP-omformeren kombinerer `while`, `for`, `break` og `continue`


:::knowledge-check
---
q: Hvornår er **`foreach`** særligt velegnet?
- Når du skal ændre samlingens størrelse under gennemløb
- Når du skal **gennemløbe alle elementer** uden at håndtere indeks
- Kun til dictionaries — ikke arrays
correct: 1
explain: **`foreach`** er den simpleste måde at læse hvert element i en array, liste eller dictionary — uden at skrive `for (int i = 0; i < ...`.
---
q: Hvad bruger du til antal elementer i en **List&lt;T&gt;** i en `for`-løkke?
- `.Length`
- **`.Count`**
- `.Size`
correct: 1
explain: Lister bruger **`.Count`**. Arrays bruger **`.Length`**. I `for (int i = 0; i < names.Count; i++)` skal det være `.Count` for lister.
---
q: Hvad er formålet med **nested loops**?
- At køre samme kode hurtigere
- At gennemløbe **strukturer indeni strukturer** — fx lister af lister eller bits per oktet
- At erstatte `foreach` helt
correct: 1
explain: **Nested loops** (løkke inde i løkke) bruges til 2D-data — fx matrix eller IP-omformeren: ydre løkke over 4 oktetter, indre over 8 bits.
---
q: Hvordan får du **kun nøglerne** fra en Dictionary?
- `foreach (int value in ages.Values)`
- **`foreach (string key in ages.Keys)`**
- `ages.GetKeys()`
correct: 1
explain: **`.Keys`** giver alle nøgler, **`.Values`** alle værdier, og **`foreach (var (navn, alder) in ages)`** giver begge (deconstruction).
---
q: Hvorfor bør du undgå `.Add()`/`.Remove()` på en liste **under `foreach`**?
- Det gør outputtet uleseligt
- Det giver **runtime-fejl** — samlingen må ikke modificeres under foreach
- Det er langsommere end `while`
correct: 1
explain: Modificering under **`foreach`** invalidérer enumerator'en. Brug **`for`** baglæns eller kopiér til `ToList()`, hvis du skal fjerne under iteration.
---
q: Hvad er mønsteret **akkumulering** i en løkke?
- At slette alle elementer én for én
- **Initialiser sum → loop → læg værdier til sum** → beregn fx gennemsnit
- At bruge `break` efter første element
correct: 1
explain: Fx `int sum = 0; foreach (var score in scores.Values) sum += score;` — derefter `gennemsnit = (double)sum / scores.Count`. Det genbruges konstant i GF2.
---
q: Hvornår vælger du **`for` med indeks** frem for `foreach`?
- Aldrig — `foreach` er altid bedst
- Når du har brug for **positionen** (indeks) eller skal ændre elementer på bestemte pladser
- Kun i Git-projekter
correct: 1
explain: **`for`** giver adgang til **`i`** og `collection[i]` — nødvendigt til nested strukturer, baglæns fjernelse og når indeks betyder noget (fx bit-position i binær oktet).
:::
