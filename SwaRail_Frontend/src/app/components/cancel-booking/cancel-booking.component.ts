import { Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ReservationService } from '../../services/reservation.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-cancel-booking',
  standalone: true,
  imports: [FormsModule, CommonModule, ReactiveFormsModule],
  templateUrl: './cancel-booking.component.html',
  styleUrl: './cancel-booking.component.css'
})
export class CancelBookingComponent implements OnInit {
  ticket: any;
  cancelForm: FormGroup;

  constructor(
    private route: ActivatedRoute,
    private reservationService: ReservationService,
    private fb: FormBuilder,
    private router: Router
  ) {
    this.cancelForm = this.fb.group({
      reservationID: [null],
      passengers: this.fb.array([]) // FormArray
    });
  }

  ngOnInit() {
    const reservationID = this.route.snapshot.paramMap.get('id');
    if (reservationID) {
      this.loadTicketDetails(parseInt(reservationID, 10));
    }
  }

  loadTicketDetails(reservationID: number) {
    this.reservationService.getTicketDetails(reservationID).subscribe(
      (data) => {
        if (data) {
          // console.log("Ticket Details: ", data); 
          // this.ticket = data;
          this.ticket = data.updatedTicket;
          // console.log(this.ticket);
          this.cancelForm.patchValue({ reservationID: data.pnr });
          // console.log(data.updatedTicket.passengers);
          // if (data.passengers && data.passengers.length > 0) {
          if (data.updatedTicket.passengers && data.updatedTicket.passengers.length > 0) {
            this.initializePassengers(data.updatedTicket.passengers);
          } else {
            console.warn("⚠️ No passengers found.");
          }
        }
      },
      (error) => console.error('Error fetching ticket:', error)
    );
  }

  get passengers(): FormArray {
    return this.cancelForm.get('passengers') as FormArray;
  }
  
  initializePassengers(passengers: any[]) {
    const passengerArray = this.passengers;
    passengerArray.clear(); // Clear previous values

    passengers.forEach(passenger => {
      passengerArray.push(this.fb.group({
        passengerID: [passenger.passengerID],
        passengerName: [passenger.passengerName],
        age: [passenger.age],
        sex: [passenger.sex],
        coachNo: [passenger.coachNo],
        seatNo: [passenger.seatNo],
        selected: [false] // Checkbox control
      }));
    });

    // console.log("Passengers added to FormArray: ", this.passengers.value); 
  }

  confirmCancellation() {
    const selectedPassengers = this.cancelForm.value.passengers
      .filter((p: any) => p.selected)
      .map((p: any) => p.passengerID);

    if (selectedPassengers.length === 0) {
      alert('Please select at least one passenger to cancel.');
      return;
    }

    const cancelRequest = {
      reservationID: this.ticket.pnr,
      passengerIDs: selectedPassengers
    };

    console.log("Cancel Request Payload:", cancelRequest);

    this.reservationService.cancelBooking(cancelRequest).subscribe(
      (response) => {
        console.log(response);
        alert('Booking Cancelled Successfully');
        this.router.navigate(['/show-cancelled-ticket', this.ticket.pnr]);
      },
      (error) => console.error('Cancellation Failed:', error)
    );
  }
}
