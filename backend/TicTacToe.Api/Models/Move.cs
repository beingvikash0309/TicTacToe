namespace TicTacToe.Api.Models;

public class Move
{
    public int MoveNumber { get; init; }
    public Player Player { get; init; }
    public int Row { get; init; }
    public int Col { get; init; }
    public int CellIndex => Row * 3 + Col;
    public bool WasComputerMove { get; init; }
}
