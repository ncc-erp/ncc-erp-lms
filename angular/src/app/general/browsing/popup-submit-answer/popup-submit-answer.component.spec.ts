import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PopupSubmitAnswerComponent } from './popup-submit-answer.component';

describe('PopupSubmitAnswerComponent', () => {
  let component: PopupSubmitAnswerComponent;
  let fixture: ComponentFixture<PopupSubmitAnswerComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PopupSubmitAnswerComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PopupSubmitAnswerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
