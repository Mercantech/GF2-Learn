---
title: "Klasser og objekter"
order: 8
difficulty: begynder
category: oop
related_pensum: [09-oop-grundlaeg]
kompetencemaal:
  - "Kan oprette klasser med properties og metoder"
  - "Kan bruge constructors og read-only properties"
  - "Kan arve fra en base-klasse"
  - "Kan strukturere små programmer med OOP"
---

# Kapitel 7 — Klasser og objekter

Seksten opgaver om **klasser**, **objekter**, **properties**, **constructors**, **metoder i klasser** og **arv**. Skriv din kode **efter `// TODO`** i editoren.

:::callout type="tip"
Læs pensum [Klasser og objekter](/curriculum/09-oop-grundlaeg). I browser-editoren: skriv **klassen først** (med `{` og `}`), derefter koden der opretter objekter og kalder metoder — så kan programmet kompilere korrekt.
:::

---

## Grundlæggende — Opgave 1

:::exercise level="begynder"

Lav en klasse **`Person`** med properties for **navn** og **alder**. Opret et objekt og udskriv informationen.

:::

:::code-playground
```csharp
Console.WriteLine("Opgave 1 (Grundlæggende klasse):");
Console.WriteLine("Lav en klasse kaldet 'Person' med properties for navn og alder.");
Console.WriteLine("Opret et objekt af klassen og udskriv informationen.");
// TODO: Lav opgave 1 herunder!
```
:::

:::solution

```csharp
class Person
{
    public string Navn { get; set; } = "";
    public int Alder { get; set; }
}

Person person = new Person { Navn = "Emma", Alder = 17 };
Console.WriteLine($"{person.Navn} er {person.Alder} år");
```

:::

---

## Grundlæggende — Opgave 2

:::exercise level="begynder"

Lav en klasse **`Bil`** med properties for **mærke**, **model** og **årgang**. Opret **to** bil-objekter og udskriv deres information.

:::

:::code-playground
```csharp
Console.WriteLine("Opgave 2 (Grundlæggende klasse):");
Console.WriteLine("Lav en klasse kaldet 'Bil' med properties for mærke, model og årgang.");
Console.WriteLine("Opret to forskellige bil-objekter og udskriv deres information.");
// TODO: Lav opgave 2 herunder!
```
:::

:::solution

```csharp
class Bil
{
    public string Maerke { get; set; } = "";
    public string Model { get; set; } = "";
    public int Aargang { get; set; }
}

Bil bil1 = new Bil { Maerke = "Toyota", Model = "Yaris", Aargang = 2019 };
Bil bil2 = new Bil { Maerke = "VW", Model = "ID.3", Aargang = 2022 };
Console.WriteLine($"{bil1.Maerke} {bil1.Model} ({bil1.Aargang})");
Console.WriteLine($"{bil2.Maerke} {bil2.Model} ({bil2.Aargang})");
```

:::

---

## Grundlæggende — Opgave 3

:::exercise level="begynder"

Lav en klasse **`Cirkel`** med property for **radius**. Opret et cirkel-objekt og udskriv radius.

:::

:::code-playground
```csharp
Console.WriteLine("Opgave 3 (Grundlæggende klasse):");
Console.WriteLine("Lav en klasse kaldet 'Cirkel' med properties for radius.");
Console.WriteLine("Opret et cirkel-objekt og udskriv radiusen.");
// TODO: Lav opgave 3 herunder!
```
:::

:::solution

```csharp
class Cirkel
{
    public double Radius { get; set; }
}

Cirkel c = new Cirkel { Radius = 4.5 };
Console.WriteLine($"Radius: {c.Radius}");
```

:::

---

## Properties — Opgave 4

:::exercise level="begynder"

Lav en klasse **`Student`** med properties for **navn**, **alder** og **karakter**. Brug **get** og **set**. Opret et objekt og sæt/udskriv alle properties.

:::

:::code-playground
```csharp
Console.WriteLine("Opgave 4 (Properties):");
Console.WriteLine("Lav en klasse kaldet 'Student' med properties for navn, alder og karakter.");
Console.WriteLine("Opret et student-objekt og sæt/udskriv alle properties.");
// TODO: Lav opgave 4 herunder!
```
:::

:::solution

```csharp
class Student
{
    public string Navn { get; set; } = "";
    public int Alder { get; set; }
    public char Karakter { get; set; }
}

Student s = new Student();
s.Navn = "Noah";
s.Alder = 18;
s.Karakter = 'B';
Console.WriteLine($"{s.Navn}, {s.Alder} år, karakter {s.Karakter}");
```

:::

---

## Properties — Opgave 5

:::exercise level="begynder"

Lav en klasse **`Rektangel`** med **længde** og **bredde**. Tilføj en **read-only** property **`Areal`** (`længde × bredde`). Udskriv arealet.

:::

:::code-playground
```csharp
Console.WriteLine("Opgave 5 (Properties):");
Console.WriteLine("Lav en klasse kaldet 'Rektangel' med properties for længde og bredde.");
Console.WriteLine("Tilføj en read-only property 'Areal' der beregner længde * bredde.");
// TODO: Lav opgave 5 herunder!
```
:::

:::solution

```csharp
class Rektangel
{
    public double Laengde { get; set; }
    public double Bredde { get; set; }
    public double Areal => Laengde * Bredde;
}

Rektangel r = new Rektangel { Laengde = 5, Bredde = 3 };
Console.WriteLine($"Areal: {r.Areal}");
```

:::

---

## Properties — Opgave 6

:::exercise level="begynder"

Lav en klasse **`BankKonto`** med **kontonummer** og **saldo**. Saldoen må **ikke** blive negativ (tjek i **set**). Prøv positiv og negativ værdi.

:::

:::code-playground
```csharp
Console.WriteLine("Opgave 6 (Properties):");
Console.WriteLine("Lav en klasse kaldet 'BankKonto' med kontonummer og saldo.");
Console.WriteLine("Saldoen må ikke være negativ (brug set-metoden).");
// TODO: Lav opgave 6 herunder!
```
:::

:::solution

```csharp
class BankKonto
{
    public string Kontonummer { get; set; } = "";
    private decimal _saldo;
    public decimal Saldo
    {
        get => _saldo;
        set
        {
            if (value < 0)
            {
                Console.WriteLine("Saldo kan ikke være negativ.");
                return;
            }
            _saldo = value;
        }
    }
}

BankKonto konto = new BankKonto { Kontonummer = "DK001" };
konto.Saldo = 500;
Console.WriteLine($"Saldo: {konto.Saldo}");
konto.Saldo = -100;
Console.WriteLine($"Saldo efter forsøg: {konto.Saldo}");
```

:::

---

## Constructor — Opgave 7

:::exercise level="begynder"

Lav en klasse **`Hund`** med **navn** og **race**. Lav en **constructor** med begge parametre og opret et objekt med den.

:::

:::code-playground
```csharp
Console.WriteLine("Opgave 7 (Constructor):");
Console.WriteLine("Lav en klasse kaldet 'Hund' med navn og race og en constructor med parametre.");
// TODO: Lav opgave 7 herunder!
```
:::

:::solution

```csharp
class Hund
{
    public string Navn { get; }
    public string Race { get; }

    public Hund(string navn, string race)
    {
        Navn = navn;
        Race = race;
    }
}

Hund hund = new Hund("Balder", "Labrador");
Console.WriteLine($"{hund.Navn} er en {hund.Race}");
```

:::

---

## Constructor — Opgave 8

:::exercise level="begynder"

Lav en klasse **`Bog`** med **titel**, **forfatter** og **antal sider**. Lav både en **constructor med alle parametre** og en **default constructor**. Opret bøger med begge.

:::

:::code-playground
```csharp
Console.WriteLine("Opgave 8 (Constructor):");
Console.WriteLine("Lav en klasse 'Bog' med titel, forfatter og antal sider.");
Console.WriteLine("Lav både fuld constructor og default constructor.");
// TODO: Lav opgave 8 herunder!
```
:::

:::solution

```csharp
class Bog
{
    public string Titel { get; set; } = "";
    public string Forfatter { get; set; } = "";
    public int AntalSider { get; set; }

    public Bog() { }

    public Bog(string titel, string forfatter, int antalSider)
    {
        Titel = titel;
        Forfatter = forfatter;
        AntalSider = antalSider;
    }
}

Bog b1 = new Bog("1984", "George Orwell", 328);
Bog b2 = new Bog();
b2.Titel = "Kort bog";
b2.Forfatter = "Ukendt";
b2.AntalSider = 50;
Console.WriteLine($"{b1.Titel} — {b1.Forfatter}");
Console.WriteLine($"{b2.Titel} — {b2.AntalSider} sider");
```

:::

---

## Constructor — Opgave 9

:::exercise level="begynder"

Lav en klasse **`Punkt`** med **x** og **y**. Constructor med **x og y**, plus **overloaded** constructor med kun **x** (y = 0).

:::

:::code-playground
```csharp
Console.WriteLine("Opgave 9 (Constructor):");
Console.WriteLine("Lav en klasse 'Punkt' med x og y og to constructors (x,y og kun x).");
// TODO: Lav opgave 9 herunder!
```
:::

:::solution

```csharp
class Punkt
{
    public int X { get; }
    public int Y { get; }

    public Punkt(int x, int y)
    {
        X = x;
        Y = y;
    }

    public Punkt(int x) : this(x, 0) { }
}

Punkt p1 = new Punkt(3, 7);
Punkt p2 = new Punkt(5);
Console.WriteLine($"p1: ({p1.X}, {p1.Y})");
Console.WriteLine($"p2: ({p2.X}, {p2.Y})");
```

:::

---

## Metoder i klasser — Opgave 10

:::exercise level="begynder"

Lav en klasse **`Lommeregner`** med en metode, der tager **to tal** og **returnerer summen**. Opret et objekt og test metoden.

:::

:::code-playground
```csharp
Console.WriteLine("Opgave 10 (Methods i klasser):");
Console.WriteLine("Lav en klasse 'Lommeregner' med en method der returnerer summen af to tal.");
// TODO: Lav opgave 10 herunder!
```
:::

:::solution

```csharp
class Lommeregner
{
    public int Sum(int a, int b) => a + b;
}

Lommeregner calc = new Lommeregner();
Console.WriteLine(calc.Sum(14, 28));
```

:::

---

## Metoder i klasser — Opgave 11

:::exercise level="begynder"

Lav en klasse **`Cirkel`** med **radius** og metoder til **areal** og **omkreds**. Udskriv begge.

:::

:::code-playground
```csharp
Console.WriteLine("Opgave 11 (Methods i klasser):");
Console.WriteLine("Lav en klasse 'Cirkel' med radius og methods for areal og omkreds.");
// TODO: Lav opgave 11 herunder!
```
:::

:::solution

```csharp
class Cirkel
{
    public double Radius { get; set; }

    public double BeregnAreal() => Math.PI * Radius * Radius;
    public double BeregnOmkreds() => 2 * Math.PI * Radius;
}

Cirkel c = new Cirkel { Radius = 3 };
Console.WriteLine($"Areal: {c.BeregnAreal():F2}");
Console.WriteLine($"Omkreds: {c.BeregnOmkreds():F2}");
```

:::

---

## Metoder i klasser — Opgave 12

:::exercise level="begynder"

Lav en klasse **`Person`** med **navn** og **alder** og metoden **`IntroduceYourself()`**, der udskriver en hilsen.

:::

:::code-playground
```csharp
Console.WriteLine("Opgave 12 (Methods i klasser):");
Console.WriteLine("Tilføj IntroduceYourself() der udskriver navn og alder.");
// TODO: Lav opgave 12 herunder!
```
:::

:::solution

```csharp
class Person
{
    public string Navn { get; set; } = "";
    public int Alder { get; set; }

    public void IntroduceYourself()
    {
        Console.WriteLine($"Hej, jeg hedder {Navn} og er {Alder} år gammel");
    }
}

Person p = new Person { Navn = "Sofia", Alder = 19 };
p.IntroduceYourself();
```

:::

---

## Arv — Opgave 13

:::exercise level="begynder"

Lav en base-klasse **`Dyr`** med **navn** og **alder**. Lav **`Hund`** der arver fra `Dyr` med ekstra property **race**. Opret begge objekttyper.

:::

:::code-playground
```csharp
Console.WriteLine("Opgave 13 (Inheritance):");
Console.WriteLine("Base-klasse Dyr og derived klasse Hund med race.");
// TODO: Lav opgave 13 herunder!
```
:::

:::solution

```csharp
class Dyr
{
    public string Navn { get; set; } = "";
    public int Alder { get; set; }
}

class Hund : Dyr
{
    public string Race { get; set; } = "";
}

Dyr dyr = new Dyr { Navn = "Fugl", Alder = 2 };
Hund hund = new Hund { Navn = "Max", Alder = 4, Race = "Schæfer" };
Console.WriteLine($"{dyr.Navn}, {dyr.Alder} år");
Console.WriteLine($"{hund.Navn} ({hund.Race}), {hund.Alder} år");
```

:::

---

## Arv — Opgave 14

:::exercise level="begynder"

Lav **`Køretøj`** med **mærke** og **årgang**. Lav **`Bil`** der arver med **antal døre** og en metode, der udskriver alle oplysninger.

:::

:::code-playground
```csharp
Console.WriteLine("Opgave 14 (Inheritance):");
Console.WriteLine("Base Køretøj og derived Bil med antal døre og udskriv-metode.");
// TODO: Lav opgave 14 herunder!
```
:::

:::solution

```csharp
class Koeretoej
{
    public string Maerke { get; set; } = "";
    public int Aargang { get; set; }
}

class Bil : Koeretoej
{
    public int AntalDoere { get; set; }

    public void UdskrivInfo()
    {
        Console.WriteLine($"{Maerke} ({Aargang}), {AntalDoere} døre");
    }
}

Bil bil = new Bil { Maerke = "Ford", Aargang = 2020, AntalDoere = 5 };
bil.UdskrivInfo();
```

:::

---

## Mini-projekt — Personstyring

:::exercise level="begynder"

Lav et lille **personstyring**: klasse med navn, alder, email og telefon — validering (email med `@`, positiv alder), metode til fuld info og metode til at skifte email. Opret flere personer og test.

:::

:::code-playground
```csharp
// gf2-input: ny@mail.dk
Console.WriteLine("Mini-projekt: Person management system");
Console.WriteLine("- Person med navn, alder, email, telefon");
Console.WriteLine("- Validering og metoder til info og skift email");
// TODO: Lav opgave 15 herunder!
```
:::

:::solution

```csharp
class Person
{
    public string Navn { get; set; } = "";
    public string Telefon { get; set; } = "";
    private int _alder;
    private string _email = "";

    public int Alder
    {
        get => _alder;
        set
        {
            if (value <= 0)
            {
                Console.WriteLine("Alder skal være positiv.");
                return;
            }
            _alder = value;
        }
    }

    public string Email
    {
        get => _email;
        set => _email = ErGyldigEmail(value) ? value : _email;
    }

    private static bool ErGyldigEmail(string email) =>
        !string.IsNullOrWhiteSpace(email) && email.Contains('@');

    public void UdskrivInfo()
    {
        Console.WriteLine($"{Navn}, {Alder} år — {Email}, tlf. {Telefon}");
    }

    public void SkiftEmail(string nyEmail)
    {
        if (ErGyldigEmail(nyEmail))
            Email = nyEmail;
        else
            Console.WriteLine("Ugyldig email.");
    }
}

Person p = new Person { Navn = "Mads", Alder = 20, Telefon = "12345678" };
p.Email = "mads@test.dk";
p.UdskrivInfo();
Console.Write("Ny email: ");
p.SkiftEmail(Console.ReadLine()!);
p.UdskrivInfo();
```

:::

---

## Mini-projekt — Bil-showroom

:::exercise level="begynder"

Lav et **showroom**: base **`Køretøj`** (mærke, model, årgang), **`Bil`** (døre, brændstof) og **`Motorcykel`** (cylinderantal). Metoder til info, alder og om køretøjet er **over 10 år**.

:::

:::code-playground
```csharp
Console.WriteLine("Mini-projekt: Bil showroom");
Console.WriteLine("- Køretøj, Bil og Motorcykel med arv og metoder");
// TODO: Lav opgave 16 herunder!
```
:::

:::solution

```csharp
class Koeretoej
{
    public string Maerke { get; set; } = "";
    public string Model { get; set; } = "";
    public int Aargang { get; set; }

    public int BeregnAlder() => DateTime.Now.Year - Aargang;
    public bool ErGammel() => BeregnAlder() > 10;

    public virtual void UdskrivInfo()
    {
        Console.WriteLine($"{Maerke} {Model} ({Aargang}) — alder {BeregnAlder()} år");
    }
}

class Bil : Koeretoej
{
    public int AntalDoere { get; set; }
    public string Braendstof { get; set; } = "";

    public override void UdskrivInfo()
    {
        base.UdskrivInfo();
        Console.WriteLine($"  Bil: {AntalDoere} døre, {Braendstof}");
    }
}

class Motorcykel : Koeretoej
{
    public int CylinderAntal { get; set; }

    public override void UdskrivInfo()
    {
        base.UdskrivInfo();
        Console.WriteLine($"  MC: {CylinderAntal} cylindre");
    }
}

Koeretoej[] udstilling =
{
    new Bil { Maerke = "Toyota", Model = "Yaris", Aargang = 2010, AntalDoere = 5, Braendstof = "Benzin" },
    new Motorcykel { Maerke = "Honda", Model = "CB500", Aargang = 2022, CylinderAntal = 2 }
};

foreach (var k in udstilling)
{
    k.UdskrivInfo();
    Console.WriteLine(k.ErGammel() ? "  Gammelt køretøj" : "  Relativt nyt");
}
```

:::
