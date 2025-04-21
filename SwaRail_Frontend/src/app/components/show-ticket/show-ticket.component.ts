import { Component } from '@angular/core';
import { ReservationService } from '../../services/reservation.service';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-show-ticket',
  imports: [CommonModule],
  templateUrl: './show-ticket.component.html',
  styleUrl: './show-ticket.component.css',
})
export class ShowTicketComponent {
  ticket: any;

  constructor(
    private reservationService: ReservationService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit() {
    const reservationId = this.route.snapshot.paramMap.get('reservationId');
    if (reservationId) {
      this.fetchTicketDetails(parseInt(reservationId, 10));
    }
  }

  fetchTicketDetails(reservationId: number) {
    this.reservationService.getTicketDetails(reservationId).subscribe({
      next: (response) => {
        // console.log(response);
        this.ticket = response.updatedTicket;
      },
      error: (error) => {
        console.error('Error fetching ticket details:', error);
        alert('Failed to fetch ticket details. Please try again.');
      }
    });
  }

  cancelBooking(reservationID: number) {
    this.router.navigate(['/cancel-booking', reservationID]);
  }

  goBackToHome() {
    this.router.navigate(['/user-dashboard']);
  }

  printTicket() {
    const printContents = document.getElementById('print-section')?.innerHTML;
    const popupWin = window.open('', '_blank', 'width=900,height=650');
  
    if (popupWin && printContents) {
      popupWin.document.open();
      popupWin.document.write(`
        <html>
          <head>
            <title>Print Ticket</title>
  
            <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css">
            <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.0/css/all.min.css">
  
            <style>
              body {
                margin: 40px;
                font-family: 'Segoe UI', sans-serif;
                color: #000;
                background-color: #fff;
              }
  
              .ticket-container {
                max-width: 700px;
                margin: 0 auto;
              }
  
              .ticket-card {
                background: white;
                border-radius: 12px;
                box-shadow: 0 5px 10px rgba(0, 0, 0, 0.15);
                overflow: hidden;
                padding: 20px;
              }
  
              .card-header {
                background: #003366;
                padding: 18px;
                font-weight: bold;
                font-size: 20px;
                color: white;
                text-align: center;
              }
  
              .section {
                padding: 20px 0;
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
  
              .payment-info,
              .refund-info {
                background: #eef2f5;
                padding: 15px;
                border-radius: 10px;
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
              }
  
              ul.list-group li {
                font-size: 15px;
              }
  
              @media print {
                body {
                  -webkit-print-color-adjust: exact !important;
                  print-color-adjust: exact !important;
                }
              }
            </style>
          </head>
          <body onload="window.print(); window.close();">
            <div class="ticket-container">
              <div class="ticket-card">
                ${printContents}
              </div>
            </div>
          </body>
        </html>
      `);
      popupWin.document.close();
    }
  }
  
  
  
}
