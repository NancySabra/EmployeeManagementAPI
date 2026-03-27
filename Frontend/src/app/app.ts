import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';


import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatMenuModule } from '@angular/material/menu';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    // Add these to your imports array!
    RouterOutlet,
    MatToolbarModule, 
    MatButtonModule, 
    MatIconModule, 
    MatTableModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatMenuModule,
   
  ],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})

export class App {}