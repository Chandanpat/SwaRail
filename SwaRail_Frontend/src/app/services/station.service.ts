import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { AuthService } from './auth.service';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class StationService {

  private baseUrl = 'http://localhost:5157/api/Station';

  constructor(private http: HttpClient, private authService: AuthService) { }

  getStations(): Observable<any[]> {
    const headers = this.authService.getAuthHeaders();
    return this.http.get<any[]>(`${this.baseUrl}/GetAllStations`, { headers });
  }

  getStationByName(stationName: string): Observable<any> {
    const headers = this.authService.getAuthHeaders();
    return this.http.get<any>(`${this.baseUrl}/GetStationByName?stationName=${stationName}`, { headers });
  }

  addStation(data: { stationName: string }): Observable<any> {
    const headers = this.authService.getAuthHeaders();
    return this.http.post(`${this.baseUrl}/AddStation`, data, { headers });
  }

  updateStation(stationId: number, data: any): Observable<any> {
    const headers = this.authService.getAuthHeaders();
    return this.http.put(`${this.baseUrl}/UpdateStation/${stationId}`, data, { headers });
  }

  deleteStation(id: number): Observable<any> {
    const headers = this.authService.getAuthHeaders();
    return this.http.delete(`${this.baseUrl}/DeleteStation/${id}`, {
      headers,
      responseType: 'text' as 'json' // This prevents Angular from throwing a parsing error
    });
  }
}
