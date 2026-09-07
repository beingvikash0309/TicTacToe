using Microsoft.AspNetCore.Mvc;
using TicTacToe.Api.Dtos;
using TicTacToe.Api.Services;

namespace TicTacToe.Api.Controllers;

[ApiController]
[Route("api/games")]
public class GamesController : ControllerBase
{
    private readonly IGameService _gameService;

    public GamesController(IGameService gameService)
    {
        _gameService = gameService;
    }

    // POST /api/games - create a new game session.
    [HttpPost]
    public ActionResult<GameStateResponse> CreateGame([FromBody] CreateGameRequest request)
    {
        var game = _gameService.CreateGame(request.Mode);
        var response = GameStateMapper.ToResponse(game, _gameService.GetScoreboard());
        return CreatedAtAction(nameof(GetGame), new { id = game.Id }, response);
    }

    // GET /api/games/{id} - get current game state.
    [HttpGet("{id:guid}")]
    public ActionResult<GameStateResponse> GetGame(Guid id)
    {
        var game = _gameService.GetGame(id);
        if (game is null)
        {
            return NotFound(new { error = "Game not found." });
        }

        return Ok(GameStateMapper.ToResponse(game, _gameService.GetScoreboard()));
    }

    // POST /api/games/{id}/moves - submit a player move.
    [HttpPost("{id:guid}/moves")]
    public ActionResult<GameStateResponse> MakeMove(Guid id, [FromBody] MoveRequest request)
    {
        var result = _gameService.MakeMove(id, request.Player, request.Row, request.Col);
        if (!result.Success)
        {
            return result.Error == "Game not found."
                ? NotFound(new { error = result.Error })
                : BadRequest(new { error = result.Error });
        }

        return Ok(GameStateMapper.ToResponse(result.Data!, _gameService.GetScoreboard()));
    }

    // POST /api/games/{id}/undo - undo the last move (or move pair in computer mode).
    [HttpPost("{id:guid}/undo")]
    public ActionResult<GameStateResponse> Undo(Guid id)
    {
        var result = _gameService.Undo(id);
        if (!result.Success)
        {
            return result.Error == "Game not found."
                ? NotFound(new { error = result.Error })
                : BadRequest(new { error = result.Error });
        }

        return Ok(GameStateMapper.ToResponse(result.Data!, _gameService.GetScoreboard()));
    }

    // POST /api/games/{id}/reset - reset the current game (scoreboard untouched).
    [HttpPost("{id:guid}/reset")]
    public ActionResult<GameStateResponse> ResetGame(Guid id)
    {
        var result = _gameService.ResetGame(id);
        if (!result.Success)
        {
            return NotFound(new { error = result.Error });
        }

        return Ok(GameStateMapper.ToResponse(result.Data!, _gameService.GetScoreboard()));
    }
}
