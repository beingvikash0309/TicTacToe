using TicTacToe.Api.Models;

namespace TicTacToe.Api.Dtos;

public class MoveRequest
{
    public Player Player { get; set; }
    public int Row { get; set; }
    public int Col { get; set; }
}
