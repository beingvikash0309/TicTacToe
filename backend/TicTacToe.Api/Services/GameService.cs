using System.Collections.Concurrent;
using TicTacToe.Api.Models;

namespace TicTacToe.Api.Services;

/// <summary>
/// Owns all game session state and the session-level scoreboard.
/// In-memory only: state is lost on process restart, which is acceptable for this exercise.
/// </summary>
public class GameService : IGameService
{
    private readonly ConcurrentDictionary<Guid, GameState> _games = new();
    private readonly object _scoreboardLock = new();
    private readonly Scoreboard _scoreboard = new();

    public GameState CreateGame(GameMode mode)
    {
        var game = new GameState { Mode = mode };
        _games[game.Id] = game;
        return game;
    }

    public GameState? GetGame(Guid id)
    {
        return _games.TryGetValue(id, out var game) ? game : null;
    }

    public ServiceResult<GameState> MakeMove(Guid id, Player player, int row, int col)
    {
        if (!_games.TryGetValue(id, out var game))
        {
            return ServiceResult<GameState>.Fail("Game not found.");
        }

        if (game.Status != GameStatus.InProgress)
        {
            return ServiceResult<GameState>.Fail("Move rejected: the game has already been completed.");
        }

        if (row is < 0 or > 2 || col is < 0 or > 2)
        {
            return ServiceResult<GameState>.Fail("Move rejected: the cell is outside the board.");
        }

        if (game.Mode == GameMode.VsComputer && player != Player.X)
        {
            return ServiceResult<GameState>.Fail("Move rejected: the computer controls O in this mode.");
        }

        if (player != game.CurrentPlayer)
        {
            return ServiceResult<GameState>.Fail("Move rejected: it is not this player's turn.");
        }

        var cellIndex = row * 3 + col;
        if (game.Board[cellIndex] is not null)
        {
            return ServiceResult<GameState>.Fail("Move rejected: the cell is already occupied.");
        }

        ApplyMove(game, player, row, col, wasComputerMove: false);

        if (game.Status == GameStatus.InProgress && game.Mode == GameMode.VsComputer && game.CurrentPlayer == Player.O)
        {
            var computerCell = ComputerPlayer.SelectMove(game.Board);
            ApplyMove(game, Player.O, computerCell / 3, computerCell % 3, wasComputerMove: true);
        }

        return ServiceResult<GameState>.Ok(game);
    }

    public ServiceResult<GameState> Undo(Guid id)
    {
        if (!_games.TryGetValue(id, out var game))
        {
            return ServiceResult<GameState>.Fail("Game not found.");
        }

        if (game.MoveHistory.Count == 0)
        {
            return ServiceResult<GameState>.Fail("Undo rejected: there are no moves to undo.");
        }

        // Option A (see README "Scoreboard and Undo"): undo is disabled once a game is complete,
        // so the scoreboard never needs to be reversed.
        if (game.Status != GameStatus.InProgress)
        {
            return ServiceResult<GameState>.Fail("Undo rejected: the game is already completed.");
        }

        // In computer mode, moves are removed in human/computer pairs so control always
        // returns to the human player (X).
        var movesToRemove = game.Mode == GameMode.VsComputer
            ? Math.Min(2, game.MoveHistory.Count)
            : 1;

        for (var i = 0; i < movesToRemove; i++)
        {
            game.MoveHistory.RemoveAt(game.MoveHistory.Count - 1);
        }

        RebuildFromHistory(game);

        return ServiceResult<GameState>.Ok(game);
    }

    public ServiceResult<GameState> ResetGame(Guid id)
    {
        if (!_games.TryGetValue(id, out var existing))
        {
            return ServiceResult<GameState>.Fail("Game not found.");
        }

        // Reset clears the board, history, and status but keeps the scoreboard untouched.
        var fresh = new GameState { Id = existing.Id, Mode = existing.Mode };
        _games[id] = fresh;
        return ServiceResult<GameState>.Ok(fresh);
    }

    public Scoreboard GetScoreboard()
    {
        lock (_scoreboardLock)
        {
            return new Scoreboard { XWins = _scoreboard.XWins, OWins = _scoreboard.OWins, Draws = _scoreboard.Draws };
        }
    }

    public Scoreboard ResetScoreboard()
    {
        lock (_scoreboardLock)
        {
            _scoreboard.XWins = 0;
            _scoreboard.OWins = 0;
            _scoreboard.Draws = 0;
            return new Scoreboard { XWins = 0, OWins = 0, Draws = 0 };
        }
    }

    private void ApplyMove(GameState game, Player player, int row, int col, bool wasComputerMove)
    {
        var cellIndex = row * 3 + col;
        game.Board[cellIndex] = player;
        game.MoveHistory.Add(new Move
        {
            MoveNumber = game.MoveHistory.Count + 1,
            Player = player,
            Row = row,
            Col = col,
            WasComputerMove = wasComputerMove
        });

        var winningLine = WinChecker.FindWinningLine(game.Board, player);
        if (winningLine is not null)
        {
            game.Status = GameStatus.Won;
            game.Winner = player;
            game.WinningCells = winningLine;
            RecordScore(game, player);
            return;
        }

        if (WinChecker.IsBoardFull(game.Board))
        {
            game.Status = GameStatus.Draw;
            RecordScore(game, null);
            return;
        }

        game.CurrentPlayer = player == Player.X ? Player.O : Player.X;
    }

    private void RecordScore(GameState game, Player? winner)
    {
        // A given game's outcome is only ever counted once toward the scoreboard.
        if (game.ScoreRecorded)
        {
            return;
        }

        lock (_scoreboardLock)
        {
            if (winner == Player.X)
            {
                _scoreboard.XWins++;
            }
            else if (winner == Player.O)
            {
                _scoreboard.OWins++;
            }
            else
            {
                _scoreboard.Draws++;
            }
        }

        game.ScoreRecorded = true;
    }

    private static void RebuildFromHistory(GameState game)
    {
        var board = new Player?[9];
        foreach (var move in game.MoveHistory)
        {
            board[move.CellIndex] = move.Player;
        }

        Array.Copy(board, game.Board, board.Length);

        game.CurrentPlayer = game.MoveHistory.Count == 0
            ? Player.X
            : game.MoveHistory[^1].Player == Player.X ? Player.O : Player.X;

        game.Status = GameStatus.InProgress;
        game.Winner = null;
        game.WinningCells = null;
    }
}
