import { Routes } from '@angular/router';
import { LoginComponent } from './features/auth/login/login';
import { DepartmentsComponent } from './features/departments/departments/departments';
import { EmployeesComponent } from './features/employees/employees/employees';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'departments', component: DepartmentsComponent, canActivate: [authGuard] },
  { path: 'employees', component: EmployeesComponent, canActivate: [authGuard] }
];