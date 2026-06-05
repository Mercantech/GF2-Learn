using System.Collections.Concurrent;

namespace GF2Learn.Web.Client.Services;

/// <summary>Coordinates interactive console input between the playground terminal UI and running code.</summary>
public static class PlaygroundStdinBridge
{
    private static readonly ConcurrentQueue<string> PreQueue = new();

    public static event Action<TaskCompletionSource<string?>>? InputRequested;

    public static void Reset()
    {
        while (PreQueue.TryDequeue(out _))
        {
        }
    }

    public static void Enqueue(string line) => PreQueue.Enqueue(line);

    public static string? ReadLineSync()
    {
        if (PreQueue.TryDequeue(out var queued))
            return queued;

        var tcs = new TaskCompletionSource<string?>(TaskCreationOptions.RunContinuationsAsynchronously);
        InputRequested?.Invoke(tcs);
        return tcs.Task.GetAwaiter().GetResult();
    }
}
