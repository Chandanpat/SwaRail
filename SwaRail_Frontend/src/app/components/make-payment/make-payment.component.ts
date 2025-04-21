import { Component } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ReservationService } from '../../services/reservation.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-make-payment',
  standalone: true,
  imports: [FormsModule, CommonModule, ReactiveFormsModule],
  templateUrl: './make-payment.component.html',
  styleUrl: './make-payment.component.css'
})
export class MakePaymentComponent {
  paymentForm: FormGroup;
  ticketDetails: any;
  reservationId: number;

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private reservationService: ReservationService,
    private router: Router
  ) {
    this.paymentForm = this.fb.group({
      reservationID: [{ value: '', disabled: true }, Validators.required],
      amount: [{ value: '', disabled: true }, Validators.required],
      paymentMethod: ['', Validators.required]
    });

    this.reservationId = Number(this.route.snapshot.paramMap.get('reservationId'));
  }

  ngOnInit() {
    this.loadTicketDetails();
  }

  loadTicketDetails() {
    if (!this.reservationId) {
      alert('Invalid reservation ID');
      return;
    }

    this.reservationService.getTicketDetails(this.reservationId).subscribe({
      next: (response) => {
        console.log(response);
        response = response.updatedTicket;
        this.ticketDetails = response;
        this.paymentForm.patchValue({
          reservationID: response.pnr,
          amount: response.totalFare
        });
      },
      error: (error) => {
        // console.error('Error fetching ticket details:', error);
        alert('Failed to load ticket details. Please try again.');
      }
    });
  }

  makePayment() {
    if (this.paymentForm.invalid) return;

    const paymentData = {
      reservationID: this.ticketDetails.pnr,
      amount: this.ticketDetails.totalFare,
      paymentMethod: this.paymentForm.get('paymentMethod')?.value
    };

    this.reservationService.makePayment(paymentData).subscribe({
      next: (response) => {
        alert('Payment successful!');
        this.router.navigate(['/show-ticket', response.pnr]);
      },
      error: (error) => {
        // console.error('Payment error:', error);
        alert('Payment failed. Please try again.');
      }
    });
  }
}
