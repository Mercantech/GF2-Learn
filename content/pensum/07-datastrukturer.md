---
title: "Arrays, Lists & Dictionaries"
order: 7
topics: [arrays, lister, dictionaries, datatyper]
kompetencemaal:
  - "Kan forklare forskellen på arrays, lister og dictionaries"
  - "Kan oprette, tilgå og ændre elementer i array og liste"
  - "Kan forklare, hvornår en liste er bedre end et array"
  - "Kan bruge en dictionary til key-value opslag"
  - "Kan vælge den rigtige datastruktur ud fra problemstillingen"
timer: 3
---

# Arrays, lister og dictionaries

Rækketyper som **arrays**, **lister** og **dictionaries** samler flere værdier under ét navn — så du kan pege på en hel samling med én variabel i stedet for ti enkeltvariabler.

## Fra simple typer til samlinger

En `int` er 32 bit (4 bytes) og gemmer **én** værdi. `double`, `float` og `decimal` fylder mere grundet deres præcision. `char` er 16 bit i C# (Unicode), selvom mange tegn kun bruger 8 bit.

Disse kaldes **simple datatyper** eller **primitive typer** — de kan vises som enkelt værdier:

```
int alder = 17;      // én værdi
char bogstav = 'A';  // ét tegn
```

Hvad gør vi, når vi vil gemme **mange** værdier — fx en række tal eller tegn? Her kommer **arrays** og **lister** ind. I har faktisk allerede brugt dem: en **string** er en sekvens af tegn — i C# et objekt, men idéen er den samme som en række `char`-værdier bundet sammen.


## Arrays

Et **array** gemmer en samling af elementer af **samme type** med **fast størrelse**.

Arrays er **indekserede** — hvert element har en plads (indeks), og du tilgår dem med `[indeks]`:

```
int[] numbers = { 1, 2, 3, 4, 5 }

  [0]    [1]    [2]    [3]    [4]
   1  →   2  →   3  →   4  →   5
```

**Opret et array** med fast størrelse:

```csharp
int[] numbers = new int[10];   // 10 heltal, initialiseret til 0
```

**Initialiser med værdier:**

```csharp
int[] tal = { 10, 20, 30 };
```

**Tilgå og ændre elementer** — indeks starter altid ved **0**:

```csharp
int[] tal = { 10, 20, 30 };
int firstNumber = tal[0];   // første element
tal[0] = 42;                // ændr værdi
Console.WriteLine(tal.Length);  // antal elementer (3)
```

:::callout type="warning"
Gå aldrig uden for arrayets grænser — `numbers[10]` på et array med længde 10 giver fejl. Gyldige indeks er `0` til `Length - 1`.
:::

Arrays er effektive når størrelsen er **kendt og uforanderlig** — fx ugedage (7), oktetter i en IP (4), eller en fast liste scores.


## Lister

En **List&lt;T&gt;** kan **vokse og krympe** dynamisk — den mest brugte samling i daglig C#.

```csharp
using System.Collections.Generic;

List<int> numbers = new List<int>();
```

**Tilføj elementer:**

```csharp
numbers.Add(10);
numbers.Add(20);
numbers.Add(30);
```

**Tilgå og ændre via indeks** — samme syntaks som arrays:

```csharp
List<int> numbers = new List<int> { 10, 20, 30 };

int firstNumber = numbers[0];
numbers[0] = 42;
Console.WriteLine(numbers.Count);   // antal elementer
```

**Flere metoder:**

```csharp
var navne = new List<string> { "Ada", "Alan" };

navne.Add("Grace");          // tilføj til slutningen
navne.Insert(0, "Grace H."); // indsæt på indeks
navne.Remove("Alan");        // fjern efter værdi
navne.Clear();               // fjern alle
```

Lister er **reference-typer** — variablen peger på listen i hukommelsen, ikke en kopi. Vær opmærksom, når du deler lister mellem metoder.


## Arrays vs. lister — hvornår bruger man hvad?

| Emne | Array | List&lt;T&gt; |
|------|-------|-------------|
| Hukommelse | Mindre — ingen ekstra metoder | Lidt mere — baggrund og metoder |
| Størrelse | **Fast** ved oprettelse | **Dynamisk** — Add/Remove |
| Adgang | Hurtig, kontinuerlig blok | Fleksibel tilføjelse/fjernelse |
| Tilføj element | ❌ fast størrelse | ✅ `.Add()` |
| Fjern element | ❌ | ✅ `.Remove()` |
| Tilgå element | `numbers[0]` | `names[0]` |
| Antal elementer | `.Length` | `.Count` |
| Loop | `for` / `foreach` | `for` / `foreach` |

**Generelt:** Kender du størrelsen på forhånd og skal den ikke ændres → **array**. Skal samlingen vokse eller krympe → **liste**.

:::callout type="info"
**Huskeregel:**

- **Array** → du ved præcis hvor mange. Fx dage i ugen (7).
- **Liste** → du ved ikke hvor mange. Fx en indkøbsliste.
- **Dictionary** → du vil slå noget op på et navn. Fx en telefonbog.
:::

I praksis vælger mange udviklere **liste** som standard — den er mere fleksibel.


:::video-list
- [List&lt;T&gt; and Collections of Data [Pt 12] | C# for Beginners](https://www.youtube.com/watch?v=M3UsM9l1m6c)
- [Arrays, Lists, Indexing, and Foreach [Pt 13] | C# for Beginners](https://www.youtube.com/watch?v=7PDNqmBdtrE)
- [Sorting and Searching Lists [Pt 14] | C# for Beginners](https://www.youtube.com/watch?v=2sp4gWCq3o4)
:::


## Dictionary — nøgle og værdi

En **Dictionary&lt;TKey, TValue&gt;** gemmer **nøgle/værdi-par**. Den er bygget som et **hash map**, så opslag på nøgle er meget effektivt — typisk **O(1)** (konstant tid).

[Dictionary (Microsoft Docs)](https://learn.microsoft.com/dotnet/api/system.collections.generic.dictionary-2)

```csharp
Dictionary<string, int> ageMap = new Dictionary<string, int>();

ageMap.Add("Anders", 25);
ageMap.Add("Line", 30);

int lineAge = ageMap["Line"];   // 30 — opslag via nøgle
```

**Tilføj eller opdater** med indexer-syntaks:

```csharp
ageMap["Grace"] = 22;   // tilføj ny
ageMap["Anders"] = 26;  // opdater eksisterende
```

**Sikkert opslag** med `TryGetValue` — undgår fejl hvis nøglen ikke findes:

```csharp
Dictionary<string, int> ageMap = new Dictionary<string, int>
{
    { "Anders", 25 },
    { "Line", 30 }
};

if (ageMap.TryGetValue("Ada", out int alder))
    Console.WriteLine($"Ada er {alder} år");
else
    Console.WriteLine("Ada findes ikke i mappet");
```

Hash-funktionen fordeler nøgler i **buckets** — derfor er opslag hurtigt uden at gennemsøge hele listen.


## Sammenligning — array, liste og dictionary

| Metode | Array | List&lt;T&gt; | Dictionary&lt;K,V&gt; |
|--------|-------|-------------|---------------------|
| Tilføj element | ❌ fast størrelse | ✅ `.Add()` | ✅ `.Add(key, val)` |
| Fjern element | ❌ | ✅ `.Remove()` | ✅ `.Remove(key)` |
| Tilgå element | `numbers[0]` | `names[0]` | `map["Anders"]` |
| Loop | `for` / `foreach` | `for` / `foreach` | `foreach` KeyValuePair |
| Antal | `.Length` | `.Count` | `.Count` |

**Vælg datastruktur:**

1. Skal du gemme flere værdier af samme type?
2. **Kender du størrelsen?** → Ja → **array**. Nej → videre.
3. **Har du brug for nøgler?** → Nej → **liste**. Ja → **dictionary**.


## Iteration over samlinger

Alle tre kan gennemløbes med loops — se også kapitlet [Loops](/curriculum/05-loekker):

```csharp
int[] tal = { 10, 20, 30 };
foreach (int t in tal)
    Console.WriteLine(t);

Dictionary<string, int> ageMap = new Dictionary<string, int>
{
    { "Anders", 25 },
    { "Line", 30 }
};

foreach (var kvp in ageMap)
    Console.WriteLine($"{kvp.Key}: {kvp.Value}");
```


## Praktisk eksempel — elever og karakterer

```csharp
var elever = new List<string> { "Ada", "Alan", "Grace" };
var karakterer = new Dictionary<string, int>
{
    ["Ada"] = 12,
    ["Alan"] = 10,
    ["Grace"] = 7
};

foreach (var elev in elever)
{
    if (karakterer.TryGetValue(elev, out int karakter))
        Console.WriteLine($"{elev}: karakter {karakter}");
    else
        Console.WriteLine($"{elev}: ingen karakter");
}
```

Mønsteret **liste over nøgler + dictionary med data** genbruges i mange GF2-opgaver og projekter.


:::git-step
commit: "feat: arrays lister dictionary"
branch: main
:::

## Opsummering

- **Arrays** — fast størrelse, effektiv indeksering, god til kendte mængder
- **Lister** — dynamisk størrelse, Add/Remove, standardvalg i moderne C#
- **Dictionaries** — nøgle/værdi-opslag, hurtig hash-baseret adgang
- Vælg struktur efter problem: fast størrelse, dynamisk samling eller opslag på navn


:::knowledge-check
---
q: Hvad er hovedforskellen på et **array** og en **List&lt;T&gt;**?
- Arrays kan kun indeholde strings
- Et array har fast størrelse; en liste kan vokse og krympe dynamisk
- Lister er primitiv type; arrays er reference-type
correct: 1
explain: **Arrays** allokeres med fast længde ved oprettelse. **Lister** understøtter `.Add()` og `.Remove()` — vælg liste, når du ikke kender antallet på forhånd.
---
q: Hvilket indeks har **første element** i et array?
- `1`
- `0`
- `-1`
correct: 1
explain: C# (som de fleste sprog) bruger **0-baseret indeksering**. Første element er `array[0]`, sidste er `array[Length - 1]`.
---
q: Hvornår er en **Dictionary** det rigtige valg?
- Når du skal gemme mange tal i rækkefølge uden navne
- Når du skal slå data op via en nøgle (fx navn → alder)
- Når størrelsen er kendt og aldrig ændres
correct: 1
explain: En **Dictionary** gemmer **nøgle/værdi-par** — som en telefonbog. Opslag via nøgle er typisk **O(1)** takket være hash-fordeling.
---
q: Hvad returnerer `ageMap.TryGetValue("Ada", out int alder)` hvis nøglen **ikke findes**?
- Den kaster en exception
- `false` — og `alder` får standardværdien 0
- `true` med alder sat til -1
correct: 1
explain: **`TryGetValue`** er sikkert opslag — den returnerer **`false`** uden at crashe, hvis nøglen mangler. Direkte `map["Ada"]` kaster fejl, når nøglen ikke findes.
---
q: Hvad er gyldige indeks for et array med **Length = 10**?
- `0` til `10` (inklusiv)
- `0` til `9`
- `1` til `10`
correct: 1
explain: Med længde 10 er gyldige indeks **`0` til `9`**. `array[10]` giver **IndexOutOfRangeException** — det er en almindelig begynderfejl.
---
q: Hvilken property bruger du til antal elementer i en **List&lt;string&gt;**?
- `.Length`
- `.Count`
- `.Size`
correct: 1
explain: Lister bruger **`.Count`**. Arrays bruger **`.Length`**. Bland dem ikke — compileren giver fejl, hvis du bruger den forkerte.
---
q: Ifølge huskereglen i kapitlet — hvornår vælger du et **array**?
- Når du ikke ved, hvor mange elementer der kommer
- Når du kender størrelsen på forhånd og den ikke skal ændres — fx 7 ugedage
- Når du skal slå op på navn
correct: 1
explain: **Array** → kendt, fast antal. **Liste** → ukendt eller varierende antal. **Dictionary** → opslag på nøgle/navn.
:::
