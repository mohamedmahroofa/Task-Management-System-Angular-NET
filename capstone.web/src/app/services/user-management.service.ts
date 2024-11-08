import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { UserManagement } from '../models/user-management';
@Injectable({
  providedIn: 'root'
})
export class UserManagementService {
  constructor(private http: HttpClient) { }
  url = 'https://localhost:7197/api/usersmanagement';
    // Fetch the list of Seller
    getUsersManagement(): Observable<UserManagement[]> {
      return this.http.get<UserManagement[]>(this.url);
    }
  
    // Fetch a single Priority by id
    getUserManagement(id: number): Observable<UserManagement> {
      return this.http.get<UserManagement>(`${this.url}/${id}`);
    }
  
    // Add a new Priority
    addUserManagement(usermanagement: UserManagement): Observable<UserManagement> {
      return this.http.post<UserManagement>(this.url, usermanagement);
    }
  
    // Update an existing Priority
    updateUserManagement(usermanagement: UserManagement ): Observable<UserManagement> {
      return this.http.put<UserManagement>(`${this.url}/${usermanagement.priorityId}`, usermanagement);
    }
  
    // Delete a Priority by id
    deleteUserManagement(id: number): Observable<void> {
      return this.http.delete<void>(`${this.url}/${id}`);
    }
  }

