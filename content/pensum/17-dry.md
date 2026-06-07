---
title: "DRY — Don't Repeat Yourself"
order: 17
topics: [dry, genbrug, metoder, refaktorering]
kompetencemaal:
  - "Kan forklare DRY-princippet med egne ord"
  - "Kan genkende gentagen kode og samle den i metoder"
  - "Kan refaktorere simpel duplikeret logik uden at ændre adfærd"
  - "Kan skelne mellem DRY og for tidlig abstraktion (KISS)"
timer: 2
---

# DRY — Don't Repeat Yourself

**DRY** betyder: **gentag ikke den samme logik** mange steder. Hvis du har skrevet det samme (eller næsten det samme) to gange, bør det typisk være **én metode** — så retter du fejl ét sted.

:::callout type="info"
DRY handler om **viden og logik** — ikke om at to linjer tilfældigt ligner hinanden. `Console.WriteLine("Hej");` og `Console.WriteLine("Farvel");` er *ikke* et DRY-problem.
:::

:::callout type="warning"
DRY betyder **ikke** "lav alt generisk med det samme". Én kopi er bedre end en dårlig abstraktion. Se [KISS](/curriculum/16-kiss) — forenkel først, *derefter* samle gentagelser når mønsteret er tydeligt.
:::


## Hvad skal samles?

| DRY — gentag **logik** | Ikke nødvendigvis DRY |
|------------------------|------------------------|
| Samme beregning tre steder | To `Console.WriteLine` med forskellig tekst |
| Samme validering i to `if` | To variabler med samme type |
| Samme udskriftsformat i en løkke og udenfor | Lignende men *forskellig* forretningsregel |


## Eksempel 1 — Samme udskrift tre gange

**Gentaget (DRY-brud):**

```csharp
Console.WriteLine("=== Rapport ===");
Console.WriteLine($"Navn: {name}");
Console.WriteLine($"Score: {score}");

Console.WriteLine("=== Rapport ===");
Console.WriteLine($"Navn: {teacher}");
Console.WriteLine($"Score: {examScore}");

Console.WriteLine("=== Rapport ===");
Console.WriteLine($"Navn: {guest}");
Console.WriteLine($"Score: {guestScore}");
```

**DRY:**

```csharp
// gf2-setup: string name = "Ada";
// gf2-setup: int score = 72;
// gf2-setup: string teacher = "Kim";
// gf2-setup: int examScore = 88;
// gf2-setup: string guest = "Max";
// gf2-setup: int guestScore = 55;

PrintReport(name, score);
PrintReport(teacher, examScore);
PrintReport(guest, guestScore);

static void PrintReport(string personName, int personScore)
{
    Console.WriteLine("=== Rapport ===");
    Console.WriteLine($"Navn: {personName}");
    Console.WriteLine($"Score: {personScore}");
}
```


## Eksempel 2 — Samme grænseværdi mange steder

**Gentaget:**

```csharp
if (score >= 60)
{
    Console.WriteLine("Bestået");
}

// ... 30 linjer senere ...

if (score >= 60)
{
    bonusPoints = 5;
}

// ... igen ...

passed = score >= 60;
```

**DRY:**

```csharp
// gf2-setup: int score = 75;
// gf2-setup: int bonusPoints = 0;
// gf2-setup: bool passed = false;

const int PassingScore = 60;

if (IsPassing(score))
{
    Console.WriteLine("Bestået");
}

if (IsPassing(score))
{
    bonusPoints = 5;
}

passed = IsPassing(score);
Console.WriteLine($"Bonus: {bonusPoints}, bestået: {passed}");

static bool IsPassing(int value)
{
    return value >= 60;
}
```

Nu ændrer du **ét sted**, hvis grænsen bliver 50 eller 70.


## Eksempel 3 — Læs input med samme validering

**Gentaget:**

```csharp
Console.Write("Alder: ");
int age = int.Parse(Console.ReadLine() ?? "0");

Console.Write("Antal forsøg: ");
int attempts = int.Parse(Console.ReadLine() ?? "0");

Console.Write("Point: ");
int points = int.Parse(Console.ReadLine() ?? "0");
```

**DRY:**

```csharp
// gf2-input: 25, 3, 80

int age = ReadPositiveInt("Alder: ");
int attempts = ReadPositiveInt("Antal forsøg: ");
int points = ReadPositiveInt("Point: ");

Console.WriteLine($"Alder {age}, forsøg {attempts}, point {points}.");

static int ReadPositiveInt(string prompt)
{
    Console.Write(prompt);
    string input = Console.ReadLine() ?? "0";
    return int.Parse(input);
}
```

Senere kan du udvide **én** metode med `TryParse` og fejlbeskeder — ikke tre kopier.


## Eksempel 4 — Samme loop-mønster

**Gentaget:**

```csharp
for (int i = 0; i < names.Length; i++)
{
    Console.WriteLine($"{i + 1}. {names[i]}");
}

// ... senere med et andet array ...

for (int i = 0; i < cities.Length; i++)
{
    Console.WriteLine($"{i + 1}. {cities[i]}");
}
```

**DRY:**

```csharp
// gf2-setup: string[] names = { "Ada", "Lin", "Max" };
// gf2-setup: string[] cities = { "Aarhus", "København", "Odense" };

PrintNumberedList(names);
PrintNumberedList(cities);

static void PrintNumberedList(string[] items)
{
    for (int i = 0; i < items.Length; i++)
    {
        Console.WriteLine($"{i + 1}. {items[i]}");
    }
}
```


## Eksempel 5 — Beregning kopieret i `if` og `else`

**Gentaget:**

```csharp
if (hours > 40)
{
    double pay = 40 * rate + (hours - 40) * rate * 1.5;
    Console.WriteLine(pay);
}
else
{
    double pay = hours * rate;
    Console.WriteLine(pay);
}
```

**DRY:**

```csharp
// gf2-setup: double hours = 45;
// gf2-setup: double rate = 200;

double pay = CalculatePay(hours, rate);
Console.WriteLine(pay);

static double CalculatePay(double hours, double rate)
{
    if (hours > 40)
    {
        return 40 * rate + (hours - 40) * rate * 1.5;
    }

    return hours * rate;
}
```

Reglen for løn findes **ét sted** — nemmere at teste og forklare.


## Eksempel 6 — Validering kopieret til flere felter

**Gentaget:**

```csharp
if (string.IsNullOrWhiteSpace(firstName))
{
    Console.WriteLine("Fornavn mangler.");
    return;
}

if (string.IsNullOrWhiteSpace(lastName))
{
    Console.WriteLine("Efternavn mangler.");
    return;
}

if (string.IsNullOrWhiteSpace(email))
{
    Console.WriteLine("Email mangler.");
    return;
}
```

**DRY:**

```csharp
// gf2-setup: string firstName = "Ada";
// gf2-setup: string lastName = "Lovelace";
// gf2-setup: string email = "ada@example.com";

if (!RequireText(firstName, "Fornavn")) return;
if (!RequireText(lastName, "Efternavn")) return;
if (!RequireText(email, "Email")) return;

Console.WriteLine("Alle felter er udfyldt.");

static bool RequireText(string value, string fieldLabel)
{
    if (string.IsNullOrWhiteSpace(value))
    {
        Console.WriteLine($"{fieldLabel} mangler.");
        return false;
    }

    return true;
}
```


## Eksempel 7 — Formatér output ens

**Gentaget:**

```csharp
Console.WriteLine($"Pris: {price:F2} kr.");
Console.WriteLine($"Rabat: {discount:F2} kr.");
Console.WriteLine($"Total: {total:F2} kr.");
```

**DRY:**

```csharp
// gf2-setup: double price = 99.50;
// gf2-setup: double discount = 10.00;
// gf2-setup: double total = 89.50;

PrintMoney("Pris", price);
PrintMoney("Rabat", discount);
PrintMoney("Total", total);

static void PrintMoney(string label, double amount)
{
    Console.WriteLine($"{label}: {amount:F2} kr.");
}
```


## Eksempel 8 — Switch med duplikeret kode i cases

**Gentaget:**

```csharp
switch (choice)
{
    case "1":
        Console.WriteLine("Starter spil...");
        Console.WriteLine("Indlæser data...");
        StartGame();
        break;
    case "2":
        Console.WriteLine("Starter øvelse...");
        Console.WriteLine("Indlæser data...");
        StartPractice();
        break;
}
```

**DRY:**

```csharp
// gf2-setup: string choice = "1";

switch (choice)
{
    case "1":
        BeginSession("spil");
        StartGame();
        break;
    case "2":
        BeginSession("øvelse");
        StartPractice();
        break;
}

static void BeginSession(string modeName)
{
    Console.WriteLine($"Starter {modeName}...");
    Console.WriteLine("Indlæser data...");
}

static void StartGame()
{
    Console.WriteLine("Spillet er klar.");
}

static void StartPractice()
{
    Console.WriteLine("Øvelsen er klar.");
}
```


## Eksempel 9 — Array + manuel gentagelse

**Gentaget:**

```csharp
int sum = numbers[0] + numbers[1] + numbers[2] + numbers[3] + numbers[4];
double avg = sum / 5.0;
Console.WriteLine($"Sum: {sum}, Avg: {avg:F1}");
```

Hvis arrayet vokser, kopierer folk ofte mønsteret igen.

**DRY:**

```csharp
// gf2-setup: int[] numbers = { 4, 8, 15, 16, 23 };

int sum = SumArray(numbers);
double average = (double)sum / numbers.Length;

Console.WriteLine($"Sum: {sum}, Avg: {average:F1}");

static int SumArray(int[] values)
{
    int total = 0;
    for (int i = 0; i < values.Length; i++)
    {
        total += values[i];
    }

    return total;
}
```


## Eksempel 10 — Konstanter frem for "magic numbers"

**Gentaget:**

```csharp
if (attempts > 3) { /* ... */ }
// ...
if (attempts > 3) { /* ... */ }
// ...
maxAttempts = 3;
```

**DRY:**

```csharp
// gf2-setup: int attempts = 4;

const int MaxAttempts = 3;

if (attempts > MaxAttempts)
{
    Console.WriteLine("For mange forsøg.");
}

if (attempts > MaxAttempts)
{
    Console.WriteLine("Blokeret midlertidigt.");
}

int maxAttempts = MaxAttempts;
Console.WriteLine($"Max forsøg: {maxAttempts}");
```

Konstanten er **ét navngivet sted** for tallet 3.


## DRY vs. KISS — hvornår venter du?

| Situation | Gør |
|-----------|-----|
| Koden er gentaget **2+ gange** med samme regel | Overvej metode |
| Du har kun set mønsteret **én gang** | Vent — lad være med at abstrahere for tidligt |
| Abstraktionen kræver 8 parametre | Forenkl først ([KISS](/curriculum/16-kiss)) |
| Metodenavnet er uklart (`DoThing`) | DRY hjælper ikke — navngiv bedre |

:::callout type="info"
**Rule of Three** (tommelfingerregel): Først når noget gentager sig **tre gange**, er det ofte værd at samle det. To gange kan du leve med — især i små øvelsesprogrammer.
:::


## Refaktorering trin for trin

1. **Find** to blokke der gør det samme (eller næsten)
2. **Uddrag** til en metode med et **beskrivende navn**
3. **Erstat** kopierne med metodekald
4. **Kør** programmet — adfærden skal være uændret
5. **Formater** med Ctrl+Shift+F i GF2 Learn


## Hurtig tjekliste

- [ ] Er samme **regel** skrevet mere end ét sted?
- [ ] Har jeg **magic numbers** der burde være `const`?
- [ ] Kan jeg samle gentagen logik i en **metode** med et klart navn?
- [ ] Er min abstraktion **enklere** end kopierne — ikke sværere?


:::knowledge-check
---
q: Hvad betyder **DRY**?
- Du må kun have én fil i projektet
- **Gentag ikke den samme logik** — samle den ét sted
- Du må aldrig kopiere kode fra internettet
correct: 1
explain: DRY handler om **vedligeholdelse**: én rettelse ét sted i stedet for at jage fejl i mange kopier.
---
q: Hvilket er et tydeligt DRY-brud?
- To `WriteLine` med forskellig tekst
- **Samme `if (score >= 60)` kopieret tre steder i programmet**
- To variabler der begge hedder `count`
correct: 1
explain: Samme **grænse/regel** på flere steder bør typisk være en **metode** eller **konstant**.
---
q: Hvad er den bedste DRY-løsning til gentagen "=== Rapport ===" udskrift?
- Kopier blokken igen med nye variabler
- **Metoden `PrintReport(name, score)` kaldt fra flere steder**
- En generisk `Print(object a, object b, object c, object d)` med 12 parametre
correct: 1
explain: En **navngiven metode** med få parametre samler formatet uden at blive uforståelig.
---
q: Hvad er **Rule of Three**?
- Du må højst have tre variabler
- **Overvej at abstrahere når samme mønster gentager sig ca. tre gange**
- Du skal altid lave præcis tre metoder
correct: 1
explain: Tommelfingerregel mod **for tidlig** abstraktion — to gentagelser kan være OK i små opgaver.
---
q: Hvordan arbejder DRY og KISS sammen?
- De modarbejder altid hinanden
- **KISS: hold det enkelt — DRY: saml tydelige gentagelser uden at over-komplicere**
- KISS betyder at du aldrig må bruge metoder
correct: 1
explain: Først **simpel** kode — derefter **genbrug** når mønsteret er klart. Dårlig DRY (for generisk) bryder KISS.
---
q: Hvorfor er `const int PassingScore = 60;` et DRY-trick?
- Fordi const gør programmet hurtigere
- **Fordi tallet 60 kun defineres ét sted med et navn**
- Fordi compileren kræver det
correct: 1
explain: **Magic numbers** spredt i koden er et DRY-problem — konstanten er ét sted at ændre grænsen.
:::
