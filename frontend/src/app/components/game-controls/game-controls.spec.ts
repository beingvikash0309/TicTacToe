import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GameControls } from './game-controls';

describe('GameControls', () => {
  let fixture: ComponentFixture<GameControls>;
  let component: GameControls;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GameControls],
    }).compileComponents();

    fixture = TestBed.createComponent(GameControls);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should be created', () => {
    expect(component).toBeTruthy();
  });

  it('disables Undo when canUndo is false', () => {
    component.canUndo = false;
    fixture.detectChanges();
    const undoButton: HTMLButtonElement = fixture.nativeElement.querySelectorAll('button')[0];
    expect(undoButton.disabled).toBeTrue();
  });

  it('enables Undo when canUndo is true', () => {
    component.canUndo = true;
    fixture.detectChanges();
    const undoButton: HTMLButtonElement = fixture.nativeElement.querySelectorAll('button')[0];
    expect(undoButton.disabled).toBeFalse();
  });

  it('emits resetGame when Reset Game is clicked', () => {
    spyOn(component.resetGame, 'emit');
    const buttons: HTMLButtonElement[] = fixture.nativeElement.querySelectorAll('button');
    buttons[1].click();

    expect(component.resetGame.emit).toHaveBeenCalled();
  });
});
