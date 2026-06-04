---
title: "Metoder"
order: 7
difficulty: begynder
related_pensum: [06-metoder]
kompetencemaal:
  - "Kan definere og kalde egne metoder"
  - "Kan bruge parametre og returværdier"
  - "Kan strukturere programmer med flere små metoder"
  - "Kan lave simple rekursive metoder med base case"
---

# Kapitel 6 — Metoder

Femten opgaver om **metoder**, **parametre**, **returværdier** og **rekursion** — plus to mini-projekter. Skriv din kode **efter `// TODO`** i editoren under hver opgave.

:::callout type="tip"
Læs pensum [Metoder](/curriculum/06-metoder). I editoren kan du definere **lokale metoder** i samme program (fx `void Hej() { ... }` og derefter `Hej();`). Til flere hjælpemetoder kan du bruge `// gf2-setup:` — linjen indsættes over din kode. Ved input: `// gf2-input:`.
:::

---

## Grundlæggende — Opgave 1

:::exercise level="begynder"

Lav en metode, der udskriver **«Hej verden!»**, og kald den fra programmet.

:::

:::code-playground
```csharp
Console.WriteLine("Opgave 1 (Grundlæggende method):");
Console.WriteLine("Lav en method der udskriver 'Hej verden!' og kald den fra denne method.");
// TODO: Lav opgave 1 herunder!
```
:::

:::solution

```csharp
void HejVerden()
{
    Console.WriteLine("Hej verden!");
}
HejVerden();
```

:::

---

## Grundlæggende — Opgave 2

:::exercise level="begynder"

Lav en metode, der udskriver tallene **fra 1 til 5**, og kald den.

:::

:::code-playground
```csharp
Console.WriteLine("Opgave 2 (Grundlæggende method):");
Console.WriteLine("Lav en method der udskriver tallene fra 1 til 5 og kald den fra denne method.");
// TODO: Lav opgave 2 herunder!
```
:::

:::solution

```csharp
void UdskrivEtTilFem()
{
    for (int i = 1; i <= 5; i++)
        Console.WriteLine(i);
}
UdskrivEtTilFem();
```

:::

---

## Grundlæggende — Opgave 3

:::exercise level="begynder"

Lav en metode, der beder brugeren om deres **navn** og **hilser** på dem. Kald metoden.

:::

:::code-playground
```csharp
// gf2-input: Mads
Console.WriteLine("Opgave 3 (Grundlæggende method):");
Console.WriteLine("Lav en method der beder brugeren om deres navn og hilser på dem, og kald den fra denne method.");
// TODO: Lav opgave 3 herunder!
```
:::

:::solution

```csharp
void Hilse()
{
    Console.Write("Hvad hedder du? ");
    string navn = Console.ReadLine()!;
    Console.WriteLine($"Hej {navn}!");
}
Hilse();
```

:::

---

## Parameter — Opgave 4

:::exercise level="begynder"

Lav en metode, der tager et **navn** som parameter og udskriver **«Hej [navn]!»**. Kald metoden med dit eget navn.

:::

:::code-playground
```csharp
Console.WriteLine("Opgave 4 (Parameter):");
Console.WriteLine("Lav en method der tager et navn som parameter og udskriver 'Hej [navn]!'");
// TODO: Lav opgave 4 herunder! Kald metoden med dit eget navn.
```
:::

:::solution

```csharp
void SigHej(string navn)
{
    Console.WriteLine($"Hej {navn}!");
}
SigHej("Sofia");
```

:::

---

## Parameter — Opgave 5

:::exercise level="begynder"

Lav en metode, der tager **to tal** som parametre og udskriver **summen**. Kald metoden med to forskellige tal.

:::

:::code-playground
```csharp
Console.WriteLine("Opgave 5 (Parameter):");
Console.WriteLine("Lav en method der tager to tal som parametre og udskriver summen af dem.");
// TODO: Lav opgave 5 herunder! Kald metoden med to forskellige tal.
```
:::

:::solution

```csharp
void UdskrivSum(int a, int b)
{
    Console.WriteLine(a + b);
}
UdskrivSum(12, 30);
```

:::

---

## Parameter — Opgave 6

:::exercise level="begynder"

Lav en metode, der tager et **tal** som parameter og tjekker, om det er **lige eller ulige**. Kald metoden med både et lige og et ulige tal.

:::

:::code-playground
```csharp
Console.WriteLine("Opgave 6 (Parameter):");
Console.WriteLine("Lav en method der tager et tal som parameter og tjekker om det er lige eller ulige.");
// TODO: Lav opgave 6 herunder! Kald med både et lige og et ulige tal.
```
:::

:::solution

```csharp
void TjekLigeUlige(int tal)
{
    if (tal % 2 == 0)
        Console.WriteLine($"{tal} er lige");
    else
        Console.WriteLine($"{tal} er ulige");
}
TjekLigeUlige(8);
TjekLigeUlige(7);
```

:::

---

## Parameter — Opgave 7

:::exercise level="begynder"

Lav en metode, der tager **navn**, **alder** og **by** som parametre og udskriver en kort præsentation.

:::

:::code-playground
```csharp
Console.WriteLine("Opgave 7 (Flere parametre):");
Console.WriteLine("Lav en method der tager navn, alder og by som parametre og udskriver en præsentation.");
Console.WriteLine("Eksempel: Jeg hedder [navn], er [alder] år gammel og kommer fra [by]");
// TODO: Lav opgave 7 herunder!
```
:::

:::solution

```csharp
void Praesenter(string navn, int alder, string by)
{
    Console.WriteLine($"Jeg hedder {navn}, er {alder} år gammel og kommer fra {by}");
}
Praesenter("Emma", 17, "Aarhus");
```

:::

---

## Returværdi — Opgave 8

:::exercise level="begynder"

Lav en metode, der tager **to tal** som parametre og **returnerer summen**. Udskriv resultatet.

:::

:::code-playground
```csharp
Console.WriteLine("Opgave 8 (Return value):");
Console.WriteLine("Lav en method der tager to tal som parametre og returnerer summen. Udskriv resultatet.");
// TODO: Lav opgave 8 herunder!
```
:::

:::solution

```csharp
int BeregnSum(int a, int b)
{
    return a + b;
}
int resultat = BeregnSum(15, 27);
Console.WriteLine(resultat);
```

:::

---

## Returværdi — Opgave 9

:::exercise level="begynder"

Lav en metode, der tager et **tal** som parameter og returnerer, om det er **lige** (`true` / `false`).

:::

:::code-playground
```csharp
Console.WriteLine("Opgave 9 (Return value):");
Console.WriteLine("Lav en method der tager et tal som parameter og returnerer om det er lige (true/false).");
// TODO: Lav opgave 9 herunder!
```
:::

:::solution

```csharp
bool ErLige(int tal)
{
    return tal % 2 == 0;
}
Console.WriteLine(ErLige(10));
Console.WriteLine(ErLige(11));
```

:::

---

## Returværdi — Opgave 10

:::exercise level="begynder"

Lav en metode, der tager et **navn** som parameter og **returnerer** strengen **«Hej [navn]!»**.

:::

:::code-playground
```csharp
Console.WriteLine("Opgave 10 (Return value):");
Console.WriteLine("Lav en method der tager et navn som parameter og returnerer 'Hej [navn]!'");
// TODO: Lav opgave 10 herunder!
```
:::

:::solution

```csharp
string LavHilsen(string navn)
{
    return $"Hej {navn}!";
}
Console.WriteLine(LavHilsen("Noah"));
```

:::

---

## Returværdi — Opgave 11

:::exercise level="begynder"

Lav en metode, der tager **tre tal** som parametre og returnerer det **største** tal.

:::

:::code-playground
```csharp
Console.WriteLine("Opgave 11 (Return value):");
Console.WriteLine("Lav en method der tager tre tal som parametre og returnerer det største tal.");
// TODO: Lav opgave 11 herunder!
```
:::

:::solution

```csharp
int Stoerste(int a, int b, int c)
{
    int max = a;
    if (b > max) max = b;
    if (c > max) max = c;
    return max;
}
Console.WriteLine(Stoerste(3, 9, 5));
```

:::

---

## Rekursion — Opgave 12

:::exercise level="begynder"

Lav en **rekursiv** metode, der beregner **fakultet** af et tal (fx `5! = 5×4×3×2×1`).

:::

:::code-playground
```csharp
Console.WriteLine("Opgave 12 (Recursion):");
Console.WriteLine("Lav en rekursiv method der beregner fakultet af et tal (f.eks. 5! = 5*4*3*2*1).");
Console.WriteLine("Tip: Fakultet af n = n * fakultet af (n-1), og fakultet af 1 = 1");
// TODO: Lav opgave 12 herunder!
```
:::

:::solution

```csharp
int Fakultet(int n)
{
    if (n <= 1)
        return 1;
    return n * Fakultet(n - 1);
}
Console.WriteLine(Fakultet(5));
```

:::

---

## Rekursion — Opgave 13

:::exercise level="begynder"

Lav en **rekursiv** metode, der **tæller ned** fra et givet tal til **0** (fx `3, 2, 1, 0`).

:::

:::code-playground
```csharp
Console.WriteLine("Opgave 13 (Recursion):");
Console.WriteLine("Lav en rekursiv method der tæller ned fra et givet tal til 0.");
Console.WriteLine("Eksempel: CountDown(3) skal udskrive: 3, 2, 1, 0");
// TODO: Lav opgave 13 herunder!
```
:::

:::solution

```csharp
void CountDown(int n)
{
    Console.WriteLine(n);
    if (n > 0)
        CountDown(n - 1);
}
CountDown(3);
```

:::

---

## Mini-projekt — Lommeregner

:::exercise level="begynder"

Lav et program med **separate metoder** for de fire regnearter (`+`, `-`, `*`, `/`). Hver metode tager to tal og **returnerer** resultatet. En **hoveddel** af programmet beder om to tal og en operation og kalder den rigtige metode.

:::

:::code-playground
```csharp
// gf2-input: 20
// gf2-input: 4
// gf2-input: /
Console.WriteLine("Mini-projekt: Lommeregner med methods");
Console.WriteLine("Lav et program med separate methods for +, -, *, /.");
Console.WriteLine("Hver method returnerer resultatet. Bed om to tal og en operation (+ - * /).");
// TODO: Lav opgave 14 herunder!
```
:::

:::solution

```csharp
double Plus(double a, double b) => a + b;
double Minus(double a, double b) => a - b;
double Gange(double a, double b) => a * b;
double Divider(double a, double b) => a / b;

Console.Write("Tal 1: ");
double t1 = double.Parse(Console.ReadLine()!);
Console.Write("Tal 2: ");
double t2 = double.Parse(Console.ReadLine()!);
Console.Write("Operation (+ - * /): ");
string op = Console.ReadLine()!.Trim();

double resultat = op switch
{
    "+" => Plus(t1, t2),
    "-" => Minus(t1, t2),
    "*" => Gange(t1, t2),
    "/" => Divider(t1, t2),
    _ => double.NaN
};
Console.WriteLine($"Resultat: {resultat}");
```

:::

---

## Mini-projekt — Gæt-et-tal

:::exercise level="begynder"

Lav et **gæt-et-tal-spil** med metoder til: tilfældigt tal, brugerens gæt, sammenligning og feedback (**«for højt»**, **«for lavt»**, **«rigtigt!»**).

:::

:::code-playground
```csharp
// gf2-setup: Random rnd = new Random(42);
// gf2-setup: int hemmeligt = rnd.Next(1, 11);
// gf2-input: 5
// gf2-input: 8
// gf2-input: 7
Console.WriteLine("Mini-projekt: Gæt-et-tal spil med methods");
Console.WriteLine("- Metode til tilfældigt tal (eller brug hemmeligt fra gf2-setup)");
Console.WriteLine("- Metode til brugerens gæt");
Console.WriteLine("- Metode til sammenligning og feedback");
// TODO: Lav opgave 15 herunder!
```
:::

:::solution

```csharp
int LavHemmeligtTal()
{
    Random rnd = new Random();
    return rnd.Next(1, 11);
}

int LaesGaet()
{
    Console.Write("Gæt et tal (1-10): ");
    return int.Parse(Console.ReadLine()!);
}

string GivFeedback(int gaet, int hemmeligt)
{
    if (gaet == hemmeligt)
        return "Rigtigt!";
    if (gaet > hemmeligt)
        return "For højt!";
    return "For lavt!";
}

int hemmeligt = LavHemmeligtTal();
while (true)
{
    int gaet = LaesGaet();
    string feedback = GivFeedback(gaet, hemmeligt);
    Console.WriteLine(feedback);
    if (feedback == "Rigtigt!")
        break;
}
```

:::
