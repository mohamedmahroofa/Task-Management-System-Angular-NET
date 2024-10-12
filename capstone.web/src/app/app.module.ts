import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { AuthInterceptor } from './core/security/auth.interceptor';
import { HeaderComponent } from './header/header.component';
import { LoginComponent } from './login/login.component';
import { FormsModule } from '@angular/forms';
import { PriorityComponent } from './priority/priority.component';
import { PriorityListComponent } from './priority-list/priority-list.component';
import { NavigationComponent } from './navigation/navigation.component';
import { CategoryListComponent } from './category-list/category-list.component';
import { CategoryComponent } from './category/category.component';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import {MatTableModule} from '@angular/material/table';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import {MatButtonModule} from '@angular/material/button';

@NgModule({
  declarations: [
    AppComponent,
    HeaderComponent,
    LoginComponent,
    PriorityComponent,
    PriorityListComponent,
    NavigationComponent,
    CategoryListComponent,
    CategoryComponent
  ],
  imports: [  
    BrowserModule,
    AppRoutingModule,
    FormsModule,
    HttpClientModule,
    MatTableModule, MatFormFieldModule,
    MatInputModule,
    MatDatepickerModule,
    MatNativeDateModule, 
    MatButtonModule
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true },
    provideAnimationsAsync()
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
