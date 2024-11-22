import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-registration',
  templateUrl: './registration.component.html',
  styleUrls: ['./registration.component.css']
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
  isSubmitting: boolean = false;

  constructor(private http: HttpClient, private router: Router) {}

  register() {
    // Ensure passwords match
    if (this.password !== this.confirmPassword) {
      this.message = 'Passwords do not match!';
      return;
    }

    // Create user data object
    const userData = {
      firstName: this.firstName,
      lastName: this.lastName,
      email: this.email,
      username: this.username,
      passwordHash: this.password, // Adjust if the backend uses a different key
      role: this.role
    };

    // Set submission state
    this.isSubmitting = true;

    // Make API call
    this.http.post('https://localhost:7197/api/users', userData).subscribe({
      next: (response) => {
        this.message = 'User registered successfully!';
        this.router.navigate(['/login']);
      },
      error: (error) => {
        this.message = 'Error registering user: ' + (error.error || 'Server error');
        console.error(error);
      },
      complete: () => {
        this.isSubmitting = false;
      }
    });
  }
}
