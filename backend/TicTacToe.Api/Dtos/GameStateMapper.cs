using TicTacToe.Api.Models;

namespace TicTacToe.Api.Dtos;

public static class GameStateMapper
{
    public static GameStateResponse ToResponse(GameState game, Scoreboard scoreboard)
    {
        return new GameStateResponse
        {
            Id = game.Id,
            Board = game.Board.Select(cell => cell?.ToString()).ToArray(),
            CurrentPlayer = game.CurrentPlayer.ToString(),
            Mode = game.Mode.ToString(),
            Status = game.Status.ToString(),
            Winner = game.Winner?.ToString(),
            WinningCells = game.WinningCells,
            CanUndo = game.Status == GameStatus.InProgress && game.MoveHistory.Count > 0,
            MoveHistory = game.MoveHistory.Select(m => new MoveHistoryItemResponse
            {
                MoveNumber = m.MoveNumber,
                Player = m.Player.ToString(),
                Row = m.Row,
                Col = m.Col,
                Position = $"Row {m.Row + 1}, Column {m.Col + 1}"
            }).ToList(),
            Scoreboard = new ScoreboardResponse
            {
                XWins = scoreboard.XWins,
                OWins = scoreboard.OWins,
                Draws = scoreboard.Draws
            }
        };
    }
}
