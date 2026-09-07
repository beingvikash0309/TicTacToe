import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Scoreboard } from './scoreboard';

describe('Scoreboard', () => {
  let fixture: ComponentFixture<Scoreboard>;
  let component: Scoreboard;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Scoreboard],
    }).compileComponents();

    fixture = TestBed.createComponent(Scoreboard);
    component = fixture.componentInstance;
    component.scoreboard = { xWins: 2, oWins: 1, draws: 3 };
    fixture.detectChanges();
  });

  it('should be created', () => {
    expect(component).toBeTruthy();
  });

  it('renders the tallies', () => {
    const values = fixture.nativeElement.querySelectorAll('.value');
    expect(values[0].textContent).toContain('2');
    expect(values[1].textContent).toContain('1');
    expect(values[2].textContent).toContain('3');
  });

  it('emits resetScoreboard when the button is clicked', () => {
    spyOn(component.resetScoreboard, 'emit');
    fixture.nativeElement.querySelector('.reset-scoreboard').click();

    expect(component.resetScoreboard.emit).toHaveBeenCalled();
  });
});
