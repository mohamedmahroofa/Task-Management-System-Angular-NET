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

  usermanagement: UserManagement = {
    priorityId: 0,
    name: '',
    dateCreated: new Date(),
    isDeleted: false,
    color: '',
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
    this.usermanagementService.getUserManagement(parseInt(id)).subscribe(data => {
      this.usermanagement = data;
      console.log(this.usermanagement)

   /*   if(typeof this.priority.dateCreated === "string") {
        this.priority.dateCreated = this.priority.dateCreated.split('T')[0];
      } */
    });
  }
}

saveUserManagement() {
  if (this.usermanagement.priorityId) {
    this.usermanagementService.updateUserManagement(this.usermanagement).subscribe(() => {
      this.goBack();
    });
  } else {
    this.usermanagementService.addUserManagement(this.usermanagement).subscribe(() => {
      this.goBack();
    });
  }
}



deleteUserManagement() {
  if (this.usermanagement.priorityId) {
    this.usermanagementService.deleteUserManagement(this.usermanagement.priorityId).subscribe(() => {
      this.goBack();
    }, error => {
      console.error('Error deleting user:', error);
      // Handle error appropriately (e.g., show error message)
    });
  }
}

goBack() {
  this.router.navigate(['/usersmanagement']);
}
}

