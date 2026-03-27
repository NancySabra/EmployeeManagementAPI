import { Component, ChangeDetectorRef, ViewChild, AfterViewInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';

// Material Imports
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule, MatTableDataSource } from '@angular/material/table';
import { MatSort, MatSortModule } from '@angular/material/sort'; // Added Sort modules
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatMenuModule } from '@angular/material/menu';

// Services & Components
import { DepartmentService } from '../../../core/services/department.service';
import { AuthService } from '../../../core/services/auth.service';
import { Department } from '../../../shared/models/department';
import { NavbarComponent } from '../../../shared/navbar/navbar';
import { ConfirmDialogComponent } from '../../../shared/components/confirm-dialog';

@Component({
  selector: 'app-departments',
  standalone: true,
  imports: [
    NavbarComponent,
    MatToolbarModule,
    MatButtonModule,
    MatIconModule,
    MatTableModule,
    MatSortModule, // Added
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatMenuModule,
    MatDialogModule,
    MatSnackBarModule,
    CommonModule,
    FormsModule
  ],
  templateUrl: './departments.html',
  styleUrl: './departments.scss'
})
export class DepartmentsComponent implements AfterViewInit {
  dataSource = new MatTableDataSource<Department>([]);
  displayedColumns: string[] = ['id', 'name', 'description', 'actions'];
  
  @ViewChild(MatSort) sort!: MatSort;

  errorMessage = '';
  panelMessage = '';
  panelMessageType: 'success' | 'error' | '' = '';
  editingDepartmentId: number | null = null;
  isPanelOpen = false;

  newDepartment = {
    departmentName: '',
    description: ''
  };

  constructor(
    private departmentService: DepartmentService,
    private authService: AuthService,
    private cdr: ChangeDetectorRef,
    private dialog: MatDialog,
    private snackBar: MatSnackBar
  ) {
    this.refreshDepartments();
  }


// ... inside DepartmentsComponent

ngAfterViewInit() {
  this.configureSorting();
}

configureSorting() {
  this.dataSource.sort = this.sort;
  this.sort.disableClear = true;

  // 1. SET INITIAL STATE: This makes Department Name blue on load
  if (!this.sort.active) {
    this.sort.active = 'name';
    this.sort.direction = 'asc';
  }

  this.dataSource.sortingDataAccessor = (item: any, property: string) => {
    switch (property) {
      case 'id': return Number(item.departmentId);
      case 'name': return item.departmentName.toLowerCase();
      default: return item[property];
    }
  };

  this.sort.sortChange.subscribe(() => {
    this.cdr.detectChanges(); // Refresh colors when a new arrow is clicked
  });
}

  refreshDepartments(): void {
    this.departmentService.getAll().subscribe(data => {
      this.dataSource.data = data;
      this.configureSorting(); // Re-bind sorting after data loads
      this.cdr.detectChanges();
    });
  }






  openCreatePanel(): void {
    this.editingDepartmentId = null;
    this.newDepartment = { departmentName: '', description: '' };
    this.clearPanelMessage();
    this.isPanelOpen = true;
  }

  editDepartment(department: Department): void {
    this.editingDepartmentId = department.departmentId;
    this.newDepartment = {
      departmentName: department.departmentName,
      description: department.description || ''
    };
    this.clearPanelMessage();
    this.isPanelOpen = true;
  }

  cancelEdit(): void {
    this.editingDepartmentId = null;
    this.newDepartment = { departmentName: '', description: '' };
    this.clearPanelMessage();
    this.isPanelOpen = false;
  }

  createDepartment(): void {
    this.clearPanelMessage();
    const payload = {
      departmentName: this.newDepartment.departmentName.trim(),
      description: (this.newDepartment.description || '').trim()
    };
    if (!payload.departmentName) {
      this.setPanelError('Department name is required.');
      return;
    }
    this.departmentService.create(payload).subscribe({
      next: () => {
        this.setPanelSuccess('Department created successfully.');
        this.refreshDepartments();
      },
      error: (error) => this.setPanelError(this.extractApiErrorMessage(error, 'Failed to create department.'))
    });
  }

  updateDepartment(): void {
    this.clearPanelMessage();
    if (this.editingDepartmentId === null) return;
    const payload = {
      departmentName: this.newDepartment.departmentName.trim(),
      description: (this.newDepartment.description || '').trim()
    };
    this.departmentService.update(this.editingDepartmentId, payload).subscribe({
      next: () => {
        this.setPanelSuccess('Department updated successfully.');
        this.refreshDepartments();
        setTimeout(() => this.cancelEdit(), 300);
      },
      error: (error) => this.setPanelError(this.extractApiErrorMessage(error, 'Failed to update department.'))
    });
  }

  deleteDepartment(id: number): void {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      width: '400px',
      data: { title: 'Delete Department', message: 'Are you sure? This action cannot be undone.' }
    });
    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.departmentService.delete(id).subscribe({
          next: () => {
            this.snackBar.open('Department deleted successfully', 'Close', { duration: 3000, panelClass: ['success-snackbar'] });
            this.refreshDepartments();
          },
          error: (error) => {
            let errorMsg = error.status === 409 || error.status === 400 ? 
              'Cannot delete department: It still has employees assigned to it.' : 
              this.extractApiErrorMessage(error, 'Failed to delete department.');
            this.snackBar.open(errorMsg, 'Close', { duration: 5000, panelClass: ['error-snackbar'] });
          }
        });
      }
    });
  }

  setPanelSuccess(message: string): void {
    this.panelMessage = message;
    this.panelMessageType = 'success';
    this.cdr.detectChanges();
  }

  setPanelError(message: string): void {
    this.panelMessage = message;
    this.panelMessageType = 'error';
    this.cdr.detectChanges();
  }

  clearPanelMessage(): void {
    this.panelMessage = '';
    this.panelMessageType = '';
  }

  extractApiErrorMessage(error: any, fallback: string): string {
    if (typeof error?.error === 'string') {
      try { return JSON.parse(error.error)?.message || fallback; } catch { return error.error || fallback; }
    }
    return error?.error?.message || error?.message || fallback;
  }
}