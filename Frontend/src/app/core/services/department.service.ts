import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../../shared/models/api-response';
import { Department } from '../../shared/models/department';

@Injectable({
  providedIn: 'root'
})
export class DepartmentService {
  constructor(private http: HttpClient) {}

  getAll(): Observable<Department[]> {
    return this.http
      .get<ApiResponse<Department[]>>(`${environment.apiBaseUrl}/Department`)
      .pipe(map(response => response.data));
  }

  create(payload: { departmentName: string; description?: string }) {
    return this.http.post<ApiResponse<Department>>(
      `${environment.apiBaseUrl}/Department`,
      payload
    );
  }

  update(id: number, payload: { departmentName: string; description?: string }) {
    return this.http.put<ApiResponse<Department>>(
      `${environment.apiBaseUrl}/Department/${id}`,
      payload
    );
  }

  delete(id: number) {
    return this.http.delete<ApiResponse<any>>(
      `${environment.apiBaseUrl}/Department/${id}`
    );
  }
}