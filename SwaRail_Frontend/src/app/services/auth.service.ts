import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private apiUrl = 'http://localhost:5157/api/Auth';
  private isLoggedInSubject = new BehaviorSubject<boolean>(this.hasToken());

  constructor(private http: HttpClient) { }

  login(credentials: { userName: string; password: string }): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/Login`, credentials);
  }

  register(userData: { fullName: string; userName: string; password: string; roles?: string[] }): Observable<any> {
    return this.http.post(`${this.apiUrl}/Register`, userData, {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' })
    });
  }

  setToken(token: string): void {
    if (typeof window !== 'undefined') {
      localStorage.setItem('jwtToken', token);
      this.isLoggedInSubject.next(true); 
    }
  }

  getToken(): string | null {
    if (typeof window !== 'undefined') {
      return localStorage.getItem('jwtToken');
    }
    return null;
  }

  getAuthHeaders(): HttpHeaders {
    const token = this.getToken();
    return new HttpHeaders({
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json'
    });
  }

  getRole(): string | null {
    const token = this.getToken();
    if (token) {
      try {
        const payload = JSON.parse(atob(token.split('.')[1]));
        return payload["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] || payload.role || null;
      } catch (error) {
        console.error('Error decoding JWT:', error);
      }
    }
    return null;
  }

  logout(): void {
    if (typeof window !== 'undefined') {
      localStorage.removeItem('jwtToken');
      this.isLoggedInSubject.next(false); 
    }
  }

  hasToken(): boolean {
    if (typeof window !== 'undefined') {
      return !!localStorage.getItem('jwtToken');
    }
    return false;
  }

  isLoggedIn(): Observable<boolean> {
    return this.isLoggedInSubject.asObservable();
  }
}