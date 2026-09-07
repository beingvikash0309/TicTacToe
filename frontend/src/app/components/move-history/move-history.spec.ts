import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MoveHistory } from './move-history';

describe('MoveHistory', () => {
  let fixture: ComponentFixture<MoveHistory>;
  let component: MoveHistory;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MoveHistory],
    }).compileComponents();

    fixture = TestBed.createComponent(MoveHistory);
    component = fixture.componentInstance;
    component.moves = [];
    fixture.detectChanges();
  });

  it('should be created', () => {
    expect(component).toBeTruthy();
  });

  it('shows an empty message when there are no moves', () => {
    const empty = fixture.nativeElement.querySelector('.empty');
    expect(empty?.textContent).toContain('No moves yet');
  });

  it('renders a row per move', () => {
    component.moves = [
      { moveNumber: 1, player: 'X', row: 0, col: 0, position: 'Row 1, Column 1' },
      { moveNumber: 2, player: 'O', row: 1, col: 1, position: 'Row 2, Column 2' },
    ];
    fixture.detectChanges();

    const rows = fixture.nativeElement.querySelectorAll('tbody tr');
    expect(rows.length).toBe(2);
    expect(rows[0].textContent).toContain('Row 1, Column 1');
  });
});
