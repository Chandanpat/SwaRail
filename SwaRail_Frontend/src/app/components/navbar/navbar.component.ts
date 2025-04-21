import { Component, NgZone } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.css'
})
export class NavbarComponent {

  isLoggedIn: boolean = false;
  userRole: string | null = null;
  private authSubscription: Subscription = new Subscription();

  constructor(private authService: AuthService, private router: Router, private ngZone: NgZone) {}

  ngOnInit() {
    this.authSubscription = this.authService.isLoggedIn().subscribe((loggedIn) => {
      this.ngZone.run(() => {
        this.isLoggedIn = loggedIn;
        this.userRole = this.authService.getRole();
      });
    });
  }

  goToHome(): void {
    if (this.userRole === 'Admin') {
      this.router.navigate(['/admin-dashboard']);
    } else if (this.userRole === 'User') {
      this.router.navigate(['/user-dashboard']);
    } else {
      this.router.navigate(['/login']);
    }
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['/login']);
  }

  ngOnDestroy(): void {
    this.authSubscription.unsubscribe();
  }
}