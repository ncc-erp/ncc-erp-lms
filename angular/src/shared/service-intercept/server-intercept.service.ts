import { finalize } from 'rxjs/operators';
import { Observable } from 'rxjs';
import { LoadingBarService } from '@ngx-loading-bar/core';
import { Injectable } from '@angular/core';
import { HttpHandler, HttpRequest, HttpEvent } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class ServerInterceptService {

  constructor ( private loadingBar : LoadingBarService) { }
  intercept ( req: HttpRequest<any>, next: HttpHandler ): Observable<HttpEvent<any>> {
    if ( req.headers.has( 'ignoreLoadingBar' ) )
    {
      return next.handle( req.clone( { headers: req.headers.delete( 'ignoreLoadingBar' ) } ) );
    }

    const r = next.handle( req );

    let started = false;
    const responseSubscribe = r.subscribe.bind( r );
    r.subscribe = ( ...args ) => {
      this.loadingBar.start();
      started = true;
      return responseSubscribe( ...args );
    };

    return r.pipe(
      finalize( () => {
        started && this.loadingBar.complete();
      } ),
    );
  }
}
