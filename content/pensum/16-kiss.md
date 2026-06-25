---
title: "KISS — Keep It Simple"
order: 16
category: kodeprincipper
topics: [kiss, enkelhed, læsbarhed, design]
kompetencemaal:
  - "Kan forklare KISS-princippet med egne ord"
  - "Kan genkende unødvendigt kompliceret kode og forenkle den"
  - "Kan vælge den simpleste løsning der stadig løser opgaven"
  - "Kan undgå for tidlig 'smart' kode i GF2-opgaver"
timer: 2
---

# KISS — Keep It Simple, Stupid

**KISS** betyder: lav den **simpleste løsning**, der virker. Ikke den smarteste, ikke den korteste, ikke den der imponerer — den der er **nem at læse, rette og forklare**.

:::callout type="info"
KISS handler ikke om at være dum — det handler om at **respektere din egen og andres tid**. Simpel kode har færre fejl og er hurtigere at forstå i eksamen og i gruppearbejde.
:::


## Hvad betyder det i praksis?

| Gør (KISS) | Undgå |
|------------|-------|
| Få trin ad gangen | Fem lag af logik i én linje |
| Klare metodenavne | "Clever" one-liners |
| Løs opgaven **nu** | Byg et "framework" til én opgave |
| Brug det I har lært | Avancerede tricks I ikke har pensum til endnu |

KISS passer godt sammen med [kode-stil og navngivning](/curriculum/15-kode-stil-og-navngivning) — begge handler om, at koden skal være let at forstå.


## Eksempel 1 — Betingelse: hold det læsbart

**For kompliceret:**

```csharp
bool ok = (age >= 18 && age <= 67) && (hasLicense == true) && (score >= 60 || isGuest == true);
if (ok)
{
    Console.WriteLine("Du må starte.");
}
```

**KISS:**

```csharp
// gf2-setup: int age = 20;
// gf2-setup: bool hasLicense = true;
// gf2-setup: int score = 75;
// gf2-setup: bool isGuest = false;

bool isAdult = age >= 18 && age <= 67;
bool hasValidLicense = hasLicense;
bool passedOrGuest = score >= 60 || isGuest;

if (isAdult && hasValidLicense && passedOrGuest)
{
    Console.WriteLine("Du må starte.");
}
```

Samme logik — men hvert trin har et **navn**, du kan forklare mundtligt.


## Eksempel 2 — Undgå unødvendig nesting

**For kompliceret:**

```csharp
if (input != null)
{
    if (input.Length > 0)
    {
        if (int.TryParse(input, out int value))
        {
            if (value > 0)
            {
                Console.WriteLine(value);
            }
        }
    }
}
```

**KISS (tidlig exit):**

```csharp
// gf2-setup: string input = "42";

if (string.IsNullOrWhiteSpace(input))
{
    Console.WriteLine("Tomt input.");
    return;
}

if (!int.TryParse(input, out int value))
{
    Console.WriteLine("Ikke et tal.");
    return;
}

if (value <= 0)
{
    Console.WriteLine("Skal være positivt.");
    return;
}

Console.WriteLine(value);
```

Færre niveauer af `{` — du læser oppefra og stopper, når noget er forkert.


## Eksempel 3 — Simpel menu i stedet for "generisk motor"

**For kompliceret** (overkill til GF2):

```csharp
var actions = new Dictionary<int, Action>
{
    { 1, () => StartGame() },
    { 2, () => ShowHelp() },
    { 3, () => Environment.Exit(0) }
};

Console.Write("Valg: ");
if (int.TryParse(Console.ReadLine(), out int key) && actions.ContainsKey(key))
{
    actions[key]();
}
```

**KISS:**

```csharp
// gf2-input: 1

Console.WriteLine("1. Start");
Console.WriteLine("2. Hjælp");
Console.WriteLine("3. Afslut");
Console.Write("Valg: ");

string choice = Console.ReadLine() ?? "";

if (choice == "1")
{
    StartGame();
}
else if (choice == "2")
{
    ShowHelp();
}
else if (choice == "3")
{
    Console.WriteLine("Farvel!");
}
else
{
    Console.WriteLine("Ukendt valg.");
}

static void StartGame()
{
    Console.WriteLine("Spillet starter!");
}

static void ShowHelp()
{
    Console.WriteLine("Vælg 1 for start, 3 for afslut.");
}
```

`if/else` er **helt fint** til tre valg. Du behøver ikke `Dictionary` og `Action` endnu.


## Eksempel 4 — Én opgave ad gangen i Main

**For kompliceret:**

```csharp
static void Main()
{
    int a = 5, b = 10, c = 15;
    int sum = a + b + c;
    double avg = sum / 3.0;
    string msg = avg >= 10 ? "Høj" : "Lav";
    Console.WriteLine($"Sum={sum} Avg={avg:F1} Status={msg}");
    for (int i = 0; i < 3; i++) { Console.WriteLine(i * 2); }
    PrintExtra();
    ReadNames();
    // ... 40 linjer mere
}
```

**KISS:**

```csharp
static void Main()
{
    PrintNumberReport();
    PrintDoubles();
}

static void PrintNumberReport()
{
    int a = 5;
    int b = 10;
    int c = 15;

    int sum = a + b + c;
    double average = sum / 3.0;

    Console.WriteLine($"Sum: {sum}");
    Console.WriteLine($"Average: {average:F1}");
}

static void PrintDoubles()
{
    for (int i = 0; i < 3; i++)
    {
        Console.WriteLine(i * 2);
    }
}

Main();
```

`Main` fortæller **historien** — detaljerne bor i små metoder.


## Eksempel 5 — Simpel løkke frem for smart trick

**For kompliceret:**

```csharp
Console.WriteLine(string.Join(", ", Enumerable.Range(1, 5).Select(n => n * n)));
```

**KISS:**

```csharp
for (int i = 1; i <= 5; i++)
{
    int square = i * i;
    Console.WriteLine(square);
}
```

I GF2 er en almindelig `for`-løkke ofte det rigtige valg — alle kan læse den.


## Eksempel 6 — Få datatyper, få koncepter

**Opgave:** Gem tre elevnavne og print dem.

**For kompliceret:**

```csharp
var students = new List<Dictionary<string, object>>
{
    new() { ["id"] = 1, ["name"] = "Ada", ["active"] = true },
    new() { ["id"] = 2, ["name"] = "Lin", ["active"] = false }
};

foreach (var s in students.Where(x => (bool)x["active"]))
{
    Console.WriteLine($"{s["id"]}: {s["name"]}");
}
```

**KISS:**

```csharp
string[] names = { "Ada", "Lin", "Max" };

for (int i = 0; i < names.Length; i++)
{
    Console.WriteLine($"{i + 1}. {names[i]}");
}
```

Brug **array** eller **`List<string>`** når det er nok — ikke nested dictionaries til en simpel opgave.


## Eksempel 7 — Klare beskeder til brugeren

**For kompliceret:**

```csharp
Console.WriteLine("ERR_CODE_42: operation failed");
```

**KISS:**

```csharp
Console.WriteLine("Kunne ikke gemme filen. Tjek at mappen findes.");
```

Brugeren (og din lærer) skal forstå beskeden uden en fejlkode-tabel.


## Eksempel 8 — Undgå for mange parametre

**For kompliceret:**

```csharp
static void PrintPerson(string first, string last, int age, string city, string country, string email, bool active)
{
    // ...
}
```

**KISS** — opdel eller brug det du faktisk skal bruge lige nu:

```csharp
PrintPerson("Ada Lovelace", 28, "London");

static void PrintPerson(string fullName, int age, string city)
{
    Console.WriteLine($"{fullName}, {age} år, {city}");
}
```

Til GF2-opgaver: **tre–fire parametre** er ofte rigeligt. Resten kan vente til I lærer klasser/objekter ordentligt.


## Eksempel 9 — Simpel validering

**For kompliceret:**

```csharp
static bool IsValidEmail(string email) =>
    !string.IsNullOrEmpty(email) && email.Contains('@') && email.IndexOf('@') == email.LastIndexOf('@')
    && email.Split('@')[1].Contains('.') && email.Length >= 5;
```

**KISS til øvelsesprogram:**

```csharp
string email = "ada@example.com";
Console.WriteLine(HasAtSign(email) ? "Email ser ok ud." : "Mangler @.");

static bool HasAtSign(string email)
{
    return email.Contains('@');
}
```

Fuldt e-mail-tjek er **svært** — i en øvelse er "indeholder @" ofte nok til at vise, at du tænker validering.


## Eksempel 10 — Når KISS *ikke* betyder doven kode

KISS er **ikke**:

- At kopiere den samme kode ti gange → se [DRY](/curriculum/17-dry)
- At ignorere fejl (`catch { }` uden besked)
- At springe navngivning over (`x`, `temp`, `data`)

KISS **er**:

- Den løsning en medstuderende kan læse uden at spørge "hvorfor gjorde du det sådan?"


## Hurtig tjekliste

Før du afleverer — spørg dig selv:

1. Kan jeg forklare hver del **højt** uden at blive forvirret?
2. Bruger jeg **kun** sprogkonstruktioner vi har haft pensum i?
3. Er der en **kortere vej** med *færre* begreber — ikke færre linjer?
4. Ville jeg forstå denne kode om **tre uger**?


:::knowledge-check
---
q: Hvad er hovedbudskabet i **KISS**?
- Kode skal altid være så kort som muligt
- Vælg den simpleste forståelige løsning der virker
- Du må aldrig bruge metoder eller loops
correct: 1
explain: KISS handler om **forståelighed** — ikke om at presse alt ind på én linje eller imponere med avancerede tricks.
---
q: Hvilken version følger KISS bedst?
- `if (a) { if (b) { if (c) { Do(); } } }`
- Tidlig `return` når input er ugyldigt, derefter normal logik
- En `Dictionary` med delegates til en menu med tre valg
correct: 1
explain: **Tidlig exit** og flade strukturer gør koden nemmere at følge — især når du debugger.
---
q: Hvornår er en almindelig `for`-løkke bedre end LINQ one-liners i GF2?
- Når du vil vise at du kender avanceret C#
- Når læsbarhed og pensumniveau er vigtigere end "smart" kode
- Når programmet skal køre hurtigere
correct: 1
explain: I GF2 er en **klassisk for-løkke** ofte det rigtige valg — den er eksplicit og let at forklare til eksamen.
---
q: Hvad er **ikke** KISS?
- At dele `Main` op i små metoder
- At kopiere samme blok kode mange steder i stedet for at genbruge
- At bruge beskrivende variabelnavne
correct: 1
explain: Gentagen kode er et **DRY**-problem. KISS og DRY arbejder sammen — simpel *struktur*, ikke copy-paste.
---
q: Hvilken fejlbesked til brugeren følger KISS?
- `ERR_CODE_42`
- `Kunne ikke gemme — tjek at mappen findes.`
- `Exception in thread main`
correct: 1
explain: Brugeren skal forstå beskeden **uden** at kende interne fejlkoder.
---
q: Du skal vise tre menuvalg i et konsolprogram. KISS-valget er ofte:
- `Dictionary<int, Action>` med lambdaer
- `if / else if` med tydelige strenge som `"1"`, `"2"`, `"3"`
- Et generelt menu-framework med ti klasser
correct: 1
explain: Til få valg er **if/else** helt rigeligt — simpelt at læse og rette.
:::
