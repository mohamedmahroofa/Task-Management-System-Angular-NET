import { Component,OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Category } from '../models/category';
import { CategoryService } from '../services/category.service';
import { formatDate } from '@angular/common';

@Component({
  selector: 'app-category',
  templateUrl: './category.component.html',
  styleUrl: './category.component.css'
})
export class CategoryComponent implements OnInit {

  category: Category = {
    name: '',
    dateCreated: new Date(),
    isDeleted: false,
    categoryId: 0,
  };

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private categoryService: CategoryService,
  ) {}

  ngOnInit() {
    const id = this.route.snapshot.paramMap?.get('id'); // Get category id from route
  if (id) {
    // Fetch category details if id exists
    this.categoryService.getCategory(parseInt(id)).subscribe(data => {
      this.category = data;
      console.log(this.category)
    });
    }
  }
  // Save category details
  saveCategory() {
    if (this.category.categoryId) {
      this.categoryService.updateCategory(this.category).subscribe(() => {
        this.goBack();
      });
    } else {
      this.categoryService.addCategory(this.category).subscribe(() => {
        this.goBack();
      });
    }
  }
  
  
  deleteCategory() {
    if (this.category.categoryId) {
      this.categoryService.deleteCategory(this.category.categoryId).subscribe(() => {
        this.goBack();
      }, error => {
        console.error('Error deleting category:', error);
        // Handle error appropriately (e.g., show error message)
      });
    }
  }
  
  goBack() {
    this.router.navigate(['/categories']);
  }
  }