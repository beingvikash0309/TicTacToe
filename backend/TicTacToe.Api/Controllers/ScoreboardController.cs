using Microsoft.AspNetCore.Mvc;
using TicTacToe.Api.Dtos;
using TicTacToe.Api.Services;

namespace TicTacToe.Api.Controllers;

[ApiController]
[Route("api/scoreboard")]
public class ScoreboardController : ControllerBase
{
    private readonly IGameService _gameService;

    public ScoreboardController(IGameService gameService)
    {
        _gameService = gameService;
    }

    // GET /api/scoreboard - get the session-level scoreboard.
    [HttpGet]
    public ActionResult<ScoreboardResponse> GetScoreboard()
    {
        var scoreboard = _gameService.GetScoreboard();
        return Ok(new ScoreboardResponse { XWins = scoreboard.XWins, OWins = scoreboard.OWins, Draws = scoreboard.Draws });
    }

    // POST /api/scoreboard/reset - reset X wins, O wins, and draws to zero.
    [HttpPost("reset")]
    public ActionResult<ScoreboardResponse> ResetScoreboard()
    {
        var scoreboard = _gameService.ResetScoreboard();
        return Ok(new ScoreboardResponse { XWins = scoreboard.XWins, OWins = scoreboard.OWins, Draws = scoreboard.Draws });
    }
}
