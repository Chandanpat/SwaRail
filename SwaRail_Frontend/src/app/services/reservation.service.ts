import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { AuthService } from './auth.service';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ReservationService {
  private baseUrl = 'http://localhost:5157/api/Reservation';

  constructor(private http: HttpClient, private authService: AuthService) { }

  getAvailableTrains(sourceStation: string, destinationStation: string, journeyDate: string): Observable<any[]> {
    const headers = this.authService.getAuthHeaders();
    const url = `${this.baseUrl}/SearchTrains?sourceStation=${sourceStation}&destinationStation=${destinationStation}&journeyDate=${journeyDate}`;
    return this.http.get<any[]>(url, { headers });
  }

  bookTicket(bookingData: any): Observable<any> {
    const headers = this.authService.getAuthHeaders();
    return this.http.post<any>(`${this.baseUrl}/BookTicket`, bookingData, { headers });
  }

  getTicketDetails(reservationId: number): Observable<any> {
    const headers = this.authService.getAuthHeaders();
    return this.http.get<any>(`${this.baseUrl}/ShowTicket/${reservationId}`, { headers });
  }

  makePayment(paymentData: any): Observable<any> {
    const headers = this.authService.getAuthHeaders();
    return this.http.post<any>(`${this.baseUrl}/MakePayment`, paymentData, { headers });
  }

  getUserBookings(): Observable<any[]> {
    const headers = this.authService.getAuthHeaders();
    return this.http.get<any[]>(`${this.baseUrl}/UserBookings`, { headers });
  }

  cancelBooking(cancelRequest: any): Observable<any> {
    const headers = this.authService.getAuthHeaders();
    return this.http.post(`${this.baseUrl}/CancelBooking`, cancelRequest, { headers });
  }

  getCancelledTicket(reservationId: number): Observable<any> {
    const headers = this.authService.getAuthHeaders();
    return this.http.get(`${this.baseUrl}/ShowTicket/${reservationId}`, { headers });
  }
}
