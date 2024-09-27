// src/app/app-routing.module.ts
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './login/login.component';
import { HeaderComponent } from './header/header.component';
import { AuthGuard } from './core/security/auth.guard';
import { CategoryListComponent } from './category-list/category-list.component';
import { CategoryComponent } from './category/category.component';


const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: '', component: HeaderComponent, canActivate: [AuthGuard] }, // Example default page
  { path: 'categories', component: CategoryListComponent },
  { path: 'category/:id', component: CategoryComponent },
  { path: "category", component: CategoryComponent },
  { path: '**', redirectTo: ''},
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
