export type PlayerSymbol = 'X' | 'O';

export type GameMode = 'TwoPlayer' | 'VsComputer';

export type GameStatusValue = 'InProgress' | 'Won' | 'Draw';

export interface MoveHistoryItem {
  moveNumber: number;
  player: PlayerSymbol;
  row: number;
  col: number;
  position: string;
}

export interface ScoreboardDto {
  xWins: number;
  oWins: number;
  draws: number;
}

export interface GameStateDto {
  id: string;
  board: (PlayerSymbol | null)[];
  currentPlayer: PlayerSymbol;
  mode: GameMode;
  status: GameStatusValue;
  winner: PlayerSymbol | null;
  winningCells: number[] | null;
  moveHistory: MoveHistoryItem[];
  scoreboard: ScoreboardDto;
  canUndo: boolean;
}

export interface MoveRequestDto {
  player: PlayerSymbol;
  row: number;
  col: number;
}

export interface ApiErrorResponse {
  error: string;
}
