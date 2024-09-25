import { Component, OnInit } from '@angular/core';
import { Category } from '../models/category';
import { CategoryService } from '../category.service';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-category',
  templateUrl: './category.component.html',
  styleUrl: './category.component.css'
})
export class CategoryComponent implements OnInit  {
  category: Category = {
    name: "",
    dateCreated: new Date(),
    categoryId: 0,
    isDeleted: false,
    
  };  

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private categoryService: CategoryService
  )
{}

ngOnInit() {
  const id = this.route.snapshot.paramMap?.get('id'); // Get priority id from route
  if (id) {
    // Fetch priority details if id exists
    this.categoryService.getCategories().subscribe(data => {
      this.category = data;
      console.log(this.category)
    });
  }
}

// Save person details
saveCategory() {
  if (this.category.categoryId) {
    this.categoryService
      .updatecategory(this.category)
      .subscribe(() => this.goBack());
  } else {
    this.categoryService.addCategory(this.category).subscribe(() => this.goBack());
  }
}

// Delete person
deleteCategory() {
  if (this.category.categoryId) {
    this.categoryService
      .deletecategory(this.category.categoryId)
      .subscribe(() => this.goBack());
  }
}

// Navigate back to persons list
goBack() {
  this.router.navigate(["/categories"]);
}
}