// gf2-input: 50
// gf2-input: 75
// gf2-input: 62
var hemmeligt = 62;
var forsøg = 0;
Console.WriteLine("Gæt et tal (1-100):");
while (true)
{
    var input = Console.ReadLine();
    if (!int.TryParse(input, out var gæt)) { Console.WriteLine("Skriv et heltal."); continue; }
    forsøg++;
    if (gæt == hemmeligt) { Console.WriteLine($"Korrekt på {forsøg} forsøg!"); break; }
    Console.WriteLine(gæt < hemmeligt ? "Højere" : "Lavere");
}
