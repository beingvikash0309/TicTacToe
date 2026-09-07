using TicTacToe.Api.Models;

namespace TicTacToe.Api.Services;

public interface IGameService
{
    GameState CreateGame(GameMode mode);
    GameState? GetGame(Guid id);
    ServiceResult<GameState> MakeMove(Guid id, Player player, int row, int col);
    ServiceResult<GameState> Undo(Guid id);
    ServiceResult<GameState> ResetGame(Guid id);
    Scoreboard GetScoreboard();
    Scoreboard ResetScoreboard();
}
