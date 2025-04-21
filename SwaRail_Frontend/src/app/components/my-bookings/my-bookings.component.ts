import { Component, OnInit } from '@angular/core';
import { ReservationService } from '../../services/reservation.service';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-my-bookings',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './my-bookings.component.html',
  styleUrl: './my-bookings.component.css'
})
export class MyBookingsComponent implements OnInit {
  bookings: any[] = [];
  errorMessage: string = '';

  constructor(private reservationService: ReservationService, private router: Router) {}

  ngOnInit(): void {
    this.loadBookings();
  }

  loadBookings(): void {
    this.reservationService.getUserBookings().subscribe({
      next: (data) => {
        this.bookings = data;
      },
      error: (err) => {
        if(err.status === 404){
          this.errorMessage = 'No Bookings found.';
        }
        else{
          this.errorMessage = 'Failed to load bookings. Please try again later.';
        }
      }
    });
  }

  viewBookingDetails(booking: any): void {
    if (booking.status === 'Cancelled' || booking.status === 'Partially Cancelled') {
      this.router.navigate(['/show-cancelled-ticket', booking.pnr]);
    } else {
      this.router.navigate(['/show-ticket', booking.pnr]);
    }
  }

  makePayment(booking: any) {
    this.router.navigate(['/make-payment', booking.pnr]);
  }
}
