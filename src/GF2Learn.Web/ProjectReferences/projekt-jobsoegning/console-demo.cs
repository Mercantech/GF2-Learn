// gf2-setup: var stillinger = new List<string> { "Mercantec | GF2-elev | Ansøgt", "IT A/S | Junior dev | Afventer" };
// Simuleret tastatur (én linje per ReadLine) — rediger værdierne og kør igen:
// gf2-input: 2
// gf2-input: 1
// gf2-input: NovaTech ApS
// gf2-input: C# praktikant
// gf2-input: Ansøgt
// gf2-input: 2
// gf2-input: 3

Console.WriteLine("================================");
Console.WriteLine("     JOBSØGNING-TRACKER (konsol)");
Console.WriteLine("================================");

while (true)
{
    Console.WriteLine();
    Console.WriteLine("1 = Tilføj stilling");
    Console.WriteLine("2 = Vis alle stillinger");
    Console.WriteLine("3 = Afslut");
    Console.Write("Dit valg: ");
    var valg = Console.ReadLine()?.Trim();

    if (valg == "3")
    {
        Console.WriteLine();
        Console.WriteLine("Held og lykke med jobsøgningen!");
        break;
    }

    if (valg == "1")
    {
        Console.Write("Firma: ");
        var firma = Console.ReadLine()?.Trim() ?? "";
        Console.Write("Stilling: ");
        var titel = Console.ReadLine()?.Trim() ?? "";
        Console.Write("Status (Ansøgt / Afventer / Afvist): ");
        var status = Console.ReadLine()?.Trim() ?? "Afventer";
        stillinger.Add($"{firma} | {titel} | {status}");
        Console.WriteLine(">> Tilføjet.");
    }
    else if (valg == "2")
    {
        Console.WriteLine();
        Console.WriteLine("--- Dine stillinger ---");
        if (stillinger.Count == 0)
            Console.WriteLine("(ingen endnu — vælg 1 for at tilføje)");
        else
        {
            for (var i = 0; i < stillinger.Count; i++)
                Console.WriteLine($"{i + 1,2}. {stillinger[i]}");
        }
        Console.WriteLine($"--- Total: {stillinger.Count} ---");
    }
    else
    {
        Console.WriteLine("Ukendt valg — prøv 1, 2 eller 3.");
    }
}
