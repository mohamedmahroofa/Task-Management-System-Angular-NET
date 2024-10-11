import { Injectable } from '@angular/core';
import { Category } from './models/category';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CategoryService {

  private url = "http://localhost:3000/categorys"; 
  constructor(private http: HttpClient) { }

  getCat(): Observable<Category[]> {
    return this.http.get<Category[]>(this.url);
  }
  getCategory(id: number): Observable<Category> {
    return this.http.get<Category>(`${this.url}/${id}`);
  }

  addCategory(category: Category): Observable<Category> {
    return this.http.post<Category>(this.url, category);
  }

  // Update an existing category
  updatecategory(category: Category): Observable<Category> {
    return this.http.put<Category>(`${this.url}/${category.id}`, category);
  }

  // Delete a category by id
  deletecategory(id: number): Observable<void> {
    return this.http.delete<void>(`${this.url}/${id}`);
  }

}
