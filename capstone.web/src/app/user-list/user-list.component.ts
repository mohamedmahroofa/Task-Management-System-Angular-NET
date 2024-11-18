import { Component, OnInit } from '@angular/core';
import { User } from '../models/user';
import { UserService } from '../services/user.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-user-list',
  templateUrl: './user-list.component.html',
  styleUrl: './user-list.component.css'
})
export class UserListComponent implements OnInit{
  users: User[] = []; // Array to hold list of users
  displayedColumns: string[] = ['firstName', 'lastName', 'username', 'email', 'role'];

  constructor(
    private userService: UserService,
    private router: Router
  ) {}
  
  ngOnInit(){
    // Fetch Prioritys on component initialization
    this.userService.getUsers().subscribe((data) => {
     this.users = data;
   });
}

  // Navigate to User details component
  viewUser(id: number) {
    this.router.navigate(['/user', id]);
  }
  
  addUser() {
    this.router.navigate(['/user']);
  }
  goBack() {
    this.router.navigate(['/']);
  }

}
