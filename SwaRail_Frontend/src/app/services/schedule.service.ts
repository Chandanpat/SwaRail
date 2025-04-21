import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { AuthService } from './auth.service';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ScheduleService {
  private baseUrl = 'http://localhost:5157/api/Schedule';

  constructor(private http: HttpClient, private authService: AuthService) {}

  getScheduleID(trainID: number, journeyDate: string): Observable<{ scheduleID: number }> {
    const headers = this.authService.getAuthHeaders();
    const url = `${this.baseUrl}/GetScheduleID?trainId=${trainID}&journeyDate=${journeyDate}`;
    return this.http.get<{ scheduleID: number }>(url, { headers });
  }

  getScheduleById(scheduleId: number): Observable<any> {
    const headers = this.authService.getAuthHeaders();
    return this.http.get<any>(`${this.baseUrl}/GetScheduleByID?scheduleIdId=${scheduleId}`, { headers });
  }

  getAllSchedules() {
    const headers = this.authService.getAuthHeaders();
    return this.http.get<any[]>(`${this.baseUrl}/GetAllSchedules`, { headers });
  }

  addSingleSchedule(data: any): Observable<any> {
    const headers = this.authService.getAuthHeaders();
    return this.http.post(`${this.baseUrl}/AddScheduleWithCoachesAndSeats`, data, { headers });
  }

  addMultipleSchedules(data: any): Observable<any> {
    const headers = this.authService.getAuthHeaders();
    return this.http.post(`${this.baseUrl}/AddMultipleSchedulesWithCoachesAndSeats`, data, { headers });
  }

  deleteSingleSchedule(schedule: any): Observable<any> {
    const headers = this.authService.getAuthHeaders();
    return this.http.request('delete', `${this.baseUrl}/DeleteScheduleWithCoachesAndSeats`, {
      body: schedule,
      headers
    });
  }

  deleteMultipleSchedules(schedule: any): Observable<any> {
    const headers = this.authService.getAuthHeaders();
    return this.http.request('delete', `${this.baseUrl}/DeleteMultipleSchedulesWithCoachesAndSeats`, {
      body: schedule,
      headers
    });
  }
}
