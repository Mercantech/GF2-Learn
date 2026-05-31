---
title: "Operators & Expressions"
order: 3
topics: [operatorer, udtryk, bool]
kompetencemaal:
  - "Kan forklare, hvad et expression er i C#"
  - "Kan anvende aritmetiske, sammenlignings- og logiske operatorer"
  - "Kan kombinere flere operatorer i simple beregninger"
  - "Kan evaluere, hvorfor et boolsk udtryk er true eller false"
timer: 2
---

# Operatorer og udtryk

Et **udtryk** (expression) er en kombination af værdier, variabler, operatorer og funktioner, der evalueres til **én enkelt værdi**. **Operatorer** er symboler, der udfører operationer på værdier og variabler.

C# skelner mellem **statements** (handlinger) og **expressions** (værdier):

- `Console.WriteLine("Hej");` er et statement — det *gør* noget
- `2 + 3` er et expression — det *er* noget (værdien 5)

## Typer af udtryk

### Bogstavelige udtryk

Direkte værdier i koden — tal, tekst eller sandt/falsk:

```csharp
5         // Et tal
"Hej"     // En tekststreng
true      // En boolsk værdi
```

### Variable udtryk

Variabler, der gemmer værdier og kan bruges i beregninger:

```csharp
int x = 10;
string name = "Alice";
bool aktiv = true;
```

### Aritmetiske udtryk

Kombinerer tal med matematiske operatorer:

```csharp
int sum = 5 + 3;        // Resultatet er 8
int produkt = 4 * 2;    // Resultatet er 8
bool erVoksen = alder >= 18;   // true eller false
```

Du kan gemme resultatet af et udtryk i en variabel:

```csharp
int resultat = 10 + 5;
bool harBestaaet = score >= 60;
string besked = $"Score: {score}";
```


## Aritmetiske operatorer

Aritmetiske operatorer udfører grundlæggende matematik på tal.

**`+` (plus) — addition**

```csharp
int sum = 5 + 3;   // 8
```

**`-` (minus) — subtraktion**

```csharp
int difference = 5 - 3;   // 2
```

**`*` (gangetegn) — multiplikation**

```csharp
int produkt = 5 * 3;   // 15
```

**`/` (divisionstegn) — division**

```csharp
int kvotient = 10 / 2;      // 5
double dec = 10 / 4.0;      // 2.5
```

**Vigtigt om division:** `10 / 4` giver `2` (heltalsdivision), fordi begge operandi er `int`. Brug `10 / 4.0` eller `(double)10 / 4` for at få `2.5`.

**`%` (modulus) — rest ved heltalsdivision**

```csharp
int rest = 10 % 3;   // 1
```

`10 % 3` er 1, fordi 10 = 3×3 + 1. Nyttigt til at tjekke om et tal er lige: `n % 2 == 0`.


## Sammenligningsoperatorer

Sammenligningsoperatorer returnerer altid `bool` (true/false).

**`==` (lig med)**

```csharp
bool erLige = (5 == 5);   // true
```

**`!=` (ikke lig med)**

```csharp
bool erForskellige = (5 != 3);   // true
```

**`>` (større end)**

```csharp
bool erStoerre = (5 > 3);   // true
```

**`<` (mindre end)**

```csharp
bool erMindre = (5 < 3);   // false
```

**`>=` (større end eller lig med)**

```csharp
bool erStoerreEllerLig = (5 >= 5);   // true
```

**`<=` (mindre end eller lig med)**

```csharp
bool erMindreEllerLig = (5 <= 5);   // true
```

Typisk brug:

```csharp
bool erVoksen = alder >= 18;
bool harBestaaet = score >= 60;
bool korrektPassword = input == "hemmelig";
```

:::callout type="warning"
Brug `==` til sammenligning og `=` til tildeling. `if (x = 5)` er en fejl — det skal være `if (x == 5)`.
:::


## Logiske operatorer

Logiske operatorer kombinerer flere betingelser.

**`&&` (logisk OG) — sand, hvis begge betingelser er sande**

```csharp
bool resultat = (5 > 3) && (2 < 4);   // true
bool begge = aktiv && score >= 60;
```

**`||` (logisk ELLER) — sand, hvis mindst én betingelse er sand**

```csharp
bool resultat = (5 > 3) || (2 > 4);   // true
bool enAf = brugernavn == "admin" || erAdmin;
```

**`!` (logisk IKKE) — vender en betingelse**

```csharp
bool resultat = !(5 > 3);   // false
bool ikkeTom = !string.IsNullOrEmpty(navn);
```

**Short-circuit:** Med `&&` evalueres højre side kun, hvis venstre er `true`. Med `||` evalueres højre side kun, hvis venstre er `false`. Det beskytter mod fejl — fx `tal != 0 && 10/tal > 2` crasher ikke ved `tal = 0`.

:::callout type="info"
Evaluer et boolsk udtryk trin for trin: Hvad er hver del? Bruger `&&` og `||` **short-circuit** — højre side evalueres kun, når det er nødvendigt.
:::


## Kombinerede udtryk og operator-prioritet

Ved at kombinere udtryk og operatorer kan du bygge komplekse logikker og udføre beregninger i dine C#-programmer:

```csharp
int score1 = 80, score2 = 90, score3 = 70;
int timer = 120;
int alder = 20;
bool harKoerekort = true;
bool erVIP = false;

double gennemsnit = (score1 + score2 + score3) / 3.0;
bool bonus = gennemsnit >= 80 && timer >= 100;
bool resultat = (alder >= 18 && harKoerekort) || erVIP;
Console.WriteLine($"Bonus: {bonus}");
```

**Prioritet** (hvem der regnes først):

1. Parenteser `()`
2. Multiplikation, division, modulo `* / %`
3. Addition, subtraktion `+ -`
4. Sammenligning `< > <= >= == !=`
5. Logisk NOT `!`
6. Logisk AND `&&`
7. Logisk OR `||`

Brug parenteser, når du er i tvivl — det gør koden læsbar og undgår fejl.


:::knowledge-check
---
q: Hvad er forskellen på et **statement** og et **expression** i C#?
- Et statement evalueres til en værdi; et expression udfører en handling
- Et **expression** evalueres til én værdi; et **statement** udfører en handling
- Der er ingen forskel — begge termer betyder det samme
correct: 1
explain: `2 + 3` er et **expression** — det *er* værdien 5. `Console.WriteLine("Hej");` er et **statement** — det *gør* noget uden at producere en værdi, du kan gemme.
---
q: Hvad er resultatet af `10 / 4` når begge operandi er `int`?
- `2.5`
- **`2`** (heltalsdivision)
- `3`
correct: 1
explain: Når begge operandi er **heltal**, laver `/` heltalsdivision og dropper decimaldelen. For at få `2.5` skal mindst én operand være decimaltal — fx `10 / 4.0`.
---
q: Hvad returnerer `10 % 3`?
- `3`
- **`1`** (resten ved division)
- `0`
correct: 1
explain: **`%` (modulus)** giver resten ved heltalsdivision. 10 = 3×3 + **1**, så resultatet er 1. Det er nyttigt til at tjekke om et tal er lige: `n % 2 == 0`.
---
q: Hvilken operator bruger du til at sammenligne to værdier for lighed?
- `=`
- **`==`**
- `:=`
correct: 1
explain: **`==`** sammenligner to værdier og returnerer `true` eller `false`. **`=`** er **tildeling** — den gemmer en værdi i en variabel. `if (x = 5)` er en fejl; det skal være `if (x == 5)`.
---
q: Hvornår evalueres højre side **ikke** i udtrykket `A && B`?
- Aldrig — begge sider evalueres altid
- Når **A er false** (short-circuit)
- Når B er true
correct: 1
explain: Med **`&&`** evalueres B kun, hvis A er **true** — det kaldes **short-circuit**. Det beskytter mod fejl, fx `tal != 0 && 10/tal > 2` crasher ikke, når `tal` er 0.
---
q: Hvad er resultatet af `!(5 > 3)`?
- `true`
- **`false`**
- `5`
correct: 1
explain: **`!`** (logisk IKKE) vender en boolsk værdi. `5 > 3` er `true`, og `!true` bliver **`false`**.
---
q: Hvilken operator har **lavest prioritet** i udtrykket `a && b || c`?
- `&&`
- **`||`**
- `!`
correct: 1
explain: Operator-prioritet (fra højest til lavest): parenteser → `* / %` → `+ -` → sammenligning → `!` → **`&&`** → **`||`**. Derfor evalueres `||` sidst — brug parenteser, når du er i tvivl.
:::
