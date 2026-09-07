namespace TicTacToe.Api.Models;

public class GameState
{
    public Guid Id { get; init; } = Guid.NewGuid();

    // 9 cells, index = row * 3 + col. null = empty.
    public Player?[] Board { get; init; } = new Player?[9];

    public Player CurrentPlayer { get; set; } = Player.X;

    public GameMode Mode { get; init; }

    public GameStatus Status { get; set; } = GameStatus.InProgress;

    public Player? Winner { get; set; }

    public int[]? WinningCells { get; set; }

    public List<Move> MoveHistory { get; } = new();

    // Guards against double-counting the scoreboard for the same completed game.
    public bool ScoreRecorded { get; set; }
}
