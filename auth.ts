import { HttpErrorResponse, HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { API_BASE_URL } from 'src/app/app.model';
import { AuthService } from '../../modules/auth/auth.service';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  apiUrl: string = 'api/';

  constructor(
    @Inject(API_BASE_URL) private baseUrl: string,
    private _authService: AuthService) {
  }

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    let newReq = req.clone();

    const isApiUrl = req.url.startsWith(this.apiUrl);

    if (isApiUrl) {
      newReq = req.clone({ url: `${this.baseUrl}/${req.url}` });
    }

    if ((isApiUrl || isHubUrl) && this._authService.isAuthenticated) {
      newReq = newReq.clone({
        setHeaders: {
          'Content-Type': 'application/json; charset=utf-8',
          Accept: 'application/json',
          Authorization: `Bearer ${this._authService.accessToken}`
        }
      });
    }

    return next.handle(newReq)
      .pipe(catchError((error: HttpErrorResponse) => {
        return throwError(() => error.error);
      }));
  }
}
