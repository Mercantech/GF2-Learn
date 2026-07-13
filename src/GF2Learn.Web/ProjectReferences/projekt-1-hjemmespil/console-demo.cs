// gf2-setup: var random = new Random(7);
// gf2-input: valg: 1
// gf2-input: gæt: 50
// gf2-input: gæt: 75
// gf2-input: gæt: 62
// gf2-input: valg: 2
// gf2-input: rpsValg: sten
// gf2-input: igen: j
// gf2-input: rpsValg: papir
// gf2-input: igen: n
// gf2-input: valg: 3
// gf2-input: felt: 5
// gf2-input: felt: 1
// gf2-input: felt: 9
// gf2-input: felt: 3
// gf2-input: valg: 4

Console.WriteLine("=====================================");
Console.WriteLine("   TIDSFORDRIVSSPIL — reference (konsol)");
Console.WriteLine("=====================================");
Console.WriteLine("Tre små spil i én menu — som jeres aflevering kan.");

while (true)
{
    Console.WriteLine();
    Console.WriteLine("--- Hovedmenu ---");
    Console.WriteLine("1 = Gæt et tal (1-100)");
    Console.WriteLine("2 = Sten, saks, papir");
    Console.WriteLine("3 = Tic-Tac-Toe (2 spillere)");
    Console.WriteLine("4 = Afslut");
    Console.Write("Dit valg: ");
    var valg = Console.ReadLine()?.Trim();

    if (valg == "4")
    {
        Console.WriteLine();
        Console.WriteLine("Tak for spil — god pause!");
        break;
    }

    if (valg == "1")
    {
        var hemmeligt = random.Next(1, 101);
        var forsøg = 0;
        var historik = new List<int>();
        Console.WriteLine();
        Console.WriteLine(">> Gæt et tal mellem 1 og 100");
        while (true)
        {
            Console.Write("Dit gæt: ");
            var input = Console.ReadLine();
            if (!int.TryParse(input, out var gæt) || gæt < 1 || gæt > 100)
            {
                Console.WriteLine("   Skriv et heltal mellem 1 og 100.");
                continue;
            }

            forsøg++;
            historik.Add(gæt);

            if (gæt == hemmeligt)
            {
                Console.WriteLine("   Korrekt på " + forsøg + " forsøg!");
                Console.WriteLine("   Dine gæt: " + string.Join(", ", historik));
                break;
            }

            Console.WriteLine(gæt < hemmeligt ? "   Højere..." : "   Lavere...");
        }
    }
    else if (valg == "2")
    {
        var valgMuligheder = new[] { "sten", "saks", "papir" };
        var sejr = 0;
        var nederlag = 0;

        while (true)
        {
            Console.WriteLine();
            Console.Write("Dit valg (sten/saks/papir): ");
            var spiller = Console.ReadLine()?.Trim().ToLowerInvariant();
            if (spiller is not ("sten" or "saks" or "papir"))
            {
                Console.WriteLine("   Ugyldigt — skriv sten, saks eller papir.");
                continue;
            }

            var pc = valgMuligheder[random.Next(3)];
            Console.WriteLine("   PC valgte: " + pc);

            var udfald = AfgørRps(spiller, pc);
            if (udfald == "sejr") { sejr++; Console.WriteLine("   Du vandt runden!"); }
            else if (udfald == "nederlag") { nederlag++; Console.WriteLine("   PC vandt runden."); }
            else Console.WriteLine("   Uafgjort.");

            Console.WriteLine("   Stilling: " + sejr + " - " + nederlag);
            Console.Write("Spil igen? (j/n): ");
            var igen = Console.ReadLine()?.Trim().ToLowerInvariant();
            if (igen != "j" && igen != "ja") break;
        }
    }
    else if (valg == "3")
    {
        var bræt = new string[9];
        for (var i = 0; i < 9; i++) bræt[i] = " ";
        var tur = "X";

        Console.WriteLine();
        Console.WriteLine(">> Tic-Tac-Toe — skriv felt 1-9 (tastatur)");
        Console.WriteLine("   1|2|3");
        Console.WriteLine("   -+-+-");
        Console.WriteLine("   4|5|6");
        Console.WriteLine("   -+-+-");
        Console.WriteLine("   7|8|9");

        while (true)
        {
            VisBræt(bræt);
            Console.WriteLine("Tur: " + tur);
            Console.Write("Vælg felt (1-9): ");
            var input = Console.ReadLine();
            if (!int.TryParse(input, out var felt) || felt < 1 || felt > 9 || bræt[felt - 1] != " ")
            {
                Console.WriteLine("   Ugyldigt felt — prøv igen.");
                continue;
            }

            bræt[felt - 1] = tur;
            if (HarVinder(bræt, tur))
            {
                VisBræt(bræt);
                Console.WriteLine("   " + tur + " vandt!");
                break;
            }

            if (bræt.All(c => c != " "))
            {
                VisBræt(bræt);
                Console.WriteLine("   Uafgjort.");
                break;
            }

            tur = tur == "X" ? "O" : "X";
        }
    }
    else
    {
        Console.WriteLine("Ukendt valg — prøv 1-4.");
    }
}

static string AfgørRps(string spiller, string pc)
{
    if (spiller == pc) return "uafgjort";
    if ((spiller, pc) is ("sten", "saks") or ("saks", "papir") or ("papir", "sten"))
        return "sejr";
    return "nederlag";
}

static void VisBræt(string[] bræt)
{
    Console.WriteLine();
    Console.WriteLine(" " + bræt[0] + "|" + bræt[1] + "|" + bræt[2]);
    Console.WriteLine(" -+-+-");
    Console.WriteLine(" " + bræt[3] + "|" + bræt[4] + "|" + bræt[5]);
    Console.WriteLine(" -+-+-");
    Console.WriteLine(" " + bræt[6] + "|" + bræt[7] + "|" + bræt[8]);
}

static bool HarVinder(string[] b, string s) =>
    (b[0] == s && b[1] == s && b[2] == s) ||
    (b[3] == s && b[4] == s && b[5] == s) ||
    (b[6] == s && b[7] == s && b[8] == s) ||
    (b[0] == s && b[3] == s && b[6] == s) ||
    (b[1] == s && b[4] == s && b[7] == s) ||
    (b[2] == s && b[5] == s && b[8] == s) ||
    (b[0] == s && b[4] == s && b[8] == s) ||
    (b[2] == s && b[4] == s && b[6] == s);
