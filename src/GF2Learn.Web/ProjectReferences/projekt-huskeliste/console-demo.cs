var opgaver = new List<string> { "Læs pensum", "Øv loops" };
Console.WriteLine("--- Huskeliste ---");
for (var i = 0; i < opgaver.Count; i++)
    Console.WriteLine($"{i + 1}. {opgaver[i]}");
opgaver.Add("Push til GitHub");
Console.WriteLine("Tilføjet: Push til GitHub");
Console.WriteLine($"Antal opgaver: {opgaver.Count}");
