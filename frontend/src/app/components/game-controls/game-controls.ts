import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { GameMode } from '../../models/game.models';

@Component({
  selector: 'app-game-controls',
  imports: [FormsModule],
  templateUrl: './game-controls.html',
  styleUrl: './game-controls.scss',
})
export class GameControls {
  @Input({ required: true }) mode: GameMode = 'TwoPlayer';
  @Input() canUndo = false;

  @Output() modeChange = new EventEmitter<GameMode>();
  @Output() undo = new EventEmitter<void>();
  @Output() resetGame = new EventEmitter<void>();

  onModeSelect(value: string): void {
    this.modeChange.emit(value as GameMode);
  }
}
