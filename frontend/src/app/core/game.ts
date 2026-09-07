import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { API_BASE_URL } from './api-config';
import { GameMode, GameStateDto, MoveRequestDto, ScoreboardDto } from '../models/game.models';

@Injectable({
  providedIn: 'root',
})
export class GameService {
  private readonly http = inject(HttpClient);
  private readonly gamesUrl = `${API_BASE_URL}/games`;
  private readonly scoreboardUrl = `${API_BASE_URL}/scoreboard`;

  createGame(mode: GameMode): Observable<GameStateDto> {
    return this.http.post<GameStateDto>(this.gamesUrl, { mode });
  }

  getGame(id: string): Observable<GameStateDto> {
    return this.http.get<GameStateDto>(`${this.gamesUrl}/${id}`);
  }

  makeMove(id: string, move: MoveRequestDto): Observable<GameStateDto> {
    return this.http.post<GameStateDto>(`${this.gamesUrl}/${id}/moves`, move);
  }

  undo(id: string): Observable<GameStateDto> {
    return this.http.post<GameStateDto>(`${this.gamesUrl}/${id}/undo`, {});
  }

  resetGame(id: string): Observable<GameStateDto> {
    return this.http.post<GameStateDto>(`${this.gamesUrl}/${id}/reset`, {});
  }

  getScoreboard(): Observable<ScoreboardDto> {
    return this.http.get<ScoreboardDto>(this.scoreboardUrl);
  }

  resetScoreboard(): Observable<ScoreboardDto> {
    return this.http.post<ScoreboardDto>(`${this.scoreboardUrl}/reset`, {});
  }
}
