import { Component } from '@angular/core';
import { Router  } from '@angular/router';
import { UserService } from '../services/user.service';
import { HttpClient } from '@angular/common/http';


@Component({
  selector: 'app-registration',
  templateUrl: './registration.component.html',
  styleUrl: './registration.component.css'
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

  constructor(private http: HttpClient) {}

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
      password: this.password,
    };

    this.http.post('https://yourapi.com/api/users/register', userData)
      .subscribe({
        next: (response) => {
          this.message = 'User registered successfully!';
        },
        error: (error) => {
          this.message = 'Error registering user: ' + error.error;
          console.error(error);
        }
      });
  }
}
