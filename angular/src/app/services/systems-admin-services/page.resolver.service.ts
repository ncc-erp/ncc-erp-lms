import { Injectable }             from '@angular/core';
import {
  Router, Resolve,
  RouterStateSnapshot,
  ActivatedRouteSnapshot
}                                 from '@angular/router';
import { Observable, of, EMPTY }  from 'rxjs';
import { mergeMap, take }         from 'rxjs/operators';
import { PageDto, IPageDto } from '@app/models/pages-dto';
import { PagesService } from './pages.service';
import { IResultObject } from '@app/models/common-dto';


@Injectable({
  providedIn: 'root',
})
export class PageResolverService implements Resolve<IPageDto> {
  constructor(private ps: PagesService, private router: Router) {}

  resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<IPageDto> | Observable<never> {
    let id = route.paramMap.get('id');

    return this.ps.getById(id).pipe(
      take(1),
      mergeMap(result => {
        if (result.result) {
          return of(result.result);
        } else { // id not found
          this.router.navigate(['/app/systems-admin/home']);
          return EMPTY;
        }
      })
    );
  }
}