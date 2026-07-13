class Book
{
    public string Titel { get; set; } = "";
    public string Forfatter { get; set; } = "";
    public bool Ledig { get; set; } = true;
    public string Laantager { get; set; } = "";
}

class Library
{
    public List<Book> Boeger { get; } = new();
}

// gf2-setup: var bibliotek = new Library();
// gf2-setup: bibliotek.Boeger.Add(new Book { Titel = "C# for begyndere", Forfatter = "Mercantec" });
// gf2-setup: bibliotek.Boeger.Add(new Book { Titel = "Clean Code", Forfatter = "Robert C. Martin", Ledig = false, Laantager = "Anna" });
// gf2-setup: bibliotek.Boeger.Add(new Book { Titel = "Git handbook", Forfatter = "GF2 Learn" });

Console.WriteLine("=====================================");
Console.WriteLine("   BIBLIOTEK — reference (konsol)");
Console.WriteLine("=====================================");
Console.WriteLine("OOP med Book og Library — som jeres aflevering.");

while (true)
{
    Console.WriteLine();
    Console.WriteLine("--- Hovedmenu ---");
    Console.WriteLine("1 = Tilføj bog");
    Console.WriteLine("2 = Vis alle bøger");
    Console.WriteLine("3 = Udlån bog");
    Console.WriteLine("4 = Returnér bog");
    Console.WriteLine("5 = Afslut");
    Console.Write("Dit valg: ");
    var valg = Console.ReadLine()?.Trim();

    if (valg == "5")
    {
        Console.WriteLine();
        Console.WriteLine("God læselyst — husk README med klasse-diagram!");
        break;
    }

    if (valg == "1")
    {
        Console.Write("Titel: ");
        var titel = Console.ReadLine()?.Trim();
        if (string.IsNullOrWhiteSpace(titel))
        {
            Console.WriteLine("   Tom titel — prøv igen.");
            continue;
        }

        Console.Write("Forfatter: ");
        var forfatter = Console.ReadLine()?.Trim() ?? "";
        bibliotek.Boeger.Add(new Book { Titel = titel, Forfatter = forfatter });
        Console.WriteLine(">> Tilføjet: \"" + titel + "\" af " + (string.IsNullOrWhiteSpace(forfatter) ? "(ukendt)" : forfatter));
    }
    else if (valg == "2")
    {
        VisBoeger(bibliotek, "--- Bibliotekets bøger ---");
    }
    else if (valg == "3")
    {
        if (bibliotek.Boeger.Count == 0)
        {
            Console.WriteLine("   Ingen bøger — vælg 1 for at tilføje.");
            continue;
        }

        VisBoeger(bibliotek, "Vælg bog at udlåne (nummer):");
        Console.Write("Nummer: ");
        if (!int.TryParse(Console.ReadLine(), out var nr) || nr < 1 || nr > bibliotek.Boeger.Count)
        {
            Console.WriteLine("   Ugyldigt nummer.");
            continue;
        }

        var bog = bibliotek.Boeger[nr - 1];
        if (!bog.Ledig)
        {
            Console.WriteLine("   \"" + bog.Titel + "\" er allerede udlånt til " + bog.Laantager + ".");
            continue;
        }

        Console.Write("Låners navn: ");
        var laantager = Console.ReadLine()?.Trim();
        if (string.IsNullOrWhiteSpace(laantager))
        {
            Console.WriteLine("   Angiv et navn.");
            continue;
        }

        bog.Ledig = false;
        bog.Laantager = laantager;
        Console.WriteLine(">> Udlånt: \"" + bog.Titel + "\" til " + laantager);
    }
    else if (valg == "4")
    {
        if (bibliotek.Boeger.Count == 0)
        {
            Console.WriteLine("   Ingen bøger i biblioteket.");
            continue;
        }

        var udlaan = bibliotek.Boeger.Where(b => !b.Ledig).ToList();
        if (udlaan.Count == 0)
        {
            Console.WriteLine("   Ingen bøger er udlånt lige nu.");
            continue;
        }

        Console.WriteLine();
        Console.WriteLine("--- Udlånte bøger ---");
        for (var i = 0; i < bibliotek.Boeger.Count; i++)
        {
            var bog = bibliotek.Boeger[i];
            if (!bog.Ledig)
                Console.WriteLine("  " + (i + 1) + ". [X] " + bog.Titel + " — lånt af " + bog.Laantager);
        }

        Console.Write("Nummer at returnere: ");
        if (!int.TryParse(Console.ReadLine(), out var nr) || nr < 1 || nr > bibliotek.Boeger.Count)
        {
            Console.WriteLine("   Ugyldigt nummer.");
            continue;
        }

        var valgt = bibliotek.Boeger[nr - 1];
        if (valgt.Ledig)
        {
            Console.WriteLine("   \"" + valgt.Titel + "\" er ikke udlånt.");
            continue;
        }

        var tidligere = valgt.Laantager;
        valgt.Ledig = true;
        valgt.Laantager = "";
        Console.WriteLine(">> Returneret: \"" + valgt.Titel + "\" fra " + tidligere);
    }
    else
    {
        Console.WriteLine("Ukendt valg — prøv 1-5.");
    }
}

static void VisBoeger(Library bibliotek, string overskrift)
{
    Console.WriteLine();
    Console.WriteLine(overskrift);
    if (bibliotek.Boeger.Count == 0)
    {
        Console.WriteLine("(tomt — vælg 1 for at tilføje en bog)");
        return;
    }

    var ledige = 0;
    for (var i = 0; i < bibliotek.Boeger.Count; i++)
    {
        var bog = bibliotek.Boeger[i];
        var status = bog.Ledig ? "[ ]" : "[X]";
        var ekstra = bog.Ledig ? "ledig" : "udlånt til " + bog.Laantager;
        Console.WriteLine("  " + (i + 1) + ". " + status + " " + bog.Titel + " — " + bog.Forfatter + " (" + ekstra + ")");
        if (bog.Ledig) ledige++;
    }

    Console.WriteLine("--- Total: " + bibliotek.Boeger.Count + " | Ledige: " + ledige + " ---");
}
