import { Injectable } from '@angular/core';
import {
  Router, Resolve,
  RouterStateSnapshot,
  ActivatedRouteSnapshot
} from '@angular/router';
import { Observable, of, EMPTY } from 'rxjs';
import { mergeMap } from 'rxjs/operators';
import { CourseGroupService } from './course.group.service';
import { IResultObject } from '@app/models/common-dto';


@Injectable({
  providedIn: 'root',
})
export class AssignmentsResolverService implements Resolve<IResultObject> {
  constructor(
    private cs: CourseGroupService, private router: Router
  ) { }

  resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<IResultObject> | Observable<never> {
    const mode = route.paramMap.get('mode');
    switch (mode) {
      case 'add': {
        const courseInstanceId = route.paramMap.get('id');
        return this.cs.getCourseGroupsByCourseId(courseInstanceId).pipe(mergeMap(result => {
          if (result.result) {
            return of(result.result);
          } else { // id not found
            this.router.navigate(['/app/systems-admin/home']);
            return EMPTY;
          }
        })
        );
      }
      case 'edit': {
        const assignmentId = route.paramMap.get('id');
        return this.cs.getAllCourseGroupByAssignmentId(assignmentId).pipe(mergeMap(result => {
          if (result.result) {
            return of(result.result);
          } else { // id not found
            this.router.navigate(['/app/systems-admin/home']);
            return EMPTY;
          }
        })
        );
      }
      default: {
        break;
      }
    }
    return null;
  }
}
