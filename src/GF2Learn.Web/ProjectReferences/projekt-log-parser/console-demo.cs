var lines = new[]
{
    "[INFO] Server startet",
    "[WARN] Disk 80%",
    "[ERROR] Login fejlede",
    "[INFO] Bruger logget ind",
    "[ERROR] Timeout"
};
int info = 0, warn = 0, error = 0;
foreach (var line in lines)
{
    if (line.Contains("ERROR")) error++;
    else if (line.Contains("WARN")) warn++;
    else if (line.Contains("INFO")) info++;
}
Console.WriteLine($"INFO: {info}, WARN: {warn}, ERROR: {error}");
