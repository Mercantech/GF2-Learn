class LogRapport
{
    public int Info { get; set; }
    public int Warn { get; set; }
    public int Error { get; set; }
    public List<string> FejlLinjer { get; } = new();
}

Console.WriteLine("=====================================");
Console.WriteLine("   LOG-PARSER — reference (konsol)");
Console.WriteLine("=====================================");
Console.WriteLine("Læs eksempel-logfiler (txt, json, csv) — som jeres aflevering.");
Console.WriteLine("I Visual Studio bruger I File.ReadAllText — her simuleres filindhold.");

while (true)
{
    Console.WriteLine();
    Console.WriteLine("--- Hovedmenu ---");
    Console.WriteLine("1 = Analysér app.log.txt");
    Console.WriteLine("2 = Analysér app.log.json");
    Console.WriteLine("3 = Analysér app.log.csv");
    Console.WriteLine("4 = Angiv filnavn manuelt");
    Console.WriteLine("5 = Vis tilgængelige filer");
    Console.WriteLine("6 = Afslut");
    Console.Write("Dit valg: ");
    var valg = Console.ReadLine()?.Trim();

    if (valg == "6")
    {
        Console.WriteLine();
        Console.WriteLine("Hent logfilerne fra GF2 Learn og medlevér dem i jeres GitHub-repo.");
        break;
    }

    if (valg == "5")
    {
        Console.WriteLine();
        Console.WriteLine("--- Eksempelfiler (download fra projektsiden) ---");
        Console.WriteLine("  app.log.txt  — tekst med [INFO]/[WARN]/[ERROR]");
        Console.WriteLine("  app.log.json — JSON-array med level, timestamp, message");
        Console.WriteLine("  app.log.csv  — CSV med kolonner level,timestamp,message");
        continue;
    }

    string? filnavn = valg switch
    {
        "1" => "app.log.txt",
        "2" => "app.log.json",
        "3" => "app.log.csv",
        _ => null
    };

    if (filnavn is null)
    {
        if (valg != "4")
        {
            Console.WriteLine("Ukendt valg — prøv 1-6.");
            continue;
        }

        Console.Write("Filnavn (fx app.log.txt): ");
        filnavn = Console.ReadLine()?.Trim();
        if (string.IsNullOrWhiteSpace(filnavn))
        {
            Console.WriteLine("   Tomt filnavn.");
            continue;
        }
    }

    try
    {
        var indhold = LaesEksempelfil(filnavn);
        var rapport = filnavn.EndsWith(".json", StringComparison.OrdinalIgnoreCase)
            ? ParseJson(indhold, filnavn)
            : filnavn.EndsWith(".csv", StringComparison.OrdinalIgnoreCase)
                ? ParseCsv(indhold, filnavn)
                : ParseTxt(indhold.Split('\n'), filnavn);

        VisRapport(rapport, filnavn);
    }
    catch (FileNotFoundException ex)
    {
        Console.WriteLine("Fejl: " + ex.Message);
    }
    catch (Exception ex)
    {
        Console.WriteLine("Fejl ved læsning: " + ex.Message);
    }
}

static string LaesEksempelfil(string filnavn)
{
  var key = filnavn.Trim().ToLowerInvariant();
  if (key == "app.log.txt")
      return """
[INFO] 2026-03-10 08:00:01 Server startet
[WARN] 2026-03-10 08:15:22 Disk 80% fuld
[ERROR] 2026-03-10 09:02:11 Login fejlede for bruger elev1
[INFO] 2026-03-10 09:05:00 Bruger logget ind
[ERROR] 2026-03-10 10:30:44 Timeout mod database
[WARN] 2026-03-10 11:00:00 Cache næsten fuld
[INFO] 2026-03-10 12:00:00 Backup fuldført
[ERROR] 2026-03-10 14:22:18 Database forbindelse afbrudt
""";

  if (key == "app.log.json")
      return """
[
  { "level": "INFO", "timestamp": "2026-03-10T08:00:01", "message": "Server startet" },
  { "level": "WARN", "timestamp": "2026-03-10T08:15:22", "message": "Disk 80% fuld" },
  { "level": "ERROR", "timestamp": "2026-03-10T09:02:11", "message": "Login fejlede for bruger elev1" },
  { "level": "INFO", "timestamp": "2026-03-10T09:05:00", "message": "Bruger logget ind" },
  { "level": "ERROR", "timestamp": "2026-03-10T10:30:44", "message": "Timeout mod database" },
  { "level": "WARN", "timestamp": "2026-03-10T11:00:00", "message": "Cache næsten fuld" },
  { "level": "INFO", "timestamp": "2026-03-10T12:00:00", "message": "Backup fuldført" },
  { "level": "ERROR", "timestamp": "2026-03-10T14:22:18", "message": "Database forbindelse afbrudt" }
]
""";

  if (key == "app.log.csv")
      return """
level,timestamp,message
INFO,2026-03-10 08:00:01,Server startet
WARN,2026-03-10 08:15:22,Disk 80% fuld
ERROR,2026-03-10 09:02:11,Login fejlede for bruger elev1
INFO,2026-03-10 09:05:00,Bruger logget ind
ERROR,2026-03-10 10:30:44,Timeout mod database
WARN,2026-03-10 11:00:00,Cache næsten fuld
INFO,2026-03-10 12:00:00,Backup fuldført
ERROR,2026-03-10 14:22:18,Database forbindelse afbrudt
""";

  throw new FileNotFoundException("Filen findes ikke: " + filnavn + " (kun app.log.txt, .json og .csv i denne demo).");
}

static LogRapport ParseTxt(string[] lines, string filnavn)
{
    var rapport = new LogRapport();
    foreach (var raw in lines)
    {
        var line = raw.Trim();
        if (line.Length == 0) continue;

        if (line.Contains("[ERROR]")) { rapport.Error++; rapport.FejlLinjer.Add(line); }
        else if (line.Contains("[WARN]")) rapport.Warn++;
        else if (line.Contains("[INFO]")) rapport.Info++;
    }
    return rapport;
}

static LogRapport ParseJson(string json, string filnavn)
{
    var rapport = new LogRapport();
    foreach (var line in json.Replace("\r\n", "\n").Split('\n'))
    {
        var trimmed = line.Trim();
        if (trimmed.Contains("\"level\": \"ERROR\"") || trimmed.Contains("\"level\":\"ERROR\""))
        {
            rapport.Error++;
            rapport.FejlLinjer.Add(trimmed);
        }
        else if (trimmed.Contains("\"level\": \"WARN\"") || trimmed.Contains("\"level\":\"WARN\""))
            rapport.Warn++;
        else if (trimmed.Contains("\"level\": \"INFO\"") || trimmed.Contains("\"level\":\"INFO\""))
            rapport.Info++;
    }
    return rapport;
}

static LogRapport ParseCsv(string csv, string filnavn)
{
    var rapport = new LogRapport();
    var lines = csv.Replace("\r\n", "\n").Split('\n');
    for (var i = 1; i < lines.Length; i++)
    {
        var line = lines[i].Trim();
        if (line.Length == 0) continue;
        var level = line.Split(',')[0].Trim();

        if (level == "ERROR") { rapport.Error++; rapport.FejlLinjer.Add(line); }
        else if (level == "WARN") rapport.Warn++;
        else if (level == "INFO") rapport.Info++;
    }
    return rapport;
}

static void VisRapport(LogRapport rapport, string filnavn)
{
    Console.WriteLine();
    Console.WriteLine("--- Rapport for " + filnavn + " ---");
    Console.WriteLine("INFO:  " + rapport.Info);
    Console.WriteLine("WARN:  " + rapport.Warn);
    Console.WriteLine("ERROR: " + rapport.Error);

    if (rapport.FejlLinjer.Count > 0)
    {
        Console.WriteLine();
        Console.WriteLine("Seneste fejl:");
        var start = rapport.FejlLinjer.Count > 5 ? rapport.FejlLinjer.Count - 5 : 0;
        for (var i = start; i < rapport.FejlLinjer.Count; i++)
            Console.WriteLine("  " + rapport.FejlLinjer[i]);
    }
}
