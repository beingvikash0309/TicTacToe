import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Board } from './board';

describe('Board', () => {
  let fixture: ComponentFixture<Board>;
  let component: Board;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Board],
    }).compileComponents();

    fixture = TestBed.createComponent(Board);
    component = fixture.componentInstance;
    component.cells = Array(9).fill(null);
    fixture.detectChanges();
  });

  it('should be created', () => {
    expect(component).toBeTruthy();
  });

  it('renders 9 cells', () => {
    const cells = fixture.nativeElement.querySelectorAll('.cell');
    expect(cells.length).toBe(9);
  });

  it('emits cellClicked when an empty cell is clicked', () => {
    spyOn(component.cellClicked, 'emit');
    const cells = fixture.nativeElement.querySelectorAll('.cell');
    cells[0].click();

    expect(component.cellClicked.emit).toHaveBeenCalledWith(0);
  });

  it('does not emit when the cell is already occupied', () => {
    component.cells = ['X', null, null, null, null, null, null, null, null];
    fixture.detectChanges();
    spyOn(component.cellClicked, 'emit');

    const cells = fixture.nativeElement.querySelectorAll('.cell');
    cells[0].click();

    expect(component.cellClicked.emit).not.toHaveBeenCalled();
  });

  it('does not emit when the board is disabled', () => {
    component.disabled = true;
    fixture.detectChanges();
    spyOn(component.cellClicked, 'emit');

    const cells = fixture.nativeElement.querySelectorAll('.cell');
    cells[0].click();

    expect(component.cellClicked.emit).not.toHaveBeenCalled();
  });

  it('marks winning cells', () => {
    component.winningCells = [0, 1, 2];
    fixture.detectChanges();

    const cells = fixture.nativeElement.querySelectorAll('.cell');
    expect(cells[0].classList).toContain('winning');
    expect(cells[3].classList).not.toContain('winning');
  });
});
