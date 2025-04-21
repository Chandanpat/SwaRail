import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Component } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent {
  registerForm: FormGroup;
  errorMessage: string = '';

  constructor(private fb: FormBuilder, private authService: AuthService, private router: Router) {
    this.registerForm = this.fb.group({
      fullName: ['', [Validators.required]],
      userName: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      roles: [['User']] 
    });
  }

  register() {
    if (this.registerForm.valid) {
      console.log('Sending Data:', this.registerForm.value);

      this.authService.register(this.registerForm.value).subscribe({
        next: () => {
          console.log('Registration successful');
          alert('Registration successful');
          this.router.navigate(['/login']);
        },
        error: (error) => {
          console.error('Error details:', error.error); 
          if (error.error && error.error.length > 0) {
            const errorCode = error.error[0].code;
            if (errorCode === 'DuplicateUserName') {
              this.errorMessage = "This email is already registered. Try another email.";
              return;
            }
          }

          this.errorMessage = 'Registration failed. Try again.';
        }
      });
    }
  }
}


