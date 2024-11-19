import { Component , OnInit } from '@angular/core';
import { UserManagement } from '../models/user-management';
import { UserManagementService } from '../services/user-management.service';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-user-management',
  templateUrl: './user-management.component.html',
  styleUrl: './user-management.component.css'
})

export class UserManagementComponent implements OnInit  {
  user: UserManagement = {
    userId: 0,
    firstName: '',
    lastName: '',
    email: '',
    username: '',
    password: '',
    role: 'general' // default role
};
constructor(
  private route: ActivatedRoute,
  private usermanagementService: UserManagementService,
  private router: Router
) {}

ngOnInit() {
  const id = this.route.snapshot.paramMap?.get('id'); // Get priority id from route
  if (id) {
    // Fetch priority details if id exists
    this.usermanagementService.getUser(parseInt(id)).subscribe(data => {
      this.user = data;
      console.log(this.user)

   /*   if(typeof this.priority.dateCreated === "string") {
        this.priority.dateCreated = this.priority.dateCreated.split('T')[0];
      } */
    });
  }
}

saveUser() {
  if (this.user.userId) {
    this.usermanagementService.updateUser(this.user).subscribe(() => {
      this.goBack();
    });
  } else {
    this.usermanagementService.addUser(this.user).subscribe(() => {
      this.goBack();
    });
  }
}

goBack() {
  this.router.navigate(['/users']);
}
}

