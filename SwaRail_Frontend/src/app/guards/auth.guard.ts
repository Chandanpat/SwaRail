import { Injectable } from '@angular/core';
import { CanActivate, Router, ActivatedRouteSnapshot } from '@angular/router';
import { AuthService } from '../services/auth.service';


@Injectable({
  providedIn: 'root',
})
export class AuthGuard implements CanActivate {
  constructor(private authService: AuthService, private router: Router) {}

  canActivate(route: ActivatedRouteSnapshot): boolean {
    const token = this.authService.getToken();
    if (!token) {
      alert("Access Denied! Please Login.");
      this.router.navigate(['/login']);
      return false;
    }

    // Role-Based Access 
    const expectedRole = route.data['expectedRole'];
    const userRole = this.authService.getRole();

    if (userRole !== expectedRole) {
      alert("Access Denied! Unauthorized Access.");
      this.router.navigate(['/login']); 
      return false;
    }

    return true;
  }
}
