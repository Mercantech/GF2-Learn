---
title: "Code Style & Naming"
order: 15
category: kodeprincipper
topics: [stil, navngivning, læsbarhed, formatering]
kompetencemaal:
  - "Kan navngive variabler og metoder beskrivende på engelsk"
  - "Kan skrive kode med ensartede linjeskift og indrykning"
  - "Kan forklare hvorfor læsbar kode er vigtig — også når programmet virker"
  - "Kan genkende dårlig stil (fx x, temp, data) og forbedre den"
timer: 1
---

# Kode-stil og navngivning

Din kode skal ikke bare **virke** — den skal også være **let at læse** for dig selv og andre. God stil handler om små vaner: beskrivende navne, luft mellem idéer og ensartet formatering.

:::callout type="info"
I GF2 Learn kan du **formatere kode** i opgave-editoren med **Ctrl+Shift+F** (eller knappen med linje-ikonet). Det retter indrykning og linjeskift efter almindelige C#-regler.
:::


## Beskrivende navne

En variabel eller metode skal fortælle **hvad den indeholder** eller **hvad den gør** — uden at du skal gætte.

| Dårligt | Bedre | Hvorfor |
|---------|-------|---------|
| `x` | `studentCount` | Du kan se, at det er et antal elever |
| `temp` | `totalPrice` | Du ved, at det er en pris — ikke "noget midlertidigt" |
| `data` | `userNames` | Du ved, at det er navne |
| `DoStuff()` | `PrintWelcomeMessage()` | Metodenavnet beskriver handlingen |

```csharp
// Svært at forstå
int x = 10;
Console.WriteLine(x);

// Nemt at forstå
int score = 10;
Console.WriteLine(score);
```

:::callout type="warning"
Undgå **forkortelser** som `cnt`, `msg`, `nr` — medmindre hele teamet bruger dem konsekvent. `message` og `count` er tydeligere for de fleste.
:::


## Navngivning i C#

| Element | Konvention | Eksempel |
|---------|------------|----------|
| **Lokale variabler** | camelCase | `firstName`, `itemCount` |
| **Metoder** | PascalCase, start med **verbum** | `CalculateTotal()`, `ReadUserInput()` |
| **Konstanter** | ofte PascalCase eller UPPER_CASE | `MaxAttempts`, `DefaultTimeout` |
| **Klasser** | PascalCase, substantiv | `Student`, `OrderLine` |

Metoder beskriver en **handling** — brug et verbum:

```csharp
PrintMenu();
Console.WriteLine($"Sum: {CalculateSum(3, 4)}");

static void PrintMenu()
{
    Console.WriteLine("1. Start");
    Console.WriteLine("2. Afslut");
}

static int CalculateSum(int a, int b)
{
    return a + b;
}
```


## Linjeskift og luft i koden

Brug **tomme linjer** til at adskille logiske trin — som afsnit i en tekst.

```csharp
// Uoverskueligt — én lang blok
int age = 18;
string name = "Ada";
bool canVote = age >= 18;
Console.WriteLine(name);
Console.WriteLine(canVote);

// Overskueligt — grupperet
int age = 18;
string name = "Ada";

bool canVote = age >= 18;

Console.WriteLine(name);
Console.WriteLine(canVote);
```

**Én idé per linje** hvor det giver mening. Undgå at presse tre ting ind på én linje:

```csharp
// Svært at læse
int a=1,b=2,c=a+b;Console.WriteLine(c);

// Bedre
int a = 1;
int b = 2;
int sum = a + b;
Console.WriteLine(sum);
```


## Indrykning og klammer

I C# bruger vi typisk **4 mellemrum** pr. indrykningsniveau. Klammer `{` og `}` står ofte på **egen linje** (Allman-stil):

```csharp
if (score >= 60)
{
    Console.WriteLine("Bestået");
}
else
{
    Console.WriteLine("Ikke bestået endnu");
}
```

Indrykning viser **hvad der hører sammen** — især i `if`, `else`, `for` og `while`:

```csharp
for (int i = 0; i < 3; i++)
{
    Console.WriteLine($"Runde {i}");
}
```

Kør **Ctrl+Shift+F** i GF2 Learn, hvis indrykningen er rodet efter copy-paste.


## Små metoder — én opgave ad gangen

Del kode op i **korte metoder**, der hver gør én ting. Det gør programmet nemmere at læse og fejlsøge.

```csharp
RunProgram();

static void RunProgram()
{
    PrintWelcome();
    int score = ReadScore();
    PrintResult(score);
}

static void PrintWelcome()
{
    Console.WriteLine("Velkommen til quiz!");
}

static int ReadScore()
{
    Console.Write("Indtast point: ");
    string input = Console.ReadLine() ?? "0";
    return int.Parse(input);
}

static void PrintResult(int score)
{
    if (score >= 60)
    {
        Console.WriteLine("Godt klaret!");
    }
    else
    {
        Console.WriteLine("Øv lidt mere.");
    }
}
```

Du behøver ikke mange metoder fra start — men når en `Main` bliver lang, er det et tegn på, at du kan opdele.


## Kommentarer — kort og med mening

Kommentarer forklarer **hvorfor**, ikke **hvad** — koden skal kunne læses uden for mange kommentarer.

```csharp
// Dårligt — gentager koden
int age = 18; // sætter age til 18

// Bedre — forklarer en beslutning
const int VotingAge = 18; // lovkrav i eksemplet
bool canVote = age >= VotingAge;
```

Brug `// TODO:` til ting, du skal lave senere — som i GF2-opgaverne.


## Tjekliste før du afleverer

- [ ] Variabelnavne er **beskrivende** (engelsk, camelCase)
- [ ] Metodenavne starter med et **verbum** (`Print`, `Calculate`, `Read`)
- [ ] Der er **luft** mellem logiske trin (tom linje)
- [ ] **Indrykning** er ensartet (brug formatering)
- [ ] Ingen "magiske tal" uden forklaring — brug fx `const int MaxAttempts = 3;`
- [ ] Koden kan læses **højt op** næsten som en sætning


:::knowledge-check
---
q: Hvorfor er beskrivende variabelnavne vigtige?
- De gør programmet hurtigere at køre
- De gør koden lettere at læse og forstå — også for dig selv om en uge
- De er kun påkrævet i store virksomheder
correct: 1
explain: Gode navne som `totalPrice` og `studentCount` sparer tid, når du (eller en lærer) skal finde fejl eller videreudvikle koden.
---
q: Hvilket metodenavn følger bedst C#-konvention?
- `data()`
- `stuff()`
- `PrintWelcomeMessage()`
correct: 2
explain: Metoder i C# bruger **PascalCase** og beskriver en **handling** med et verbum — fx `Print`, `Calculate`, `Read`.
---
q: Hvad er formålet med en **tom linje** mellem to kodestykker?
- Den er påkrævet af compileren
- Den adskiller logiske trin og gør koden mere overskuelig
- Den gør filen mindre
correct: 1
explain: Tomme linjer er som afsnit i en tekst — de hjælper øjet med at se, hvor én idé slutter og den næste begynder.
---
q: Hvad gør **Ctrl+Shift+F** i GF2 Learn editoren?
- Gemmer din løsning
- Formaterer koden (indrykning og linjeskift)
- Kører programmet
correct: 1
explain: Formatering retter indrykning og klammer, så koden følger almindelig C#-stil — især nyttigt efter copy-paste.
---
q: Hvilken kommentar er **bedst**?
- `int age = 18; // sætter age til 18`
- `// TODO: lav opgave 2`
- `const int VotingAge = 18; // minimumsalder i eksemplet`
correct: 2
explain: God kommentar forklarer **hvorfor** eller markerer **arbejde der mangler** — ikke det, koden allerede siger tydeligt.
---
q: Hvad betyder det, at en metode har **ét ansvar**?
- Den må kun have én parameter
- Den gør én tydelig ting — fx kun at læse input eller kun at udskrive
- Den må kun kaldes én gang
correct: 1
explain: Korte metoder som `ReadScore()` og `PrintResult()` gør koden nemmere at teste, læse og rette end én lang `Main` med alt i én blok.
:::
