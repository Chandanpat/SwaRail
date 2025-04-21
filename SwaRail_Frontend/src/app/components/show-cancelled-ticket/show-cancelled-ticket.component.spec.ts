import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ShowCancelledTicketComponent } from './show-cancelled-ticket.component';

describe('ShowCancelledTicketComponent', () => {
  let component: ShowCancelledTicketComponent;
  let fixture: ComponentFixture<ShowCancelledTicketComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ShowCancelledTicketComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ShowCancelledTicketComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
