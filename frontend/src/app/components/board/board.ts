import { Component, EventEmitter, Input, Output } from '@angular/core';
import { PlayerSymbol } from '../../models/game.models';

@Component({
  selector: 'app-board',
  imports: [],
  templateUrl: './board.html',
  styleUrl: './board.scss',
})
export class Board {
  @Input({ required: true }) cells: (PlayerSymbol | null)[] = Array(9).fill(null);
  @Input() winningCells: number[] | null = null;
  @Input() disabled = false;

  @Output() cellClicked = new EventEmitter<number>();

  readonly indexes = [0, 1, 2, 3, 4, 5, 6, 7, 8];

  onCellClick(index: number): void {
    if (this.disabled || this.cells[index] !== null) {
      return;
    }
    this.cellClicked.emit(index);
  }

  isWinningCell(index: number): boolean {
    return this.winningCells?.includes(index) ?? false;
  }
}
