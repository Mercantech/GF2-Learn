class Opgave
{
    public string Tekst { get; set; } = "";
    public bool Faerdig { get; set; }
}

// gf2-setup: var opgaver = new List<Opgave> { new() { Tekst = "Læs pensum" }, new() { Tekst = "Øv loops" }, new() { Tekst = "Push til GitHub", Faerdig = true } };

Console.WriteLine("=====================================");
Console.WriteLine("   HUSKELISTE — reference (konsol)");
Console.WriteLine("=====================================");
Console.WriteLine("Menu-drevet to-do — som jeres aflevering.");

while (true)
{
    Console.WriteLine();
    Console.WriteLine("--- Hovedmenu ---");
    Console.WriteLine("1 = Tilføj opgave");
    Console.WriteLine("2 = Vis alle opgaver");
    Console.WriteLine("3 = Markér som færdig / åben");
    Console.WriteLine("4 = Slet opgave");
    Console.WriteLine("5 = Afslut");
    Console.Write("Dit valg: ");
    var valg = Console.ReadLine()?.Trim();

    if (valg == "5")
    {
        Console.WriteLine();
        Console.WriteLine("God arbejdslyst — husk at committe til GitHub!");
        break;
    }

    if (valg == "1")
    {
        Console.Write("Opgavetekst: ");
        var tekst = Console.ReadLine()?.Trim();
        if (string.IsNullOrWhiteSpace(tekst))
        {
            Console.WriteLine("   Tom tekst — prøv igen.");
            continue;
        }

        opgaver.Add(new Opgave { Tekst = tekst });
        Console.WriteLine(">> Tilføjet: " + tekst);
    }
    else if (valg == "2")
    {
        VisOpgaver(opgaver, "--- Dine opgaver ---");
    }
    else if (valg == "3")
    {
        if (opgaver.Count == 0)
        {
            Console.WriteLine("   Ingen opgaver endnu — vælg 1 for at tilføje.");
            continue;
        }

        VisOpgaver(opgaver, "Vælg opgave at skifte status (nummer):");
        Console.Write("Nummer: ");
        if (!int.TryParse(Console.ReadLine(), out var nr) || nr < 1 || nr > opgaver.Count)
        {
            Console.WriteLine("   Ugyldigt nummer.");
            continue;
        }

        var opg = opgaver[nr - 1];
        opg.Faerdig = !opg.Faerdig;
        Console.WriteLine(">> " + (opg.Faerdig ? "Markeret færdig" : "Markeret åben") + ": " + opg.Tekst);
    }
    else if (valg == "4")
    {
        if (opgaver.Count == 0)
        {
            Console.WriteLine("   Ingen opgaver at slette.");
            continue;
        }

        VisOpgaver(opgaver, "Vælg opgave at slette (nummer):");
        Console.Write("Nummer: ");
        if (!int.TryParse(Console.ReadLine(), out var nr) || nr < 1 || nr > opgaver.Count)
        {
            Console.WriteLine("   Ugyldigt nummer.");
            continue;
        }

        var fjernet = opgaver[nr - 1].Tekst;
        opgaver.RemoveAt(nr - 1);
        Console.WriteLine(">> Slettet: " + fjernet);
    }
    else
    {
        Console.WriteLine("Ukendt valg — prøv 1-5.");
    }
}

static void VisOpgaver(List<Opgave> opgaver, string overskrift)
{
    Console.WriteLine();
    Console.WriteLine(overskrift);
    if (opgaver.Count == 0)
    {
        Console.WriteLine("(ingen endnu — vælg 1 for at tilføje)");
        return;
    }

    var aabne = 0;
    for (var i = 0; i < opgaver.Count; i++)
    {
        var mark = opgaver[i].Faerdig ? "[x]" : "[ ]";
        Console.WriteLine("  " + (i + 1) + ". " + mark + " " + opgaver[i].Tekst);
        if (!opgaver[i].Faerdig) aabne++;
    }

    Console.WriteLine("--- Total: " + opgaver.Count + " | Åbne: " + aabne + " ---");
}
