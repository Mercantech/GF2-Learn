---
title: "Variables & Data Types"
order: 2
topics: [variabler, datatyper, strings]
kompetencemaal:
  - "Kan erklære og bruge variabler med korrekte datatyper"
  - "Kan arbejde med strings (concatenation, interpolation og simple metoder)"
  - "Kan skrive og afvikle et simpelt konsolprogram med Console.WriteLine"
  - "Kan navngive og bruge de mest almindelige primitive datatyper i C#"
timer: 3
---

# Variabler og datatyper

En **variabel** er en navngiven boks i hukommelsen, der gemmer data. I C# skal du angive typen eksplicit — så compileren ved, hvilken slags data variablen kan indeholde.

[Variabler (Microsoft Docs)](https://learn.microsoft.com/dotnet/csharp/language-reference/language-specification/variables)

Variabler bruges til at **gemme** og **manipulere** data. En variabel har altid en type — fx heltal, decimaltal, tekst eller sandt/falsk.

## Erklæring og initialisering

For at erklære en variabel angiver du **type** og **navn**:

```csharp
int number;
```

Du kan også **initialisere** variablen med en startværdi med det samme:

```csharp
int age = 25;
string name = "Fogell";
bool isActive = true;
double price = 49.95;
```

Når variablen er oprettet, kan du tildele en **ny værdi** (samme type):

```csharp
age = 30;
```

Du kan kombinere variabler og udføre beregninger:

```csharp
int x = 5;
int y = 10;
int sum = x + y;   // 15
```

**Regler at huske:**

- Variabelnavne starter med bogstav eller `_`, og bruger camelCase: `minScore`, `userName`
- C# skelner mellem store og små bogstaver — `Name` og `name` er to forskellige variabler
- En variabel kan ikke skifte type efter erklæring — `int x = 5; x = "hej";` giver compile-fejl

:::callout type="info"
Brug **altid engelske variabelnavne** — også når opgaven, brugerfladen eller resten af teksten er på dansk. Det er best practice, fordi kode ofte deles på tværs af lande, og fordi dokumentation, biblioteker og frameworks næsten altid bruger engelsk. Skriv fx `studentCount` og `userName` frem for `antalElever` og `brugerNavn`.
:::

### Scope

Variabler har et **scope** (synlighedsområde). En variabel erklæret inde i en metode kan kun bruges **inden for den metode**. Det hjælper med at holde koden overskuelig og undgå fejl.


## Primitive datatyper

I dagligdagen bruger du primært `int`, `double`, `bool` og `string`. Her er den fulde liste over **primitive typer** i C#:

| Keyword | .NET-type | Formål |
|---------|-----------|--------|
| `sbyte` | System.SByte | -128 til 127 (8-bit signed) |
| `byte` | System.Byte | 0 til 255 (8-bit unsigned) |
| `short` | System.Int16 | -32.768 til 32.767 (16-bit signed) |
| `ushort` | System.UInt16 | 0 til 65.535 (16-bit unsigned) |
| `int` | System.Int32 | ca. ±2 milliarder (32-bit signed) |
| `uint` | System.UInt32 | 0 til ca. 4 milliarder (32-bit unsigned) |
| `long` | System.Int64 | Meget store heltal (64-bit signed) |
| `ulong` | System.UInt64 | Meget store positive heltal (64-bit unsigned) |
| `char` | System.Char | Enkelt tegn (16-bit) |
| `float` | System.Single | Decimaltal, ca. 7 decimalers præcision |
| `double` | System.Double | Decimaltal, ca. 15–16 decimalers præcision |
| `bool` | System.Boolean | `true` eller `false` |
| `decimal` | System.Decimal | Decimaltal til fx penge, 28 decimalers præcision |

:::callout type="info"
C# er **statisk typet**: compileren tjekker typer *før* programmet kører. Det fanger mange fejl tidligt — en af grundene til, at C# er populært i erhvervslivet.
:::

## Grundlæggende datatyper i dybden

**int** gemmer hele tal uden decimaler. Det er den type, du bruger til tællere, alder, antal osv.

**double** gemmer decimaltal med flydende komma. Brug `double` til priser, gennemsnit og alt med decimaler. Skriv `10 / 4.0` (ikke `10 / 4`) hvis du vil have et decimaltal.

**bool** kan kun være `true` eller `false`. Bruges til betingelser og flags.

**string** er tekst omgivet af anførselstegn. En string er teknisk set et objekt — ikke en primitiv type — men du bruger den som alle andre variabler.

```csharp
int studentCount = 24;
double average = 7.4;
bool hasPassed = average >= 2.0;
string grade = "Bestået";
```


## Strings

Strenge (strings) er en grundlæggende datatype til **tekst**. En string er en sekvens af tegn — bogstaver, tal, symboler og mellemrum.

Strings er **immutable**: værdien kan ikke ændres efter oprettelse. Når du "ændrer" en string, oprettes en **ny** string i hukommelsen.

### Oprettelse af strings

**Direkte initialisering** (mest brugt):

```csharp
string greeting = "Hello, World!";
```

**Via char-array og konstruktor**:

```csharp
char[] characters = { 'H', 'e', 'l', 'l', 'o' };
string greeting = new string(characters);
```

### Længde og tegnadgang

```csharp
string greeting = "Hello, World!";
int length = greeting.Length;              // 13
char firstChar = greeting[0];              // 'H'
char lastChar = greeting[greeting.Length - 1]; // '!'
```

`.Length` er en **property** (ingen parenteser). Indeks starter på **0**.


## Concatenation og interpolation

**Concatenation** (+) sammenkæder strings:

```csharp
string firstName = "Fogell";
string lastName = "McLovin";
string fullName = firstName + " " + lastName;   // "Fogell McLovin"

// Alternativt
string fullName = String.Concat(firstName, " ", lastName);
```

**Interpolation** (`$"..."`) er den moderne og læsbare måde — variabler indsættes med `{name}`:

```csharp
string firstName = "Fogell";
string lastName = "McLovin";
int age = 17;
Console.WriteLine($"Hej {firstName}!");
Console.WriteLine($"Du er {age} år og hedder {firstName} {lastName}");
```

**String.Format** (ældre stil, stadig nyttig at kende):

```csharp
string message = String.Format("Du er {0} år gammel.", age);
```

**Verbatim-strenge** (`@"..."`) bevarer linjeskift og kræver ikke escape af `\`:

```csharp
string longText = @"Jeg er en lang streng,
som kan skrives på flere linjer!";
```

Interpolation er at foretrække i nyt kode — det er nemmere at læse.

:::video-list
- [Hello World, Introducing Strings! [Pt 4] | C# for Beginners](https://www.youtube.com/watch?v=lXheKhL6dDA)
- [The Basics of Strings! [Pt 5] | C# for Beginners](https://www.youtube.com/watch?v=nus5SP3AoHM)
- [Searching Strings [Pt 6] | C# for Beginners](https://www.youtube.com/watch?v=UETol82cPZk)
:::


## Split, join og søgning

**Split** deler en string op i et array:

```csharp
string sentence = "The quick brown fox";
string[] words = sentence.Split(' ');   // { "The", "quick", "brown", "fox" }
```

**Join** samler et array til én string:

```csharp
string[] words = { "The", "quick", "brown", "fox" };
string sentence = String.Join(" ", words);   // "The quick brown fox"
```

**Søgning** i strings:

```csharp
string sentence = "The quick brown fox";

int index = sentence.IndexOf("quick");       // 4
int lastIndex = sentence.LastIndexOf("o");   // 16
bool containsQuick = sentence.Contains("quick");  // true
bool startsWithThe = sentence.StartsWith("The");  // true
bool endsWithFox = sentence.EndsWith("fox");      // true
```


## Ændring af strings

Selvom strings er immutable, returnerer metoderne en **ny** string med ændringen:

```csharp
string sentence = "The quick brown fox";

string slowSentence = sentence.Replace("quick", "slow"); // "The slow brown fox"
string uppercase = sentence.ToUpper();                    // "THE QUICK BROWN FOX"
string lowercase = sentence.ToLower();                    // "the quick brown fox"

string padded = "   hello   ";
string trimmed = padded.Trim();                      // "hello"
```

`.ToUpper()` og `.ToLower()` er **metoder** (med parenteser). `.Length` er en **property**.


## Dit første konsolprogram med variabler

Et komplet eksempel, der kombinerer variabler, datatyper og output:

```csharp
namespace FirstProgram
{
    public class Program
    {
        static void Main(string[] args)
        {
            string name = "GF2";
            int year = 2025;
            double score = 8.5;
            bool isActive = true;

            Console.WriteLine($"Velkommen til {name}!");
            Console.WriteLine($"År: {year}, Score: {score}, Aktiv: {isActive}");
            Console.WriteLine($"Navn i store bogstaver: {name.ToUpper()}");
        }
    }
}
```

Kør programmet med **F5** i Visual Studio. Output vises i konsolvinduet nederst. Eksperimenter med at ændre værdierne og se, hvad der sker.


:::git-step
commit: "feat: tilfoej variabel-eksempler"
branch: main
:::


## Opsummering

- Variabler gemmer data med en fast type og et navn
- Du kan erklære, initialisere og gen-tildele værdier
- C# har mange primitive typer — start med `int`, `double`, `bool` og `string`
- Strings er immutable og har mange indbyggede metoder til formatering, søgning og manipulation
- Brug `$"..."`-interpolation til læsbar tekst-output med variabler


:::knowledge-check
---
q: Hvad sker der, når du skriver `int x = 5;` og derefter `x = "hej";`?
- Programmet kompilerer og kører — `x` bliver til en string
- Compile-fejl — en `int`-variabel kan ikke få en string-værdi
- Programmet kører, men kaster en runtime-fejl
correct: 1
explain: C# er **statisk typet**. Når `x` er erklæret som `int`, kan den kun indeholde heltal. At tildele `"hej"` giver en fejl **før** programmet kører — compileren fanger det med det samme.
---
q: Hvad er sandt om **strings** i C#?
- En string kan ændres direkte i hukommelsen, når du kalder `.Replace()`
- Strings er immutable — metoder som `.Replace()` returnerer en ny string
- `string` er en primitiv type på linje med `int` og `bool`
correct: 1
explain: Strings kan ikke ændres efter oprettelse. Når du skriver `sentence.Replace("quick", "slow")`, oprettes en **ny** string — den originale er uændret. Teknisk set er `string` en **klasse** (reference-type), ikke en primitiv type.
---
q: Hvad printer `Console.WriteLine(10 / 4);`?
- `2.5`
- `2`
- `2.0`
correct: 1
explain: Begge operandi er `int`, så `/` laver **heltalsdivision** og dropper decimaldelen. Resultatet er `2`. Vil du have `2.5`, skal mindst én operand være decimaltal — fx `10 / 4.0` eller `10.0 / 4`.
---
q: Hvilket variabelnavn følger C#-konventionen **camelCase**?
- `MinScore`
- `min score`
- `minScore`
correct: 2
explain: Lokale variabler og parametre bruger **camelCase** og engelske navne: lille begyndelsesbogstav, derefter store bogstaver ved nye ord — fx `minScore`, `userName`. `MinScore` (PascalCase) bruges typisk til klasser og metoder.
---
q: Hvilken værdi kan en variabel af typen `bool` indeholde?
- Ethvert heltal
- Kun `true` eller `false`
- Tekst-strengene `"true"` og `"false"`
correct: 1
explain: `bool` er C#s sandhedsværdi-type og kan **kun** være `true` eller `false`. `"true"` som tekst er en `string` — ikke en `bool`.
---
q: Hvad gør `$"Velkommen, {name}!"`?
- Skriver kun den bogstavelige tekst `{name}` uden erstatning
- Indsætter værdien af variablen `name` i strengen (string interpolation)
- Konverterer automatisk `name` til store bogstaver
correct: 1
explain: `$"..."` er **string interpolation**. Alt mellem `{` og `}` evalueres og indsættes i teksten. Det er ofte mere læsbart end `"Velkommen, " + name + "!"`.
---
q: Hvad er forskellen på `.Length` og `.ToUpper()` på en string?
- Begge er metoder og kræver `()`
- `.Length` er en property (uden parenteser), `.ToUpper()` er en metode (med `()`)
- `.Length` returnerer en ny string, `.ToUpper()` returnerer et tal
correct: 1
explain: `.Length` fortæller hvor mange tegn strengen har — det er en property, så du skriver `sentence.Length`. `.ToUpper()` er en metode der returnerer en **ny** string i store bogstaver: `sentence.ToUpper()`.
:::
