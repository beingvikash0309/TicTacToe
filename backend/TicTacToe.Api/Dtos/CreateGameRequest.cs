using TicTacToe.Api.Models;

namespace TicTacToe.Api.Dtos;

public class CreateGameRequest
{
    public GameMode Mode { get; set; } = GameMode.TwoPlayer;
}
