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

Klassen `Car` har tre egenskaber (`Brand`, `Model`, `Year`) og to metoder. Her opretter vi et objekt med `new`, sætter værdier og kalder metoderne:

```csharp
public class Car
{
    public string Brand;
    public string Model;
    public int Year;

    public void StartEngine()
    {
        Console.WriteLine($"{Brand} {Model}: Engine started.");
    }

    public void Accelerate()
    {
        Console.WriteLine($"{Brand} is accelerating.");
    }
}

Car myCar = new Car();
myCar.Brand = "Ford";
myCar.Model = "Mustang";
myCar.Year = 1961;

myCar.StartEngine();
myCar.Accelerate();
Console.WriteLine($"Årgang: {myCar.Year}");
```


## Object initializer

Med **object initializer** sætter du egenskaber direkte ved oprettelse — kortere end at tildele hver property på separate linjer:

```csharp
public class Car
{
    public string Brand;
    public string Model;
    public int Year;

    public void StartEngine()
    {
        Console.WriteLine($"{Brand} {Model}: Engine started.");
    }
}

Car myCar = new Car
{
    Brand = "Ford",
    Model = "Mustang",
    Year = 1961
};

myCar.StartEngine();
Console.WriteLine($"Årgang: {myCar.Year}");
```

Hvert objekt kan have **forskellige værdier** for egenskaberne, men de **deler samme metoder** og adfærd fra klassen.


## Flere objekter — samme klasse

Én klasse, to objekter med forskellig data:

```csharp
public class Car
{
    public string Brand;
    public string Model;
    public int Year;

    public void StartEngine()
    {
        Console.WriteLine($"{Brand} {Model} ({Year}): Engine started.");
    }
}

var car1 = new Car { Brand = "Ford", Model = "Mustang", Year = 1961 };
var car2 = new Car { Brand = "Volvo", Model = "XC60", Year = 2022 };

car1.StartEngine();
car2.StartEngine();
```

Det samme princip med `Person` — én skabelon, mange instanser:

```csharp
class Person
{
    public string Name { get; set; } = "";
    public int Age { get; set; }

    public void SayHello()
    {
        Console.WriteLine($"Hej, jeg hedder {Name} og er {Age} år.");
    }
}

var student = new Person { Name = "Ada", Age = 17 };
var teacher = new Person { Name = "Alan", Age = 35 };

student.SayHello();
teacher.SayHello();
```


## Eksempel: klassen Dog

Klasser kan modellere alt — biler, personer, dyr. Her opretter vi hunden `bobby` og bruger både egenskaber og metoder:

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

Dog bobby = new Dog
{
    Color = "Yellow",
    EyeColor = "Brown",
    Height = "17 in",
    Length = "35 in",
    Weight = "24 Pounds",
    NumberOfLegs = 4
};

Console.WriteLine($"Bobby er {bobby.Color} og {bobby.Height} høj.");
bobby.Sit();
bobby.Shake();
bobby.Come();
```

Klassen er **skabelonen** — `bobby` er ét konkret hund-objekt. Du kan oprette flere `Dog`-objekter med andre værdier.


## Properties og felter

I eksemplerne ovenfor bruger vi **felter** (`public string Brand`) — det er fint til læring. I moderne C# foretrækkes **properties** med `{ get; set; }`:

```csharp
class Car
{
    public string Brand { get; set; } = "";
    public int Year { get; set; }
    public double Speed { get; private set; }

    public void Accelerate(double speedIncrease)
    {
        Speed += speedIncrease;
    }
}

var car = new Car { Brand = "Volvo", Year = 2020 };
car.Accelerate(50);
car.Accelerate(30);
Console.WriteLine($"{car.Brand} ({car.Year}) kører {car.Speed} km/t");
```

`private set` betyder, at kun klassen selv kan ændre `Speed` — det hedder **indkapsling** (encapsulation).


## Metoder og data hører sammen

Metoder i en klasse definerer, hvad objekter *kan gøre*. Data og logik bør høre sammen:

```csharp
class Account
{
    public string Owner { get; set; } = "";
    public double Balance { get; private set; }

    public void Deposit(double amount)
    {
        if (amount > 0)
            Balance += amount;
    }

    public bool Withdraw(double amount)
    {
        if (amount > 0 && amount <= Balance)
        {
            Balance -= amount;
            return true;
        }
        return false;
    }
}

var account = new Account { Owner = "Ada" };
account.Deposit(1000);
bool withdrawalSucceeded = account.Withdraw(300);

Console.WriteLine($"Ejer: {account.Owner}");
Console.WriteLine($"Hævning lykkedes: {withdrawalSucceeded}");
Console.WriteLine($"Saldo: {account.Balance}");
```

`Balance` kan ikke ændres direkte udefra — kun via `Deposit` og `Withdraw`.


## Instantiere og gemme objekter

Flere objekter i en liste — hvert `new` allokerer et **nyt objekt** i hukommelsen:

```csharp
class Person
{
    public string Name { get; set; } = "";
    public int Age { get; set; }

    public void SayHello()
    {
        Console.WriteLine($"Hej, jeg hedder {Name} og er {Age} år.");
    }
}

var students = new List<Person>
{
    new Person { Name = "Ada", Age = 17 },
    new Person { Name = "Grace", Age = 18 },
    new Person { Name = "Alan", Age = 16 }
};

Console.WriteLine($"Antal elever: {students.Count}");
foreach (var student in students)
    student.SayHello();
```

Objekter er **reference-typer** — variablen peger på objektet, ikke en kopi.


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
explain: Fx `public double Balance { get; private set; }` — **Balance** kan læses udefra, men kun klassens egne metoder (`Deposit`, `Withdraw`) kan ændre den.
---
q: Kan to objekter af samme klasse have **forskellige værdier** for egenskaberne?
- Nej — alle objekter deler samme data
- Ja — hvert objekt har egne værdier, men deler samme metoder
- Kun hvis de oprettes med `static`
correct: 1
explain: `car1` og `car2` kan have forskellig `Brand` og `Year`, men begge kan kalde `StartEngine()` — **samme adfærd**, **forskellig tilstand**.
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
