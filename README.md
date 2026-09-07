# Tic Tac Toe — Angular + .NET

A browser-based Tic Tac Toe game. The frontend is built with Angular and the backend is a .NET
Web API. They talk to each other over a REST API — the backend keeps track of everything (whose
turn it is, the board, wins, move history, scores) and the frontend just displays whatever the
backend tells it.

## Tech Stack

| Layer | Technology |
|---|---|
| Frontend | Angular 20, TypeScript, SCSS |
| Backend | .NET 9, ASP.NET Core Web API |
| Storage | In-memory (no database setup needed) |
| API | REST, JSON |

## How It Works

### Game Modes

- **Two Player** — two people take turns playing X and O on the same screen.
- **Play Against Computer** — you play X, the computer plays O and moves automatically right
  after you.

### Rules

- Click any empty cell to place your mark. Once a cell is filled it can't be clicked again.
- Players alternate turns; the app always shows whose turn it is.
- A move that isn't allowed (occupied cell, wrong player's turn, or a move after the game is
  already over) is rejected and nothing changes.
- The game ends when a player completes a row, column, or diagonal (a win — those 3 cells get
  highlighted), or when all 9 cells are filled with no winner (a draw).

### Move History

Every move is logged with its move number, which player made it, and which cell (e.g.
"Row 1, Column 1").

### Undo

- **Two Player Mode** — Undo removes the single most recent move.
- **Computer Mode** — Undo removes the computer's move *and* your move before it together, so
  it's always your turn again after undoing.
- Undo is disabled once a game is won or drawn, and disabled when there's nothing to undo yet.

### Scoreboard

- Tracks X wins, O wins, and draws for the current session.
- Updates automatically, exactly once, whenever a game finishes.
- **Reset Game** clears the current board/history but keeps the scoreboard as-is.
- **Reset Scoreboard** is a separate button that zeroes the scoreboard.

### Computer Opponent

The computer (O) picks its move in this order:
1. Win immediately if it can.
2. Otherwise, block you if you're about to win.
3. Otherwise, take the center.
4. Otherwise, take a corner.
5. Otherwise, take whatever's left.

## Running the Project

You'll need two things installed first:
- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [Node.js](https://nodejs.org/) (this includes npm)

You'll need **two terminal windows** open at the same time — one for the backend, one for the
frontend. Start the backend first.

### 1. Start the backend

```bash
cd backend/TicTacToe.Api
dotnet run --urls http://localhost:5108
```

Leave this window open and running. You'll see log output ending with something like
`Now listening on: http://localhost:5108`. To double-check it's working, open
`http://localhost:5108/api/scoreboard` in a browser — you should see
`{"xWins":0,"oWins":0,"draws":0}`.

### 2. Start the frontend

Open a **new** terminal window:

```bash
cd frontend
npm install
npm start
```

`npm install` only needs to be run once (it downloads everything the app needs) — after that,
`npm start` is all you need each time.

Once it's running, open your browser to:

```
http://localhost:4200
```

You should see the Tic Tac Toe board. Pick a mode from the dropdown at the top and start playing.

> The app expects the backend to be running at `http://localhost:5108`. If you see an error
> message on the page, make sure step 1 is still running in its own window.

## API Endpoints

All endpoints are under `http://localhost:5108/api`.

| Method | Endpoint | What it does |
|---|---|---|
| POST | `/games` | Start a new game |
| GET | `/games/{id}` | Get the current state of a game |
| POST | `/games/{id}/moves` | Submit a move |
| POST | `/games/{id}/undo` | Undo the last move (or move pair, in Computer Mode) |
| POST | `/games/{id}/reset` | Reset the current game (scoreboard stays untouched) |
| GET | `/scoreboard` | Get the scoreboard |
| POST | `/scoreboard/reset` | Reset the scoreboard to zero |

### Create a game

```
POST /api/games
Content-Type: application/json

{ "mode": "TwoPlayer" }        // or "VsComputer"
```

### Submit a move

```
POST /api/games/{id}/moves
Content-Type: application/json

{ "player": "X", "row": 0, "col": 0 }
```

Rows and columns are `0`, `1`, or `2`.

### Example response

Every endpoint that returns a game (create, get, move, undo, reset) responds with the same shape:

```json
{
  "id": "b3b3d3b0-1234-5678-9abc-def012345678",
  "board": ["X", null, null, null, "O", null, null, null, null],
  "currentPlayer": "X",
  "mode": "TwoPlayer",
  "status": "InProgress",
  "winner": null,
  "winningCells": null,
  "moveHistory": [
    { "moveNumber": 1, "player": "X", "row": 0, "col": 0, "position": "Row 1, Column 1" }
  ],
  "scoreboard": { "xWins": 0, "oWins": 0, "draws": 0 },
  "canUndo": true
}
```

- `status` is one of `"InProgress"`, `"Won"`, or `"Draw"`.
- `winner` and `winningCells` are only filled in once the game is won.
- `canUndo` tells the frontend whether the Undo button should be enabled.

### If something goes wrong

Invalid requests get rejected with an error message and an appropriate status code
(`400` for a bad move, `404` if the game id doesn't exist):

```json
{ "error": "Move rejected: the cell is already occupied." }
```
