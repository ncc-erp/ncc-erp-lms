import {
  HttpEvent,
  HttpInterceptor,
  HttpHandler,
  HttpRequest,
  HttpErrorResponse
} from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

export class HttpErrorInterceptor implements HttpInterceptor {
  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(request)
      .pipe(
        catchError((response: HttpErrorResponse) => {
          const error = response.error;
          let errMsg = '';
          if (error instanceof ErrorEvent) {
            errMsg = `Error: ${error.message}`;
          } else {
            errMsg = error ? `${ error.error.message || response.message }` : response.message;
            abp.notify.error(errMsg);
          }
          return throwError(errMsg);
        })
      );
  }
}
