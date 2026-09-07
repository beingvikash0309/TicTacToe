namespace TicTacToe.Api.Dtos;

public class GameStateResponse
{
    public Guid Id { get; set; }
    public string?[] Board { get; set; } = new string?[9];
    public string CurrentPlayer { get; set; } = string.Empty;
    public string Mode { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? Winner { get; set; }
    public int[]? WinningCells { get; set; }
    public List<MoveHistoryItemResponse> MoveHistory { get; set; } = new();
    public ScoreboardResponse Scoreboard { get; set; } = new();
    public bool CanUndo { get; set; }
}
