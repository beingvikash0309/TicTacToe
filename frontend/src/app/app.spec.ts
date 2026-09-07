import { TestBed } from '@angular/core/testing';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideHttpClient } from '@angular/common/http';

import { App } from './app';
import { API_BASE_URL } from './core/api-config';
import { GameStateDto } from './models/game.models';

describe('App', () => {
  let httpMock: HttpTestingController;

  const emptyGame: GameStateDto = {
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

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [App],
      providers: [provideHttpClient(), provideHttpClientTesting()],
    }).compileComponents();

    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should create the app and start a new game on init', () => {
    const fixture = TestBed.createComponent(App);
    const app = fixture.componentInstance;
    fixture.detectChanges();

    const req = httpMock.expectOne(`${API_BASE_URL}/games`);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual({ mode: 'TwoPlayer' });
    req.flush(emptyGame);

    expect(app).toBeTruthy();
    expect(app['game']()).toEqual(emptyGame);
  });

  it('renders the board once the game loads', () => {
    const fixture = TestBed.createComponent(App);
    fixture.detectChanges();

    const req = httpMock.expectOne(`${API_BASE_URL}/games`);
    req.flush(emptyGame);
    fixture.detectChanges();

    const board = fixture.nativeElement.querySelector('app-board');
    expect(board).toBeTruthy();
  });
});
