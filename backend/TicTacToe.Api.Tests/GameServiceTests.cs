using TicTacToe.Api.Models;
using TicTacToe.Api.Services;
using Xunit;

namespace TicTacToe.Api.Tests;

public class GameServiceTests
{
    private static GameService NewService() => new();

    // ---------- Valid move ----------

    [Fact]
    public void MakeMove_ValidMove_PlacesMarkAndSwitchesTurn()
    {
        var service = NewService();
        var game = service.CreateGame(GameMode.TwoPlayer);

        var result = service.MakeMove(game.Id, Player.X, 0, 0);

        Assert.True(result.Success);
        Assert.Equal(Player.X, result.Data!.Board[0]);
        Assert.Equal(Player.O, result.Data.CurrentPlayer);
        Assert.Single(result.Data.MoveHistory);
    }

    // ---------- Invalid move ----------

    [Theory]
    [InlineData(-1, 0)]
    [InlineData(0, 3)]
    [InlineData(3, 3)]
    public void MakeMove_OutsideBoard_IsRejected(int row, int col)
    {
        var service = NewService();
        var game = service.CreateGame(GameMode.TwoPlayer);

        var result = service.MakeMove(game.Id, Player.X, row, col);

        Assert.False(result.Success);
    }

    [Fact]
    public void MakeMove_OnOccupiedCell_IsRejected()
    {
        var service = NewService();
        var game = service.CreateGame(GameMode.TwoPlayer);
        service.MakeMove(game.Id, Player.X, 0, 0);

        var result = service.MakeMove(game.Id, Player.O, 0, 0);

        Assert.False(result.Success);
        Assert.Equal(Player.X, game.Board[0]);
    }

    [Fact]
    public void MakeMove_ByWrongPlayer_IsRejectedAndDoesNotChangeTurn()
    {
        var service = NewService();
        var game = service.CreateGame(GameMode.TwoPlayer);

        var result = service.MakeMove(game.Id, Player.O, 0, 0);

        Assert.False(result.Success);
        Assert.Equal(Player.X, game.CurrentPlayer);
        Assert.Empty(game.MoveHistory);
    }

    [Fact]
    public void MakeMove_AfterGameCompletion_IsRejected()
    {
        var service = NewService();
        var game = service.CreateGame(GameMode.TwoPlayer);
        WinAsX(service, game.Id);
        Assert.Equal(GameStatus.Won, game.Status);

        var result = service.MakeMove(game.Id, Player.O, 2, 2);

        Assert.False(result.Success);
    }

    // ---------- Turn switching ----------

    [Fact]
    public void MakeMove_ValidMoves_AlternateTurns()
    {
        var service = NewService();
        var game = service.CreateGame(GameMode.TwoPlayer);

        service.MakeMove(game.Id, Player.X, 0, 0);
        Assert.Equal(Player.O, game.CurrentPlayer);

        service.MakeMove(game.Id, Player.O, 1, 0);
        Assert.Equal(Player.X, game.CurrentPlayer);
    }

    // ---------- Win detection ----------

    [Fact]
    public void MakeMove_CompletingRow_DetectsWin()
    {
        var service = NewService();
        var game = service.CreateGame(GameMode.TwoPlayer);

        service.MakeMove(game.Id, Player.X, 0, 0);
        service.MakeMove(game.Id, Player.O, 1, 0);
        service.MakeMove(game.Id, Player.X, 0, 1);
        service.MakeMove(game.Id, Player.O, 1, 1);
        var result = service.MakeMove(game.Id, Player.X, 0, 2);

        Assert.True(result.Success);
        Assert.Equal(GameStatus.Won, game.Status);
        Assert.Equal(Player.X, game.Winner);
        Assert.Equal(new[] { 0, 1, 2 }, game.WinningCells);
    }

    [Fact]
    public void MakeMove_CompletingColumn_DetectsWin()
    {
        var service = NewService();
        var game = service.CreateGame(GameMode.TwoPlayer);

        service.MakeMove(game.Id, Player.X, 0, 0);
        service.MakeMove(game.Id, Player.O, 0, 1);
        service.MakeMove(game.Id, Player.X, 1, 0);
        service.MakeMove(game.Id, Player.O, 1, 1);
        var result = service.MakeMove(game.Id, Player.X, 2, 0);

        Assert.True(result.Success);
        Assert.Equal(GameStatus.Won, game.Status);
        Assert.Equal(Player.X, game.Winner);
        Assert.Equal(new[] { 0, 3, 6 }, game.WinningCells);
    }

    [Fact]
    public void MakeMove_CompletingDiagonal_DetectsWin()
    {
        var service = NewService();
        var game = service.CreateGame(GameMode.TwoPlayer);

        service.MakeMove(game.Id, Player.X, 0, 0);
        service.MakeMove(game.Id, Player.O, 0, 1);
        service.MakeMove(game.Id, Player.X, 1, 1);
        service.MakeMove(game.Id, Player.O, 0, 2);
        var result = service.MakeMove(game.Id, Player.X, 2, 2);

        Assert.True(result.Success);
        Assert.Equal(GameStatus.Won, game.Status);
        Assert.Equal(new[] { 0, 4, 8 }, game.WinningCells);
    }

    // ---------- Draw ----------

    [Fact]
    public void MakeMove_BoardFullNoWinner_IsDraw()
    {
        var service = NewService();
        var game = service.CreateGame(GameMode.TwoPlayer);

        // X O X
        // X O O
        // O X X
        service.MakeMove(game.Id, Player.X, 0, 0);
        service.MakeMove(game.Id, Player.O, 0, 1);
        service.MakeMove(game.Id, Player.X, 0, 2);
        service.MakeMove(game.Id, Player.O, 1, 1);
        service.MakeMove(game.Id, Player.X, 1, 0);
        service.MakeMove(game.Id, Player.O, 1, 2);
        service.MakeMove(game.Id, Player.X, 2, 1);
        service.MakeMove(game.Id, Player.O, 2, 0);
        var result = service.MakeMove(game.Id, Player.X, 2, 2);

        Assert.True(result.Success);
        Assert.Equal(GameStatus.Draw, game.Status);
        Assert.Null(game.Winner);
    }

    // ---------- Reset game ----------

    [Fact]
    public void ResetGame_ClearsBoardHistoryAndStatus_KeepsScoreboard()
    {
        var service = NewService();
        var game = service.CreateGame(GameMode.TwoPlayer);
        WinAsX(service, game.Id);
        var scoreboardBefore = service.GetScoreboard();

        var result = service.ResetGame(game.Id);

        Assert.True(result.Success);
        var resetGame = result.Data!;
        Assert.All(resetGame.Board, cell => Assert.Null(cell));
        Assert.Empty(resetGame.MoveHistory);
        Assert.Equal(GameStatus.InProgress, resetGame.Status);
        Assert.Equal(Player.X, resetGame.CurrentPlayer);
        Assert.Equal(game.Id, resetGame.Id);

        var scoreboardAfter = service.GetScoreboard();
        Assert.Equal(scoreboardBefore.XWins, scoreboardAfter.XWins);
        Assert.Equal(scoreboardBefore.OWins, scoreboardAfter.OWins);
        Assert.Equal(scoreboardBefore.Draws, scoreboardAfter.Draws);
    }

    // ---------- Undo: two-player mode ----------

    [Fact]
    public void Undo_TwoPlayerMode_RemovesOnlyLastMove()
    {
        var service = NewService();
        var game = service.CreateGame(GameMode.TwoPlayer);
        service.MakeMove(game.Id, Player.X, 0, 0);
        service.MakeMove(game.Id, Player.O, 1, 1);

        var result = service.Undo(game.Id);

        Assert.True(result.Success);
        var updated = result.Data!;
        Assert.Single(updated.MoveHistory);
        Assert.Equal(Player.X, updated.Board[0]);
        Assert.Null(updated.Board[4]);
        Assert.Equal(Player.O, updated.CurrentPlayer);
    }

    [Fact]
    public void Undo_WithNoMoves_IsRejected()
    {
        var service = NewService();
        var game = service.CreateGame(GameMode.TwoPlayer);

        var result = service.Undo(game.Id);

        Assert.False(result.Success);
    }

    [Fact]
    public void Undo_AfterGameCompletion_IsRejected()
    {
        var service = NewService();
        var game = service.CreateGame(GameMode.TwoPlayer);
        WinAsX(service, game.Id);

        var result = service.Undo(game.Id);

        Assert.False(result.Success);
    }

    // ---------- Undo: computer mode ----------

    [Fact]
    public void Undo_ComputerMode_RemovesComputerAndPrecedingHumanMoveTogether()
    {
        var service = NewService();
        var game = service.CreateGame(GameMode.VsComputer);
        // X plays, computer (O) auto-responds.
        service.MakeMove(game.Id, Player.X, 0, 0);
        Assert.Equal(2, game.MoveHistory.Count);

        var result = service.Undo(game.Id);

        Assert.True(result.Success);
        var updated = result.Data!;
        Assert.Empty(updated.MoveHistory);
        Assert.All(updated.Board, cell => Assert.Null(cell));
        Assert.Equal(Player.X, updated.CurrentPlayer);
    }

    // ---------- Scoreboard ----------

    [Fact]
    public void Scoreboard_UpdatesOnceWhenGameIsWon()
    {
        var service = NewService();
        var game = service.CreateGame(GameMode.TwoPlayer);

        WinAsX(service, game.Id);

        var scoreboard = service.GetScoreboard();
        Assert.Equal(1, scoreboard.XWins);
        Assert.Equal(0, scoreboard.OWins);
        Assert.Equal(0, scoreboard.Draws);
    }

    [Fact]
    public void Scoreboard_ResetGame_DoesNotDoubleCountOrLoseTrack()
    {
        var service = NewService();
        var game = service.CreateGame(GameMode.TwoPlayer);
        WinAsX(service, game.Id);
        service.ResetGame(game.Id);
        WinAsX(service, game.Id);

        var scoreboard = service.GetScoreboard();
        Assert.Equal(2, scoreboard.XWins);
    }

    [Fact]
    public void ResetScoreboard_ZeroesAllCounters()
    {
        var service = NewService();
        var game = service.CreateGame(GameMode.TwoPlayer);
        WinAsX(service, game.Id);

        var result = service.ResetScoreboard();

        Assert.Equal(0, result.XWins);
        Assert.Equal(0, result.OWins);
        Assert.Equal(0, result.Draws);
    }

    // ---------- Computer move selection ----------

    [Fact]
    public void ComputerPlayer_TakesWinningMoveWhenAvailable()
    {
        // O has two in a row (top row) and can win at (0,2).
        var board = new Player?[9];
        board[0] = Player.O;
        board[1] = Player.O;
        board[3] = Player.X;
        board[4] = Player.X;

        var move = ComputerPlayer.SelectMove(board);

        Assert.Equal(2, move);
    }

    [Fact]
    public void ComputerPlayer_BlocksOpponentWinningMove()
    {
        // X has two in a column (left column) and threatens to win at (2,0) = index 6.
        var board = new Player?[9];
        board[0] = Player.X;
        board[3] = Player.X;
        board[1] = Player.O;

        var move = ComputerPlayer.SelectMove(board);

        Assert.Equal(6, move);
    }

    [Fact]
    public void ComputerPlayer_TakesCenterWhenNoImmediateWinOrBlock()
    {
        var board = new Player?[9];
        board[0] = Player.X;

        var move = ComputerPlayer.SelectMove(board);

        Assert.Equal(4, move);
    }

    [Fact]
    public void MakeMove_VsComputerMode_ComputerRespondsAutomatically()
    {
        var service = NewService();
        var game = service.CreateGame(GameMode.VsComputer);

        var result = service.MakeMove(game.Id, Player.X, 0, 0);

        Assert.True(result.Success);
        Assert.Equal(2, result.Data!.MoveHistory.Count);
        Assert.True(result.Data.MoveHistory[1].WasComputerMove);
        Assert.Equal(Player.X, result.Data.CurrentPlayer);
    }

    [Fact]
    public void MakeMove_VsComputerMode_RejectsMoveSubmittedAsO()
    {
        var service = NewService();
        var game = service.CreateGame(GameMode.VsComputer);

        var result = service.MakeMove(game.Id, Player.O, 0, 0);

        Assert.False(result.Success);
    }

    private static void WinAsX(GameService service, Guid gameId)
    {
        service.MakeMove(gameId, Player.X, 0, 0);
        service.MakeMove(gameId, Player.O, 1, 0);
        service.MakeMove(gameId, Player.X, 0, 1);
        service.MakeMove(gameId, Player.O, 1, 1);
        service.MakeMove(gameId, Player.X, 0, 2);
    }
}
