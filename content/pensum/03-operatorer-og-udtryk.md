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
bool isActive = true;
```

### Aritmetiske udtryk

Kombinerer tal med matematiske operatorer:

```csharp
int sum = 5 + 3;        // Resultatet er 8
int product = 4 * 2;    // Resultatet er 8
bool isAdult = age >= 18;   // true eller false
```

Du kan gemme resultatet af et udtryk i en variabel:

```csharp
int result = 10 + 5;
bool hasPassed = score >= 60;
string message = $"Score: {score}";
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
int product = 5 * 3;   // 15
```

**`/` (divisionstegn) — division**

```csharp
int quotient = 10 / 2;              // 5
double decimalResult = 10 / 4.0;    // 2.5
```

**Vigtigt om division:** `10 / 4` giver `2` (heltalsdivision), fordi begge operandi er `int`. Brug `10 / 4.0` eller `(double)10 / 4` for at få `2.5`.

**`%` (modulus) — rest ved heltalsdivision**

```csharp
int remainder = 10 % 3;   // 1
```

`10 % 3` er 1, fordi 10 = 3×3 + 1. Nyttigt til at tjekke om et tal er lige: `n % 2 == 0`.


:::video-list
- [Numbers, Integers, and Math [Pt 7] | C# for Beginners](https://www.youtube.com/watch?v=ZXCMBOxry8A)
- [Numbers, Precision, Casting, Doubles, and More [Pt 8] | C# for Beginners](https://www.youtube.com/watch?v=Kg_k0vL0dD4)
:::


## Sammenligningsoperatorer

Sammenligningsoperatorer returnerer altid `bool` (true/false).

**`==` (lig med)**

```csharp
bool isEqual = (5 == 5);   // true
```

**`!=` (ikke lig med)**

```csharp
bool areDifferent = (5 != 3);   // true
```

**`>` (større end)**

```csharp
bool isGreater = (5 > 3);   // true
```

**`<` (mindre end)**

```csharp
bool isLess = (5 < 3);   // false
```

**`>=` (større end eller lig med)**

```csharp
bool isGreaterOrEqual = (5 >= 5);   // true
```

**`<=` (mindre end eller lig med)**

```csharp
bool isLessOrEqual = (5 <= 5);   // true
```

Typisk brug:

```csharp
bool isAdult = age >= 18;
bool hasPassed = score >= 60;
bool isPasswordCorrect = input == "hemmelig";
```

:::callout type="warning"
Brug `==` til sammenligning og `=` til tildeling. `if (x = 5)` er en fejl — det skal være `if (x == 5)`.
:::


## Logiske operatorer

Logiske operatorer kombinerer flere betingelser.

**`&&` (logisk OG) — sand, hvis begge betingelser er sande**

```csharp
bool result = (5 > 3) && (2 < 4);   // true
bool both = isActive && score >= 60;
```

**`||` (logisk ELLER) — sand, hvis mindst én betingelse er sand**

```csharp
bool result = (5 > 3) || (2 > 4);   // true
bool either = username == "admin" || isAdmin;
```

**`!` (logisk IKKE) — vender en betingelse**

```csharp
bool result = !(5 > 3);   // false
bool isNotEmpty = !string.IsNullOrEmpty(name);
```

**Short-circuit:** Med `&&` evalueres højre side kun, hvis venstre er `true`. Med `||` evalueres højre side kun, hvis venstre er `false`. Det beskytter mod fejl — fx `number != 0 && 10/number > 2` crasher ikke ved `number = 0`.

:::callout type="info"
Evaluer et boolsk udtryk trin for trin: Hvad er hver del? Bruger `&&` og `||` **short-circuit** — højre side evalueres kun, når det er nødvendigt.
:::


## Kombinerede udtryk og operator-prioritet

Ved at kombinere udtryk og operatorer kan du bygge komplekse logikker og udføre beregninger i dine C#-programmer:

```csharp
int score1 = 80, score2 = 90, score3 = 70;
int hours = 120;
int age = 20;
bool hasDriversLicense = true;
bool isVip = false;

double average = (score1 + score2 + score3) / 3.0;
bool bonus = average >= 80 && hours >= 100;
bool result = (age >= 18 && hasDriversLicense) || isVip;
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
- Et expression evalueres til én værdi; et statement udfører en handling
- Der er ingen forskel — begge termer betyder det samme
correct: 1
explain: `2 + 3` er et **expression** — det *er* værdien 5. `Console.WriteLine("Hej");` er et **statement** — det *gør* noget uden at producere en værdi, du kan gemme.
---
q: Hvad er resultatet af `10 / 4` når begge operandi er `int`?
- `2.5`
- `2` (heltalsdivision)
- `3`
correct: 1
explain: Når begge operandi er **heltal**, laver `/` heltalsdivision og dropper decimaldelen. For at få `2.5` skal mindst én operand være decimaltal — fx `10 / 4.0`.
---
q: Hvad returnerer `10 % 3`?
- `3`
- `1` (resten ved division)
- `0`
correct: 1
explain: **`%` (modulus)** giver resten ved heltalsdivision. 10 = 3×3 + **1**, så resultatet er 1. Det er nyttigt til at tjekke om et tal er lige: `n % 2 == 0`.
---
q: Hvilken operator bruger du til at sammenligne to værdier for lighed?
- `=`
- `==`
- `:=`
correct: 1
explain: **`==`** sammenligner to værdier og returnerer `true` eller `false`. **`=`** er **tildeling** — den gemmer en værdi i en variabel. `if (x = 5)` er en fejl; det skal være `if (x == 5)`.
---
q: Hvornår evalueres højre side **ikke** i udtrykket `A && B`?
- Aldrig — begge sider evalueres altid
- Når A er false (short-circuit)
- Når B er true
correct: 1
explain: Med **`&&`** evalueres B kun, hvis A er **true** — det kaldes **short-circuit**. Det beskytter mod fejl, fx `number != 0 && 10/number > 2` crasher ikke, når `number` er 0.
---
q: Hvad er resultatet af `!(5 > 3)`?
- `true`
- `false`
- `5`
correct: 1
explain: **`!`** (logisk IKKE) vender en boolsk værdi. `5 > 3` er `true`, og `!true` bliver **`false`**.
---
q: Hvilken operator har **lavest prioritet** i udtrykket `a && b || c`?
- `&&`
- `||`
- `!`
correct: 1
explain: Operator-prioritet (fra højest til lavest): parenteser → `* / %` → `+ -` → sammenligning → `!` → **`&&`** → **`||`**. Derfor evalueres `||` sidst — brug parenteser, når du er i tvivl.
:::
