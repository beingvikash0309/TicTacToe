using TicTacToe.Api.Models;

namespace TicTacToe.Api.Services;

public static class ComputerPlayer
{
    private static readonly int[] Corners = { 0, 2, 6, 8 };
    private const int Center = 4;

    /// <summary>
    /// Picks the computer's next move (as O) using a fixed priority:
    /// 1. Win if possible.
    /// 2. Block an immediate opponent win.
    /// 3. Take the center.
    /// 4. Take a corner.
    /// 5. Take any remaining cell.
    /// </summary>
    public static int SelectMove(Player?[] board)
    {
        var emptyCells = Enumerable.Range(0, board.Length).Where(i => board[i] is null).ToList();
        if (emptyCells.Count == 0)
        {
            throw new InvalidOperationException("No empty cells available for the computer to play.");
        }

        var winningMove = FindWinningMove(board, Player.O, emptyCells);
        if (winningMove.HasValue)
        {
            return winningMove.Value;
        }

        var blockingMove = FindWinningMove(board, Player.X, emptyCells);
        if (blockingMove.HasValue)
        {
            return blockingMove.Value;
        }

        if (emptyCells.Contains(Center))
        {
            return Center;
        }

        var availableCorner = Corners.FirstOrDefault(c => emptyCells.Contains(c), -1);
        if (availableCorner != -1)
        {
            return availableCorner;
        }

        return emptyCells[0];
    }

    private static int? FindWinningMove(Player?[] board, Player player, List<int> emptyCells)
    {
        foreach (var cell in emptyCells)
        {
            var candidate = (Player?[])board.Clone();
            candidate[cell] = player;
            if (WinChecker.FindWinningLine(candidate, player) is not null)
            {
                return cell;
            }
        }

        return null;
    }
}
