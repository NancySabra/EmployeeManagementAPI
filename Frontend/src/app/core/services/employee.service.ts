import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http'; // Added HttpParams for search
import { Observable, map } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../../shared/models/api-response';
import { Employee } from '../../shared/models/employee';

@Injectable({
  providedIn: 'root'
})
export class EmployeeService {
  // Fix 1: Unified naming to singular "Employee" to match your working endpoints
  private apiBaseUrl = `${environment.apiBaseUrl}/Employee`;
  
  constructor(private http: HttpClient) {}

  // Fix 2: Corrected the 404 path by using the unified base URL
  getByDepartment(departmentId: number): Observable<Employee[]> {
    return this.http
      .get<ApiResponse<Employee[]>>(`${this.apiBaseUrl}/department/${departmentId}`)
      .pipe(map(response => response.data)); // Added .data mapping to match your other methods
  }

  // New Fix 3: Added Search functionality for name, email, or phone
  searchEmployees(term: string): Observable<Employee[]> {
    const params = new HttpParams().set('searchTerm', term);
    return this.http
      .get<ApiResponse<Employee[]>>(`${this.apiBaseUrl}/search`, { params })
      .pipe(map(response => response.data));
  }

  getAll(): Observable<Employee[]> {
    return this.http
      .get<ApiResponse<Employee[]>>(this.apiBaseUrl)
      .pipe(map(response => response.data));
  }

  create(payload: any): Observable<ApiResponse<Employee>> {
    return this.http.post<ApiResponse<Employee>>(this.apiBaseUrl, payload);
  }

  update(id: number, payload: any): Observable<ApiResponse<Employee>> {
    return this.http.put<ApiResponse<Employee>>(`${this.apiBaseUrl}/${id}`, payload);
  }

  delete(id: number): Observable<ApiResponse<any>> {
    return this.http.delete<ApiResponse<any>>(`${this.apiBaseUrl}/${id}`);
  }
}