import { Component, EventEmitter, Input, Output } from '@angular/core';
import { ScoreboardDto } from '../../models/game.models';

@Component({
  selector: 'app-scoreboard',
  imports: [],
  templateUrl: './scoreboard.html',
  styleUrl: './scoreboard.scss',
})
export class Scoreboard {
  @Input({ required: true }) scoreboard: ScoreboardDto = { xWins: 0, oWins: 0, draws: 0 };
  @Output() resetScoreboard = new EventEmitter<void>();
}
