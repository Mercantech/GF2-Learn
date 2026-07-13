namespace GF2Learn.Web.Client.Components.ProjectDemos.Hjemmespil;

public sealed class TicTacToeState
{
    public string[] Board { get; } = Enumerable.Repeat(" ", 9).ToArray();
    public string CurrentPlayer { get; private set; } = "X";
    public string? Winner { get; private set; }
    public bool IsDraw { get; private set; }
    public bool IsOver => Winner is not null || IsDraw;

    private static readonly int[][] Lines =
    [
        [0, 1, 2], [3, 4, 5], [6, 7, 8],
        [0, 3, 6], [1, 4, 7], [2, 5, 8],
        [0, 4, 8], [2, 4, 6]
    ];

    public bool TryPlay(int index)
    {
        if (IsOver || index < 0 || index > 8 || Board[index] != " ")
            return false;

        Board[index] = CurrentPlayer;
        Winner = FindWinner();
        if (Winner is not null)
            return true;

        if (Board.All(c => c != " "))
        {
            IsDraw = true;
            return true;
        }

        CurrentPlayer = CurrentPlayer == "X" ? "O" : "X";
        return true;
    }

    public void Reset()
    {
        for (var i = 0; i < 9; i++)
            Board[i] = " ";
        CurrentPlayer = "X";
        Winner = null;
        IsDraw = false;
    }

    private string? FindWinner()
    {
        foreach (var line in Lines)
        {
            var a = Board[line[0]];
            if (a != " " && a == Board[line[1]] && a == Board[line[2]])
                return a;
        }
        return null;
    }
}
