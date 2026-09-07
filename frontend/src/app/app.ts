import { Component, OnInit, inject, signal } from '@angular/core';
import { GameService } from './core/game';
import { Board } from './components/board/board';
import { MoveHistory } from './components/move-history/move-history';
import { Scoreboard } from './components/scoreboard/scoreboard';
import { GameControls } from './components/game-controls/game-controls';
import { GameMode, GameStateDto } from './models/game.models';

@Component({
  selector: 'app-root',
  imports: [Board, MoveHistory, Scoreboard, GameControls],
  templateUrl: './app.html',
  styleUrl: './app.scss',
})
export class App implements OnInit {
  private readonly gameService = inject(GameService);

  protected readonly game = signal<GameStateDto | null>(null);
  protected readonly busy = signal(false);
  protected readonly errorMessage = signal<string | null>(null);

  ngOnInit(): void {
    this.startNewGame('TwoPlayer');
  }

  protected onModeChange(mode: GameMode): void {
    this.startNewGame(mode);
  }

  protected onCellClick(index: number): void {
    const current = this.game();
    if (!current || current.status !== 'InProgress' || this.busy()) {
      return;
    }

    const row = Math.floor(index / 3);
    const col = index % 3;

    this.busy.set(true);
    this.gameService.makeMove(current.id, { player: current.currentPlayer, row, col }).subscribe({
      next: (updated) => {
        this.game.set(updated);
        this.errorMessage.set(null);
        this.busy.set(false);
      },
      error: (err) => {
        this.errorMessage.set(this.extractError(err));
        this.busy.set(false);
      },
    });
  }

  protected onUndo(): void {
    const current = this.game();
    if (!current || this.busy()) {
      return;
    }

    this.busy.set(true);
    this.gameService.undo(current.id).subscribe({
      next: (updated) => {
        this.game.set(updated);
        this.errorMessage.set(null);
        this.busy.set(false);
      },
      error: (err) => {
        this.errorMessage.set(this.extractError(err));
        this.busy.set(false);
      },
    });
  }

  protected onResetGame(): void {
    const current = this.game();
    if (!current || this.busy()) {
      return;
    }

    this.busy.set(true);
    this.gameService.resetGame(current.id).subscribe({
      next: (updated) => {
        this.game.set(updated);
        this.errorMessage.set(null);
        this.busy.set(false);
      },
      error: (err) => {
        this.errorMessage.set(this.extractError(err));
        this.busy.set(false);
      },
    });
  }

  protected onResetScoreboard(): void {
    this.gameService.resetScoreboard().subscribe({
      next: (scoreboard) => {
        const current = this.game();
        if (current) {
          this.game.set({ ...current, scoreboard });
        }
      },
      error: (err) => this.errorMessage.set(this.extractError(err)),
    });
  }

  protected statusMessage(): string {
    const current = this.game();
    if (!current) {
      return '';
    }

    if (current.status === 'Won') {
      return `Player ${current.winner} wins!`;
    }

    if (current.status === 'Draw') {
      return "It's a draw!";
    }

    return `Player ${current.currentPlayer}'s turn`;
  }

  private startNewGame(mode: GameMode): void {
    this.busy.set(true);
    this.gameService.createGame(mode).subscribe({
      next: (created) => {
        this.game.set(created);
        this.errorMessage.set(null);
        this.busy.set(false);
      },
      error: (err) => {
        this.errorMessage.set(this.extractError(err));
        this.busy.set(false);
      },
    });
  }

  private extractError(err: unknown): string {
    if (err && typeof err === 'object' && 'error' in err) {
      const inner = (err as { error?: { error?: string } }).error;
      if (inner?.error) {
        return inner.error;
      }
    }
    return 'Something went wrong talking to the backend. Is it running on http://localhost:5108?';
  }
}
