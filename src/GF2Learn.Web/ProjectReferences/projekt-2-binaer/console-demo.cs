Console.WriteLine("=====================================");
Console.WriteLine("   BINÆR OMVANDLER — reference (konsol)");
Console.WriteLine("=====================================");
Console.WriteLine("8-bit oktetter og 4-grupper — uden Convert.");

while (true)
{
    Console.WriteLine();
    Console.WriteLine("--- Hovedmenu ---");
    Console.WriteLine("1 = Binær → decimal (én oktet, 8 cifre)");
    Console.WriteLine("2 = Decimal → binær (én oktet, 0–255)");
    Console.WriteLine("3 = Fuld streng: binær → decimal (4 oktetter)");
    Console.WriteLine("4 = Fuld streng: decimal → binær (4 oktetter)");
    Console.WriteLine("5 = Afslut");
    Console.Write("Dit valg: ");
    var valg = Console.ReadLine()?.Trim();

    if (valg == "5")
    {
        Console.WriteLine();
        Console.WriteLine("Farvel — husk at teste edge cases i README!");
        break;
    }

    if (valg == "1")
    {
        Console.Write("8-bit binær streng: ");
        var binaer = Console.ReadLine()?.Trim() ?? "";
        if (!ErGyldigOktet(binaer))
        {
            Console.WriteLine("   Ugyldigt — skal være præcis 8 tegn med kun 0 og 1.");
            continue;
        }

        var dec = BinaerTilDecimal(binaer);
        Console.WriteLine(">> " + binaer + " (binær) = " + dec + " (decimal)");
    }
    else if (valg == "2")
    {
        Console.Write("Decimal (0–255): ");
        if (!int.TryParse(Console.ReadLine(), out var dec) || dec is < 0 or > 255)
        {
            Console.WriteLine("   Ugyldigt — skriv et heltal mellem 0 og 255.");
            continue;
        }

        var binaer = DecimalTilBinaer(dec);
        Console.WriteLine(">> " + dec + " (decimal) = " + binaer + " (binær)");
    }
    else if (valg == "3")
    {
        Console.Write("4 oktetter (fx 10111011.01001011.10101010.01010101): ");
        var input = Console.ReadLine()?.Trim() ?? "";
        var dele = input.Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (dele.Length != 4)
        {
            Console.WriteLine("   Forvent præcis 4 grupper adskilt af punktum.");
            continue;
        }

        var resultat = new List<string>();
        foreach (var oktet in dele)
        {
            if (!ErGyldigOktet(oktet))
            {
                Console.WriteLine("   Ugyldig oktet: " + oktet);
                resultat.Clear();
                break;
            }

            resultat.Add(BinaerTilDecimal(oktet).ToString());
        }

        if (resultat.Count == 4)
            Console.WriteLine(">> " + input + " = " + string.Join(".", resultat));
    }
    else if (valg == "4")
    {
        Console.Write("4 decimaltal (fx 187.75.170.85): ");
        var input = Console.ReadLine()?.Trim() ?? "";
        var dele = input.Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (dele.Length != 4)
        {
            Console.WriteLine("   Forvent præcis 4 tal adskilt af punktum.");
            continue;
        }

        var resultat = new List<string>();
        foreach (var del in dele)
        {
            if (!int.TryParse(del, out var dec) || dec is < 0 or > 255)
            {
                Console.WriteLine("   Ugyldigt tal: " + del + " (skal være 0–255).");
                resultat.Clear();
                break;
            }

            resultat.Add(DecimalTilBinaer(dec));
        }

        if (resultat.Count == 4)
            Console.WriteLine(">> " + input + " = " + string.Join(".", resultat));
    }
    else
    {
        Console.WriteLine("Ukendt valg — prøv 1-5.");
    }
}

static bool ErGyldigOktet(string binaer)
{
    if (binaer.Length != 8)
        return false;

    foreach (var c in binaer)
    {
        if (c != '0' && c != '1')
            return false;
    }

    return true;
}

static int BinaerTilDecimal(string binaer)
{
    var sum = 0;
    for (var i = 0; i < binaer.Length; i++)
    {
        if (binaer[i] == '1')
            sum += 1 << (binaer.Length - 1 - i);
    }

    return sum;
}

static string DecimalTilBinaer(int dec)
{
    var bits = "";
    var n = dec;
    for (var i = 0; i < 8; i++)
    {
        bits = (n % 2) + bits;
        n /= 2;
    }

    return bits.PadLeft(8, '0');
}
