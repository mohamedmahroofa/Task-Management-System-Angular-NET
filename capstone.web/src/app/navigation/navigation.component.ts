import { Component } from '@angular/core';
import { UserService } from '../services/user.service';
import { Router } from '@angular/router';


@Component({
  selector: 'app-navigation',
  templateUrl: './navigation.component.html',
  styleUrl: './navigation.component.css'
})
export class NavigationComponent {
  constructor(private userService: UserService, private router: Router) { }

  isAuthenticated(): boolean {
    return this.userService.isAuthenticated();
  }

  logout(): void {
    if (confirm('Do you want to logout?')) {
      this.userService.logout();
      this.router.navigate(['/login']);
    }
  }
}
