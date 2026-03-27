import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';

// Services
import { AuthService } from '../../../core/services/auth.service';

// Angular Material
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,   // Fixes [formGroup] and formControlName
    MatCardModule,          // Fixes <mat-card>
    MatFormFieldModule,     // Fixes <mat-form-field>
    MatInputModule,         // Fixes matInput
    MatButtonModule,        // Fixes mat-raised-button
    MatSnackBarModule,      // Required for the notification popup
    MatIconModule           // Useful if you want to add icons to the inputs
  ],
  templateUrl: './login.html',
  styleUrl: './login.scss'
})
export class LoginComponent {
  loginForm: FormGroup;
  loading = false;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private snackBar: MatSnackBar
  ) {
    // Professional Tip: Use Validators.required for security
    this.loginForm = this.fb.group({
      username: ['', [Validators.required, Validators.minLength(3)]],
      password: ['', [Validators.required, Validators.minLength(6)]]
    });
  }

  onSubmit(): void {
    if (this.loginForm.invalid) {
      this.snackBar.open('Please check your credentials.', 'Close', {
        duration: 3000,
        panelClass: ['error-snackbar'] // You can style this in styles.scss
      });
      return;
    }

    this.loading = true;

 
this.authService.login(this.loginForm.value).subscribe({
  next: () => {
    this.loading = false;
    this.router.navigate(['/departments']); 
  },
      error: (error) => {
        this.loading = false;
        console.error('LOGIN ERROR:', error);

        const msg =
          error?.error?.message ||
          error?.error?.Message ||
          error?.message ||
          'Invalid username or password.';

        this.snackBar.open(msg, 'Retry', {
          duration: 5000
        });
      }
    });
  }
}