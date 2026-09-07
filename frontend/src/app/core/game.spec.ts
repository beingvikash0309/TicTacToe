import { TestBed } from '@angular/core/testing';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideHttpClient } from '@angular/common/http';

import { GameService } from './game';
import { API_BASE_URL } from './api-config';
import { GameStateDto } from '../models/game.models';

describe('GameService', () => {
  let service: GameService;
  let httpMock: HttpTestingController;

  const sampleGame: GameStateDto = {
    id: 'game-1',
    board: Array(9).fill(null),
    currentPlayer: 'X',
    mode: 'TwoPlayer',
    status: 'InProgress',
    winner: null,
    winningCells: null,
    moveHistory: [],
    scoreboard: { xWins: 0, oWins: 0, draws: 0 },
    canUndo: false,
  };

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });
    service = TestBed.inject(GameService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('createGame posts the selected mode', () => {
    service.createGame('VsComputer').subscribe((game) => {
      expect(game).toEqual(sampleGame);
    });

    const req = httpMock.expectOne(`${API_BASE_URL}/games`);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual({ mode: 'VsComputer' });
    req.flush(sampleGame);
  });

  it('makeMove posts to the moves endpoint for the given game', () => {
    service.makeMove('game-1', { player: 'X', row: 0, col: 0 }).subscribe();

    const req = httpMock.expectOne(`${API_BASE_URL}/games/game-1/moves`);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual({ player: 'X', row: 0, col: 0 });
    req.flush(sampleGame);
  });

  it('undo posts to the undo endpoint', () => {
    service.undo('game-1').subscribe();

    const req = httpMock.expectOne(`${API_BASE_URL}/games/game-1/undo`);
    expect(req.request.method).toBe('POST');
    req.flush(sampleGame);
  });

  it('resetGame posts to the reset endpoint', () => {
    service.resetGame('game-1').subscribe();

    const req = httpMock.expectOne(`${API_BASE_URL}/games/game-1/reset`);
    expect(req.request.method).toBe('POST');
    req.flush(sampleGame);
  });

  it('getScoreboard issues a GET request', () => {
    service.getScoreboard().subscribe();

    const req = httpMock.expectOne(`${API_BASE_URL}/scoreboard`);
    expect(req.request.method).toBe('GET');
    req.flush({ xWins: 1, oWins: 0, draws: 0 });
  });

  it('resetScoreboard posts to the scoreboard reset endpoint', () => {
    service.resetScoreboard().subscribe();

    const req = httpMock.expectOne(`${API_BASE_URL}/scoreboard/reset`);
    expect(req.request.method).toBe('POST');
    req.flush({ xWins: 0, oWins: 0, draws: 0 });
  });
});
