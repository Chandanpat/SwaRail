import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GetTrainsComponent } from './get-trains.component';

describe('GetTrainsComponent', () => {
  let component: GetTrainsComponent;
  let fixture: ComponentFixture<GetTrainsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GetTrainsComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(GetTrainsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
