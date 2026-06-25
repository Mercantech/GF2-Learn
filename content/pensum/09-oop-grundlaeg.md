---
title: "Object-Oriented Programming"
order: 9
topics: [oop, klasser, objekter]
kompetencemaal:
  - "Kan forklare forskellen på en klasse og et objekt"
  - "Kan oprette klasser med egenskaber (properties) og metoder"
  - "Kan instantiere objekter med new og object initializer"
  - "Kan bruge objekters metoder og egenskaber i programmet"
timer: 3
---

# Klasser og objekter

**Objektorienteret programmering (OOP)** organiserer kode omkring objekter — enheder der kombinerer **data** og **adfærd**. Når vi udvikler i C#, opretter vi **klasser** som skabeloner og **objekter** som konkrete instanser.

En klasse definerer:

- **Egenskaber** — tilstanden eller dataene (fx mærke, alder, farve)
- **Metoder** — handlingerne objektet kan udføre (fx start motor, sæt dig)

:::callout type="info"
**Klasse** = blueprint (skabelon). **Objekt** = den faktiske instans i hukommelsen med egne værdier.
:::


## Eksempel: klassen Car

```csharp
public class Car
{
    // Egenskaber
    public string Brand;
    public string Model;
    public int Year;

    // Metoder
    public void StartEngine()
    {
        Console.WriteLine("Engine started.");
    }

    public void Accelerate()
    {
        Console.WriteLine("Car is accelerating.");
    }
}
```

Klassen `Car` har tre egenskaber (`Brand`, `Model`, `Year`) og to metoder (`StartEngine`, `Accelerate`).


## Oprette objekter med new

Når du vil bruge en klasse, opretter du et **objekt** med nøgleordet `new`:

```csharp
Car myCar = new Car();
```

Nu kan du sætte egenskaber og kalde metoder:

```csharp
myCar.Brand = "Ford";
myCar.Model = "Mustang";
myCar.Year = 1961;

myCar.StartEngine();
myCar.Accelerate();
```

**Object initializer** — sæt egenskaber direkte ved oprettelse:

```csharp
Car myCar = new Car()
{
    Brand = "Ford",
    Model = "Mustang",
    Year = 1961
};
```

Hvert objekt kan have **forskellige værdier** for egenskaberne, men de **deler samme metoder** og adfærd fra klassen.


## Flere objekter — samme klasse

```csharp
var bil1 = new Car { Brand = "Ford", Model = "Mustang", Year = 1961 };
var bil2 = new Car { Brand = "Volvo", Model = "XC60", Year = 2022 };

bil1.StartEngine();   // Ford
bil2.StartEngine();   // Volvo — samme metode, forskellige objekter
```

Det samme princip som i intro-kapitlet med `Person` — én skabelon, mange instanser:

```csharp
class Person
{
    public string Navn { get; set; } = "";
    public int Alder { get; set; }

    public void SigHej()
    {
        Console.WriteLine($"Hej, jeg hedder {Navn} og er {Alder} år.");
    }
}

var elev = new Person { Navn = "Ada", Alder = 17 };
var laerer = new Person { Navn = "Alan", Alder = 35 };

elev.SigHej();    // Hej, jeg hedder Ada og er 17 år.
laerer.SigHej();  // Hej, jeg hedder Alan og er 35 år.
```


## Eksempel: klassen Dog

Klasser kan modellere alt — biler, personer, dyr. Her en `Dog`-klasse med egenskaber og adfærd:

```csharp
public class Dog
{
    public string Color;
    public string EyeColor;
    public string Height;
    public string Length;
    public string Weight;
    public int NumberOfLegs;

    public void Sit()
    {
        Console.WriteLine("Dog sits");
    }

    public void LayDown()
    {
        Console.WriteLine("Dog lays down");
    }

    public void Shake()
    {
        Console.WriteLine("Dog shakes");
    }

    public void Come()
    {
        Console.WriteLine("Dog came to you");
    }
}
```

Opret et objekt og brug det:

```csharp
Dog bobby = new Dog
{
    Color = "Yellow",
    EyeColor = "Brown",
    Height = "17 in",
    Length = "35 in",
    Weight = "24 Pounds",
    NumberOfLegs = 4
};

Console.WriteLine(bobby.Height);   // 17 in
bobby.Sit();                     // Dog sits
```

Klassen er **skabelonen** — `bobby` er ét konkret hund-objekt. Du kan oprette `Dog max = new Dog { ... }` med andre værdier.


## Properties og felter

I eksemplerne ovenfor bruger vi **felter** (`public string Brand`) — det er fint til læring. I moderne C# foretrækkes **properties** med `{ get; set; }`:

```csharp
class Bil
{
    public string Maerke { get; set; } = "";
    public int Aargang { get; set; }
    public double Hastighed { get; private set; }

    public void Accelerer(double kmh)
    {
        Hastighed += kmh;
    }
}

var bil = new Bil { Maerke = "Volvo", Aargang = 2020 };
bil.Accelerer(50);
Console.WriteLine($"{bil.Maerke} kører {bil.Hastighed} km/t");
```

`private set` betyder, at kun klassen selv kan ændre `Hastighed` — det hedder **indkapsling** (encapsulation).


## Metoder og data hører sammen

Metoder i en klasse definerer, hvad objekter *kan gøre*. Data og logik bør høre sammen:

```csharp
class Konto
{
    public string Ejer { get; set; } = "";
    public double Saldo { get; private set; }

    public void Indsaet(double beloeb)
    {
        if (beloeb > 0)
            Saldo += beloeb;
    }

    public bool Hæv(double beloeb)
    {
        if (beloeb > 0 && beloeb <= Saldo)
        {
            Saldo -= beloeb;
            return true;
        }
        return false;
    }
}

var konto = new Konto { Ejer = "Ada" };
konto.Indsaet(1000);
Console.WriteLine(konto.Hæv(300));   // True
Console.WriteLine(konto.Saldo);    // 700
```

`Saldo` kan ikke ændres direkte udefra — kun via `Indsaet` og `Hæv`.


## Instantiere og gemme objekter

```csharp
// Enkelt objekt
var p1 = new Person { Navn = "Ada", Alder = 17 };

// Flere objekter i en liste
var elever = new List<Person>
{
    new Person { Navn = "Ada", Alder = 17 },
    new Person { Navn = "Grace", Alder = 18 }
};

foreach (var elev in elever)
    elev.SigHej();
```

Hvert `new` allokerer et **nyt objekt** i hukommelsen. Objekter er **reference-typer** — variablen peger på objektet, ikke en kopi.


:::git-step
commit: "feat: oop klasser og objekter"
branch: main
:::

## OOP-principper — kort overblik

Tre centrale principper (du uddyber dem senere):

**Indkapsling** — skjul intern data, eksponer kun det nødvendige via properties og metoder.

**Arv** — en klasse kan arve fra en anden og udvide den.

**Polymorfi** — objekter af forskellige klasser kan behandles ens via fælles interface.

I GF2 fokuserer vi på **klasser, objekter og indkapsling**. Arv og polymorfi bygger ovenpå dette fundament.


:::video-list
- [Object-oriented Programming (OOP) [Pt 18] | C# for Beginners](https://www.youtube.com/watch?v=Vp0vVzJgJ5g)
- [OOP with derived or abstract classes, overrides | IEnumerable [Pt 19] | C# for Beginners](https://www.youtube.com/watch?v=P1VJu5V3da8)
:::


## Opsummering

- En **klasse** er en skabelon med egenskaber og metoder
- Et **objekt** oprettes med `new` og har egne værdier
- **Object initializer** `{ Brand = "Ford", ... }` sætter egenskaber ved oprettelse
- Flere objekter kan dele samme klasse med forskellig data
- Properties og indkapsling giver bedre kontrol end offentlige felter alene


:::knowledge-check
---
q: Hvad er forskellen på en **klasse** og et **objekt**?
- De betyder det samme i C#
- Klasse = skabelon; objekt = konkret instans med egne værdier
- Objekt oprettes før klasse
correct: 1
explain: En **klasse** (fx `Car`) definerer struktur og adfærd. Et **objekt** oprettes med `new Car()` og har **egne værdier** for egenskaberne.
---
q: Hvordan opretter du et nyt objekt i C#?
- `Car myCar = Car();`
- `Car myCar = new Car();`
- `create Car myCar;`
correct: 1
explain: Nøgleordet **`new`** allokerer et nyt objekt i hukommelsen. Uden `new` får du compile-fejl — C# kræver eksplicit instansiering.
---
q: Hvad er en **object initializer**?
- En metode der sletter objekter
- Syntaks `{ Brand = "Ford", ... }` der sætter egenskaber ved oprettelse
- En constructor der kun tager strings
correct: 1
explain: Med **object initializer** kan du sætte properties direkte: `new Car { Brand = "Ford", Year = 1961 }` — kortere end at tildele hver property på separate linjer.
---
q: Hvad er **indkapsling** (encapsulation)?
- At arve fra en base-klasse
- At skjule intern data og kun eksponere det nødvendige via properties/metoder
- At oprette mange objekter af samme klasse
correct: 1
explain: Fx `public double Saldo { get; private set; }` — **Saldo** kan læses udefra, men kun klassens egne metoder (`Indsaet`, `Hæv`) kan ændre den.
---
q: Kan to objekter af samme klasse have **forskellige værdier** for egenskaberne?
- Nej — alle objekter deler samme data
- Ja — hvert objekt har egne værdier, men deler samme metoder
- Kun hvis de oprettes med `static`
correct: 1
explain: `bil1` og `bil2` kan have forskellig `Brand` og `Year`, men begge kan kalde `StartEngine()` — **samme adfærd**, **forskellig tilstand**.
---
q: Hvad er forskellen på et **felt** og en **property**?
- Properties kan ikke have get/set
- Property bruger `{ get; set; }` og er foretrukket i moderne C# frem for offentlige felter
- Felter er hurtigere og bør altid bruges
correct: 1
explain: Offentlige felter (`public string Brand`) er fine til læring. **Properties** giver bedre kontrol — fx `private set` — og er standard i professionel C#.
---
q: Objekter i C# er typisk **reference-typer**. Hvad betyder det?
- Variablen indeholder en kopi af hele objektet
- Variablen peger på objektet i hukommelsen — ikke en kopi
- Objekter gemmes kun på disken
correct: 1
explain: Når du skriver `var p1 = new Person { ... }`, peger **`p1`** på objektet i hukommelsen. Flere variabler kan pege på samme objekt — det er grundlaget for reference-semantik.
:::
