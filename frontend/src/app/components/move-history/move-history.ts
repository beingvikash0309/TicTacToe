import { Component, Input } from '@angular/core';
import { MoveHistoryItem } from '../../models/game.models';

@Component({
  selector: 'app-move-history',
  imports: [],
  templateUrl: './move-history.html',
  styleUrl: './move-history.scss',
})
export class MoveHistory {
  @Input({ required: true }) moves: MoveHistoryItem[] = [];
}
