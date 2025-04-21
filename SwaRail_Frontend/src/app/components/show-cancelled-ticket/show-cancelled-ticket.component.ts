import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ReservationService } from '../../services/reservation.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-show-cancelled-ticket',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './show-cancelled-ticket.component.html',
  styleUrl: './show-cancelled-ticket.component.css'
})
export class ShowCancelledTicketComponent implements OnInit {
  cancelledTicket: any;
  refundDetails: any;

  constructor(
    private route: ActivatedRoute,
    private reservationService: ReservationService,
    private router: Router
  ) {}

  ngOnInit(): void {
    const reservationId = this.route.snapshot.paramMap.get('reservationId');
    // console.log(reservationId);
    if (reservationId) {
      this.fetchCancelledTicketDetails(Number(reservationId));
    }
  }

  fetchCancelledTicketDetails(reservationId: number): void {
    this.reservationService.getCancelledTicket(reservationId).subscribe({
      next: (response) => {
        // console.log(response);
        this.cancelledTicket = response.updatedTicket;
        this.refundDetails = response.refundDetails;
      },
      error: (error) => {
        console.error('Error fetching cancelled ticket:', error);
      }
    });
  }

  cancelBooking(reservationID: number) {
    this.router.navigate(['/cancel-booking', reservationID]);
  }

  goBackToHome(): void {
    this.router.navigate(['/user-dashboard']);
  }

  printTicket() {
    const printContents = document.getElementById('print-section')?.innerHTML;
    if (!printContents) return;
  
    const printWindow = window.open('', '', 'height=800,width=700');
    if (printWindow) {
      printWindow.document.write(`
        <html>
          <head>
            <title>Cancelled Ticket</title>
            <style>
              /* 🎟 Ticket Layout */
              .ticket-card {
                background: white;
                border-radius: 12px;
                box-shadow: none;
                padding: 20px;
                font-family: Arial, sans-serif;
              }
  
              .card-header {
                background: #dc3545;
                color: white;
                padding: 18px;
                font-weight: bold;
                font-size: 20px;
                text-align: center;
                border-radius: 12px 12px 0 0;
              }
  
              .section {
                padding: 20px;
              }
  
              .section-title {
                font-weight: bold;
                color: #003366;
                border-bottom: 2px solid #003366;
                padding-bottom: 8px;
                margin-bottom: 12px;
                font-size: 18px;
              }
  
              .highlighted-pnr {
                font-size: 20px;
                font-weight: bold;
                color: #FF6600;
              }
  
              .train-info {
                display: grid;
                grid-template-columns: 1fr 1fr;
                column-gap: 15px;
                row-gap: 10px;
                font-size: 16px;
              }
  
              .train-info p {
                margin: 0;
              }
  
              .passenger-info li {
                font-size: 15px;
                padding: 8px;
                border-bottom: 1px solid #ddd;
                list-style: none;
              }
  
              .payment-info, .refund-info {
                background: #eef2f5;
                padding: 15px;
                border-radius: 10px;
                margin-top: 20px;
              }
  
              .payment-line {
                display: flex;
                justify-content: space-between;
                padding: 8px 0;
                font-size: 16px;
              }
  
              .badge {
                font-size: 14px;
                padding: 6px 12px;
                border-radius: 6px;
                color: white;
                display: inline-block;
              }
  
              .bg-danger {
                background-color: #dc3545 !important;
              }
  
              .bg-warning {
                background-color: #ffc107 !important;
                color: black;
              }
  
              .bg-success {
                background-color: #28a745 !important;
              }
  
              .bg-info {
                background-color: #17a2b8 !important;
              }
  
              @media print {
                body {
                  -webkit-print-color-adjust: exact;
                  print-color-adjust: exact;
                }
              }
            </style>
          </head>
          <body onload="window.print(); window.close();">
            <div class="ticket-card">${printContents}</div>
          </body>
        </html>
      `);
      printWindow.document.close();
    }
  }
  
}
