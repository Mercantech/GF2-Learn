---
title: "Mini projects"
order: 11
difficulty: begynder
category: projekter
related_pensum: [07-datastrukturer, 06-metoder, 14-fejlfinding]
kompetencemaal:
  - "Kan kombinere lister, løkker og metoder i et større program"
  - "Kan bruge strings og validering i en sammenhængende løsning"
  - "Kan strukturere kode med flere metoder og tydelige ansvar"
  - "Kan løse et lille problem trin for trin i én opgave"
---

# Kapitel 10 — Mini projects

Fire **sammensatte opgaver** der kombinerer det, du har lært: variabler, input, løkker, lister, metoder, klasser og try/catch. Tag dig tid — skriv kode **efter `// TODO`**.

:::callout type="tip"
Del opgaven op i **små metoder** (fx `VisMenu()`, `Tilfoej()`, `UdskrivListe()`). Test ofte med **Kør** og justér `// gf2-input:`.
:::

---

## Mini-projekt 1 — Todo-liste

:::exercise level="begynder"

Lav en **todo-liste** i konsollen:

- Brugeren vælger: **tilføj**, **vis liste** eller **afslut**
- Gem opgaver i en **`List<string>`**
- Brug en **while-løkke** og mindst **to metoder**

:::

:::code-playground
```csharp
// gf2-input: 1
// gf2-input: Køb mælk
// gf2-input: 2
// gf2-input: 3
Console.WriteLine("Mini-projekt 1: Todo-liste");
Console.WriteLine("Menu: 1=Tilføj, 2=Vis, 3=Afslut");
// TODO: Lav mini-projekt 1 herunder!
```
:::

:::solution

```csharp
List<string> todos = new List<string>();

void VisMenu()
{
    Console.WriteLine("1) Tilføj  2) Vis  3) Afslut");
}

void UdskrivListe()
{
    if (todos.Count == 0)
    {
        Console.WriteLine("Listen er tom.");
        return;
    }
    for (int i = 0; i < todos.Count; i++)
        Console.WriteLine($"{i + 1}. {todos[i]}");
}

while (true)
{
    VisMenu();
    Console.Write("Valg: ");
    string valg = Console.ReadLine()!.Trim();
    if (valg == "3") break;
    if (valg == "1")
    {
        Console.Write("Opgave: ");
        todos.Add(Console.ReadLine()!.Trim());
    }
    else if (valg == "2")
    {
        UdskrivListe();
    }
    else
    {
        Console.WriteLine("Ukendt valg");
    }
}
Console.WriteLine("Farvel!");
```

:::

---

## Mini-projekt 2 — Quiz

:::exercise level="begynder"

Lav et **quiz-spil** med **3 spørgsmål** (fast i koden). Brug **parallelle arrays** eller lister til spørgsmål og rigtige svar. Tæl **point** og udskriv resultat til sidst.

:::

:::code-playground
```csharp
// gf2-input: 4
// gf2-input: Paris
// gf2-input: C#
Console.WriteLine("Mini-projekt 2: Quiz");
Console.WriteLine("Svar på 3 spørgsmål (se TODO).");
// TODO: Lav mini-projekt 2 herunder!
```
:::

:::solution

```csharp
string[] spoergsmaal =
{
    "Hvad er 2 + 2?",
    "Hovedstad i Frankrig?",
    "Hvilket sprog lærer du på GF2?"
};
string[] rigtige = { "4", "Paris", "C#" };

int point = 0;
for (int i = 0; i < spoergsmaal.Length; i++)
{
    Console.WriteLine(spoergsmaal[i]);
    string svar = Console.ReadLine()!.Trim();
    if (svar.Equals(rigtige[i], StringComparison.OrdinalIgnoreCase))
    {
        point++;
        Console.WriteLine("Rigtigt!");
    }
    else
    {
        Console.WriteLine($"Forkert — rigtigt svar: {rigtige[i]}");
    }
}
Console.WriteLine($"Du fik {point} af {spoergsmaal.Length}");
```

:::

---

## Mini-projekt 3 — Indkøb med total

:::exercise level="begynder"

Lav et **indkøbsprogram**:

- Brugeren indtaster **varenavn** og **pris** indtil de skriver **`stop`**
- Gem i en **`Dictionary<string, double>`**
- Udskriv **listen** og **total pris** til sidst
- Brug **try/catch** eller **TryParse** ved pris-input

:::

:::code-playground
```csharp
// gf2-input: Brød
// gf2-input: 25
// gf2-input: Mælk
// gf2-input: 12.50
// gf2-input: stop
Console.WriteLine("Mini-projekt 3: Indkøb med total");
// TODO: Lav mini-projekt 3 herunder!
```
:::

:::solution

```csharp
Dictionary<string, double> indkoeb = new Dictionary<string, double>();

while (true)
{
    Console.Write("Vare (eller stop): ");
    string vare = Console.ReadLine()!.Trim();
    if (vare.Equals("stop", StringComparison.OrdinalIgnoreCase))
        break;

    Console.Write("Pris: ");
    if (!double.TryParse(Console.ReadLine(), out double pris))
    {
        Console.WriteLine("Ugyldig pris — springer over.");
        continue;
    }
    indkoeb[vare] = pris;
}

double total = 0;
foreach (var linje in indkoeb)
{
    Console.WriteLine($"{linje.Key}: {linje.Value:F2} kr.");
    total += linje.Value;
}
Console.WriteLine($"Total: {total:F2} kr.");
```

:::

---

## Mini-projekt 4 — Klasseregister

:::exercise level="begynder"

Lav et **klasseregister**:

- Klasse **`Elev`** med **navn**, **alder** og **email**
- Metode **`Udskriv()`** på klassen
- Gem **3 elever** i en **`List<Elev>`** (input fra bruger)
- Valider **alder** (positiv) og **email** (indeholder `@`)

:::

:::code-playground
```csharp
// gf2-input: Emma
// gf2-input: 17
// gf2-input: emma@skole.dk
// gf2-input: Noah
// gf2-input: 18
// gf2-input: noah@skole.dk
// gf2-input: Olivia
// gf2-input: 16
// gf2-input: olivia@skole.dk
Console.WriteLine("Mini-projekt 4: Klasseregister");
// TODO: Lav mini-projekt 4 herunder!
```
:::

:::solution

```csharp
class Elev
{
    public string Navn { get; set; } = "";
    public int Alder { get; set; }
    public string Email { get; set; } = "";

    public void Udskriv()
    {
        Console.WriteLine($"{Navn}, {Alder} år — {Email}");
    }
}

bool ErGyldigEmail(string email) =>
    !string.IsNullOrWhiteSpace(email) && email.Contains('@');

List<Elev> klasse = new List<Elev>();
for (int i = 0; i < 3; i++)
{
    Console.Write($"Navn elev {i + 1}: ");
    string navn = Console.ReadLine()!.Trim();

    int alder;
    while (true)
    {
        Console.Write("Alder: ");
        if (int.TryParse(Console.ReadLine(), out alder) && alder > 0)
            break;
        Console.WriteLine("Alder skal være et positivt tal.");
    }

    string email;
    while (true)
    {
        Console.Write("E-mail: ");
        email = Console.ReadLine()!.Trim();
        if (ErGyldigEmail(email))
            break;
        Console.WriteLine("E-mail skal indeholde @.");
    }

    klasse.Add(new Elev { Navn = navn, Alder = alder, Email = email });
}

Console.WriteLine("Klasseliste:");
foreach (Elev e in klasse)
    e.Udskriv();
```

:::
