namespace GF2Learn.Web.Client.Services;

/// <summary>Streams console output from interactive playground runs to the terminal UI.</summary>
public static class PlaygroundStdoutBridge
{
    public static event Action<string>? Write;
    public static event Action<string>? WriteLine;

    public static void RaiseWrite(string text) => Write?.Invoke(text);

    public static void RaiseWriteLine(string text) => WriteLine?.Invoke(text);

    public static void Reset()
    {
    }
}
