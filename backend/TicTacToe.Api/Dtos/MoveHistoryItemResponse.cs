namespace TicTacToe.Api.Dtos;

public class MoveHistoryItemResponse
{
    public int MoveNumber { get; set; }
    public string Player { get; set; } = string.Empty;
    public int Row { get; set; }
    public int Col { get; set; }
    public string Position { get; set; } = string.Empty;
}
