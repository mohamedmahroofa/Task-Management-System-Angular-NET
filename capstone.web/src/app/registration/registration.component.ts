import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-registration',
  templateUrl: './registration.component.html',
  styleUrls: ['./registration.component.css'] // Fixed styleUrl to styleUrls
})
export class RegistrationComponent {
  firstName: string = '';
  lastName: string = '';
  email: string = '';
  username: string = '';
  password: string = '';
  confirmPassword: string = '';
  role: string = '';
  message: string = '';
  isSubmitting: boolean = false; // Added to manage submission state

  constructor(private http: HttpClient, private router: Router) {}

  register() {
    if (this.password !== this.confirmPassword) {
      this.message = "Passwords do not match!";
      return;
    }

    const userData = {
      firstName: this.firstName,
      lastName: this.lastName,
      email: this.email,
      username: this.username,
      passwordHash: this.password,
      role: this.role // Include role in the request
    };

    this.isSubmitting = true; // Set submitting state

    this.http.post('https://localhost:7197/api/users', userData)
      .subscribe({
        next: (response) => {
          this.message = 'User registered successfully!';
          this.router.navigate(['/login']); // Navigate to login or another page
        },
        
        error: (error) => {
          this.message = 'Error registering user: ' + (error.error || 'Server error');
          console.error(error);
        },
        complete: () => {
          this.isSubmitting = false; // Reset submitting state
        }
      });
  }
}
