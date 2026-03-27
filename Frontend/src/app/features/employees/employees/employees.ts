import { Component, OnInit, ChangeDetectorRef, ViewChild, AfterViewInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatTableDataSource } from '@angular/material/table';
import { MatSort, MatSortModule } from '@angular/material/sort';

// Services & Models
import { EmployeeService } from '../../../core/services/employee.service';
import { Employee } from '../../../shared/models/employee';
import { DepartmentService } from '../../../core/services/department.service';
import { Department } from '../../../shared/models/department';
import { NavbarComponent } from '../../../shared/navbar/navbar';

// Material & UI Components
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatDividerModule } from '@angular/material/divider';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatMenuModule } from '@angular/material/menu';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { ConfirmDialogComponent } from '../../../shared/components/confirm-dialog';

@Component({
  selector: 'app-employees',
  standalone: true,
  imports: [
    CommonModule, 
    FormsModule, 
    NavbarComponent, 
    MatToolbarModule,
    MatButtonModule, 
    MatIconModule, 
    MatTableModule, 
    MatSortModule, 
    MatFormFieldModule,
    MatInputModule, 
    MatSelectModule, 
    MatMenuModule, 
    MatProgressBarModule,
    MatDividerModule, 
    MatSnackBarModule,
    MatDialogModule
  ],
  templateUrl: './employees.html',
  styleUrl: './employees.scss'
})
export class EmployeesComponent implements OnInit {
  
  // FIX: Using a setter ensures the sort is linked as soon as the view is ready
  private sort!: MatSort;
  @ViewChild(MatSort) set matSort(ms: MatSort) {
    this.sort = ms;
    this.setDataSourceAttributes();
  }

  errorMessage: string = '';
  dataSource = new MatTableDataSource<Employee>([]);
  loading = false;
  departments: Department[] = [];
  
  searchTerm = '';
  selectedDepartmentId: number = 0;

  isPanelOpen = false;
  editingEmployeeId: number | null = null;
  newEmployee: any = this.initEmployee();

  constructor(
    private employeeService: EmployeeService,
    private departmentService: DepartmentService,
    private snackBar: MatSnackBar,
    private dialog: MatDialog,
    private cdr: ChangeDetectorRef 
  ) {}



  ngOnInit(): void {
    this.loadDepartments(); 
    this.refreshEmployees(); 
  }

setDataSourceAttributes() {
  this.dataSource.sort = this.sort;
  
  // ADD THIS LINE: It forces the sort to sync with the UI
  this.dataSource.sort.sortChange.subscribe(() => {
    this.cdr.detectChanges();
  });

  this.dataSource.sortingDataAccessor = (item: any, property: string) => {
    switch (property) {
      case 'id': return Number(item.employeeId);
      case 'name': return `${item.firstName} ${item.lastName}`.toLowerCase();
      case 'hireDate': return item.hireDate ? new Date(item.hireDate).getTime() : 0;
      case 'salary': return Number(item.salary) || 0;
      default: return item[property];
    }
  };
}



  refreshEmployees(): void {
    this.loading = true;
    this.cdr.detectChanges();

    this.employeeService.getAll().subscribe({
      next: (data) => {
        this.dataSource.data = data;
        this.setDataSourceAttributes(); // Re-link after data load
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        const msg = this.extractApiErrorMessage(err, 'Failed to load employees.');
        this.showNotification(msg, 'error');
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }

  loadDepartments(): void {
    this.departmentService.getAll().subscribe({
      next: (data) => {
        this.departments = data;
        this.cdr.detectChanges();
      },
      error: () => this.showNotification('Error loading departments', 'error')
    });
  }

  save(): void {
    if (!this.newEmployee.firstName || !this.newEmployee.lastName || !this.newEmployee.email) {
      this.showNotification('Please fill in all required fields.', 'error');
      return;
    }

    const request = this.editingEmployeeId 
      ? this.employeeService.update(this.editingEmployeeId, this.newEmployee)
      : this.employeeService.create(this.newEmployee);

    request.subscribe({
      next: () => {
        this.showNotification(`Employee ${this.editingEmployeeId ? 'updated' : 'created'} successfully!`, 'success');
        this.refreshEmployees();
        this.cancelEdit();
      },
      error: (err) => {
        const msg = this.extractApiErrorMessage(err, 'Operation failed');
        this.showNotification(msg, 'error');
      }
    });
  }

  deleteEmployee(id: number): void {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      width: '400px',
      data: { 
        title: 'Delete Employee',
        message: 'Are you sure you want to delete this employee? This action cannot be undone.' 
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.employeeService.delete(id).subscribe({
          next: () => {
            this.showNotification('Employee removed successfully.', 'success');
            this.refreshEmployees();
          },
          error: (err) => {
            const msg = this.extractApiErrorMessage(err, 'Delete failed');
            this.showNotification(msg, 'error');
          }
        });
      }
    });
  }

  applyFilter(): void {
    const term = this.searchTerm.trim();
    if (!term) {
      this.refreshEmployees();
      return;
    }

    this.loading = true;
    this.cdr.detectChanges();

    this.employeeService.searchEmployees(term).subscribe({
      next: (data) => {
        this.dataSource.data = data;
        this.setDataSourceAttributes(); // Ensure sort works on filtered data
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        this.loading = false;
        this.cdr.detectChanges();
        const msg = this.extractApiErrorMessage(err, 'Search failed.');
        this.showNotification(msg, 'error');
      }
    });
  }

  onDepartmentChange(): void {
    const id = Number(this.selectedDepartmentId);

    if (id === 0) {
      this.refreshEmployees();
    } else {
      this.loading = true;
      this.cdr.detectChanges();

      this.employeeService.getByDepartment(id).subscribe({
        next: (data: Employee[]) => {
          this.dataSource.data = data;
          this.setDataSourceAttributes();
          this.loading = false;
          this.cdr.detectChanges();
        },
        error: (err) => {
          this.loading = false;
          this.cdr.detectChanges();
          const msg = this.extractApiErrorMessage(err, 'Could not filter by department.');
          this.showNotification(msg, 'error');
          this.refreshEmployees();
        }
      });
    }
  }

  openCreatePanel(): void {
    this.editingEmployeeId = null;
    this.newEmployee = this.initEmployee();
    this.isPanelOpen = true;
    this.cdr.detectChanges();
  }

  editEmployee(emp: Employee): void {
    this.editingEmployeeId = emp.employeeId;
    this.newEmployee = { ...emp }; 
    if (this.newEmployee.hireDate) {
      this.newEmployee.hireDate = new Date(this.newEmployee.hireDate).toISOString().substring(0, 10);
    }
    this.isPanelOpen = true;
    this.cdr.detectChanges();
  }

  cancelEdit(): void {
    this.isPanelOpen = false;
    this.editingEmployeeId = null;
    this.cdr.detectChanges();
  }

  private initEmployee() {
    return {
      firstName: '',
      lastName: '',
      email: '',
      phoneNumber: '',
      hireDate: new Date().toISOString().substring(0, 10),
      salary: 0,
      departmentId: this.departments.length > 0 ? this.departments[0].departmentId : 0
    };
  }

  private showNotification(message: string, type: 'success' | 'error') {
    this.snackBar.open(message, 'Close', {
      duration: 3000,
      panelClass: type === 'success' ? ['success-snackbar'] : ['error-snackbar']
    });
  }

  private extractApiErrorMessage(error: any, fallback: string): string {
    if (typeof error?.error === 'string') {
      try {
        const parsed = JSON.parse(error.error);
        return parsed?.message || fallback;
      } catch {
        return error.error || fallback;
      }
    }
    return (
      error?.error?.message ||
      error?.error?.Message ||
      error?.message ||
      fallback
    );
  }
}