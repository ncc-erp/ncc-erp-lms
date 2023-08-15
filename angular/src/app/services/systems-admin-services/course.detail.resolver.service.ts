import { Injectable } from '@angular/core';
import {
  Router, Resolve,
  RouterStateSnapshot,
  ActivatedRouteSnapshot
} from '@angular/router';
import { Observable, of, EMPTY } from 'rxjs';
import { mergeMap, take } from 'rxjs/operators';
import { CoursesService } from './courses.service';
import { EditCourseDto } from '@app/models/courses-dto';
import { CategoriesService } from './categories.service';
import { CourseTagService } from './course.tag.service';
import { IResult } from '@app/models/common-dto';
import { UserExtralRoleService } from './user.extra.role.service';


@Injectable({
  providedIn: 'root',
})
export class CourseDetailResolverService implements Resolve<CourseDetailResolverResult> {
  constructor(
    private courseService: CoursesService,
    // private categoryService: CategoriesService,
    // private courseTagService: CourseTagService,
    private userExtraRoleServide: UserExtralRoleService,
    private router: Router
  ) { }

  resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<CourseDetailResolverResult> | Observable<never> {
    let courseInstanceId = route.paramMap.get('id');

    return this.courseService.getByCourseInstanceId(courseInstanceId).pipe(
      take(1),
      mergeMap(result => {
        if (result.result) {
          var item = {} as CourseDetailResolverResult;
          item.courseId = result.result.id;
          return this.userExtraRoleServide.GetCourseAdminsByCourseId(item.courseId).pipe(mergeMap(resultUER => {
            if (resultUER.result) {
              item.selectCoureAdmins = resultUER.result;
              return of(item);
            } else { // id not found
              this.router.navigate(['/app/systems-admin/home']);
              return EMPTY;
            }
          })
          );

        } else { // id not found
          this.router.navigate(['/app/systems-admin/home']);
          return EMPTY;
        }
      })
    );
  }
}



export class CourseDetailResolverResult {
  courseId: string;
  // dropdownListTag = [];
  // selectedTagItems = [];

  selectCoureAdmins = [];
}