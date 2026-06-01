var stillinger = new List<string>
{
    "Mercantec | GF2-elev | Ansøgt",
    "IT-firma | Junior dev | Afventer"
};
Console.WriteLine("--- Jobsøgning ---");
foreach (var s in stillinger)
    Console.WriteLine(s);
Console.WriteLine($"Total: {stillinger.Count} stillinger");
