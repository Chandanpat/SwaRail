import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root'
})
export class TrainService {

  private baseUrl = 'http://localhost:5157/api/Train';

  constructor(private http: HttpClient, private authService: AuthService) { }

  getAllTrains(): Observable<any[]> {
    const headers = this.authService.getAuthHeaders();
    return this.http.get<any[]>(`${this.baseUrl}/GetAllTrains`, { headers });
  }

  getTrainById(trainId: number): Observable<any> {
    const headers = this.authService.getAuthHeaders();
    return this.http.get<any>(`${this.baseUrl}/GetTrainByID?trainId=${trainId}`, { headers });
  }

  getTrainByName(trainName: string): Observable<any> {
    const headers = this.authService.getAuthHeaders();
    return this.http.get<any>(`${this.baseUrl}/GetTrainByTrainName?trainName=${trainName}`, { headers });
  }

  cancelTrain(trainId: number): Observable<void> {
    const headers = this.authService.getAuthHeaders();
    return this.http.delete<void>(`${this.baseUrl}/DeleteTrain/${trainId}`, { headers });
  }  

  addTrain(trainData: any): Observable<any> {
    const headers = this.authService.getAuthHeaders();
    return this.http.post(`${this.baseUrl}/AddTrain`, trainData, { headers });
  }

  updateTrain(trainId: number, trainData: any) {
    const headers = this.authService.getAuthHeaders();
    return this.http.put(`${this.baseUrl}/UpdateTrain/${trainId}`, trainData, { headers });
  }

}

