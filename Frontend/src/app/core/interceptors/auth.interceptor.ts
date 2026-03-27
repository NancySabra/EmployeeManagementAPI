import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { AuthService } from '../services/auth.service';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const token = authService.getToken();

  // FIX: Ensure we only skip the token for the Login and Register calls
  const isAuthRequest = req.url.includes('/api/Auth/Login') || req.url.includes('/api/Auth/Register');

  const authReq = (token && !isAuthRequest)
    ? req.clone({
        setHeaders: {
          Authorization: `Bearer ${token}`
        }
      })
    : req;

  return next(authReq).pipe(
    catchError((error: HttpErrorResponse) => {
      // If the error is 401, only logout if we AREN'T currently on the login page
      if (error.status === 401 && !isAuthRequest) {
        console.error("Unauthorized request - Logging out");
        authService.logout();
      }
      return throwError(() => error);
    })
  );
};