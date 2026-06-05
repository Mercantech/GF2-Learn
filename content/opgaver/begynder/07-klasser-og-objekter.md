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
    public string Name { get; set; } = "";
    public int Age { get; set; }
}

Person person = new Person { Name = "Emma", Age = 17 };
Console.WriteLine($"{person.Name} er {person.Age} år");
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
class Car
{
    public string Brand { get; set; } = "";
    public string Model { get; set; } = "";
    public int Year { get; set; }
}

Car car1 = new Car { Brand = "Toyota", Model = "Yaris", Year = 2019 };
Car car2 = new Car { Brand = "VW", Model = "ID.3", Year = 2022 };
Console.WriteLine($"{car1.Brand} {car1.Model} ({car1.Year})");
Console.WriteLine($"{car2.Brand} {car2.Model} ({car2.Year})");
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
class Circle
{
    public double Radius { get; set; }
}

Circle circle = new Circle { Radius = 4.5 };
Console.WriteLine($"Radius: {circle.Radius}");
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
    public string Name { get; set; } = "";
    public int Age { get; set; }
    public char GradeLetter { get; set; }
}

Student student = new Student();
student.Name = "Noah";
student.Age = 18;
student.GradeLetter = 'B';
Console.WriteLine($"{student.Name}, {student.Age} år, karakter {student.GradeLetter}");
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
class Rectangle
{
    public double Length { get; set; }
    public double Width { get; set; }
    public double Area => Length * Width;
}

Rectangle rectangle = new Rectangle { Length = 5, Width = 3 };
Console.WriteLine($"Areal: {rectangle.Area}");
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
class BankAccount
{
    public string AccountNumber { get; set; } = "";
    private decimal _balance;
    public decimal Balance
    {
        get => _balance;
        set
        {
            if (value < 0)
            {
                Console.WriteLine("Saldo kan ikke være negativ.");
                return;
            }
            _balance = value;
        }
    }
}

BankAccount account = new BankAccount { AccountNumber = "DK001" };
account.Balance = 500;
Console.WriteLine($"Saldo: {account.Balance}");
account.Balance = -100;
Console.WriteLine($"Saldo efter forsøg: {account.Balance}");
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
class Dog
{
    public string Name { get; }
    public string Breed { get; }

    public Dog(string name, string breed)
    {
        Name = name;
        Breed = breed;
    }
}

Dog dog = new Dog("Balder", "Labrador");
Console.WriteLine($"{dog.Name} er en {dog.Breed}");
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
class Book
{
    public string Title { get; set; } = "";
    public string Author { get; set; } = "";
    public int PageCount { get; set; }

    public Book() { }

    public Book(string title, string author, int pageCount)
    {
        Title = title;
        Author = author;
        PageCount = pageCount;
    }
}

Book book1 = new Book("1984", "George Orwell", 328);
Book book2 = new Book();
book2.Title = "Kort bog";
book2.Author = "Ukendt";
book2.PageCount = 50;
Console.WriteLine($"{book1.Title} — {book1.Author}");
Console.WriteLine($"{book2.Title} — {book2.PageCount} sider");
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
class Point
{
    public int X { get; }
    public int Y { get; }

    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }

    public Point(int x) : this(x, 0) { }
}

Point point1 = new Point(3, 7);
Point point2 = new Point(5);
Console.WriteLine($"p1: ({point1.X}, {point1.Y})");
Console.WriteLine($"p2: ({point2.X}, {point2.Y})");
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
class Calculator
{
    public int Sum(int a, int b) => a + b;
}

Calculator calculator = new Calculator();
Console.WriteLine(calculator.Sum(14, 28));
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
class Circle
{
    public double Radius { get; set; }

    public double CalculateArea() => Math.PI * Radius * Radius;
    public double CalculateCircumference() => 2 * Math.PI * Radius;
}

Circle circle = new Circle { Radius = 3 };
Console.WriteLine($"Areal: {circle.CalculateArea():F2}");
Console.WriteLine($"Omkreds: {circle.CalculateCircumference():F2}");
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
    public string Name { get; set; } = "";
    public int Age { get; set; }

    public void IntroduceYourself()
    {
        Console.WriteLine($"Hej, jeg hedder {Name} og er {Age} år gammel");
    }
}

Person person = new Person { Name = "Sofia", Age = 19 };
person.IntroduceYourself();
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
class Animal
{
    public string Name { get; set; } = "";
    public int Age { get; set; }
}

class Dog : Animal
{
    public string Breed { get; set; } = "";
}

Animal animal = new Animal { Name = "Fugl", Age = 2 };
Dog dog = new Dog { Name = "Max", Age = 4, Breed = "Schæfer" };
Console.WriteLine($"{animal.Name}, {animal.Age} år");
Console.WriteLine($"{dog.Name} ({dog.Breed}), {dog.Age} år");
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
class Vehicle
{
    public string Brand { get; set; } = "";
    public int Year { get; set; }
}

class Car : Vehicle
{
    public int DoorCount { get; set; }

    public void PrintInfo()
    {
        Console.WriteLine($"{Brand} ({Year}), {DoorCount} døre");
    }
}

Car car = new Car { Brand = "Ford", Year = 2020, DoorCount = 5 };
car.PrintInfo();
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
    public string Name { get; set; } = "";
    public string Phone { get; set; } = "";
    private int _age;
    private string _email = "";

    public int Age
    {
        get => _age;
        set
        {
            if (value <= 0)
            {
                Console.WriteLine("Alder skal være positiv.");
                return;
            }
            _age = value;
        }
    }

    public string Email
    {
        get => _email;
        set => _email = IsValidEmail(value) ? value : _email;
    }

    private static bool IsValidEmail(string email) =>
        !string.IsNullOrWhiteSpace(email) && email.Contains('@');

    public void PrintInfo()
    {
        Console.WriteLine($"{Name}, {Age} år — {Email}, tlf. {Phone}");
    }

    public void ChangeEmail(string newEmail)
    {
        if (IsValidEmail(newEmail))
            Email = newEmail;
        else
            Console.WriteLine("Ugyldig email.");
    }
}

Person person = new Person { Name = "Mads", Age = 20, Phone = "12345678" };
person.Email = "mads@test.dk";
person.PrintInfo();
Console.Write("Ny email: ");
person.ChangeEmail(Console.ReadLine()!);
person.PrintInfo();
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
class Vehicle
{
    public string Brand { get; set; } = "";
    public string Model { get; set; } = "";
    public int Year { get; set; }

    public int CalculateAge() => DateTime.Now.Year - Year;
    public bool IsOld() => CalculateAge() > 10;

    public virtual void PrintInfo()
    {
        Console.WriteLine($"{Brand} {Model} ({Year}) — alder {CalculateAge()} år");
    }
}

class Car : Vehicle
{
    public int DoorCount { get; set; }
    public string FuelType { get; set; } = "";

    public override void PrintInfo()
    {
        base.PrintInfo();
        Console.WriteLine($"  Bil: {DoorCount} døre, {FuelType}");
    }
}

class Motorcycle : Vehicle
{
    public int CylinderCount { get; set; }

    public override void PrintInfo()
    {
        base.PrintInfo();
        Console.WriteLine($"  MC: {CylinderCount} cylindre");
    }
}

Vehicle[] display =
{
    new Car { Brand = "Toyota", Model = "Yaris", Year = 2010, DoorCount = 5, FuelType = "Benzin" },
    new Motorcycle { Brand = "Honda", Model = "CB500", Year = 2022, CylinderCount = 2 }
};

foreach (var vehicle in display)
{
    vehicle.PrintInfo();
    Console.WriteLine(vehicle.IsOld() ? "  Gammelt køretøj" : "  Relativt nyt");
}
```

:::
