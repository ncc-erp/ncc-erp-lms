import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { AppComponent } from './app.component';
import { AppRouteGuard } from '@shared/auth/auth-route-guard';
import { PageResolverService } from './services/systems-admin-services/page.resolver.service';
import { CourseDetailResolverService } from './services/systems-admin-services/course.detail.resolver.service';
import { QuizzesResolverService } from './services/systems-admin-services/quizzes.resolver.service';
import { AssignmentsResolverService } from './services/systems-admin-services/assignments.resolver.service';
import { QuizResolverService } from './services/systems-admin-services/quiz.resolver.service';

@NgModule({
    imports: [
        RouterModule.forChild([
            {
                path: '',
                component: AppComponent,
                children: [
                    { path: 'root-admin', loadChildren: './root-admin/root-admin.module#RootAdminModule', canActivate: [AppRouteGuard] },
                    { path: 'systems-admin', loadChildren: './systems-admin/systems-admin.module#SystemsAdminModule', canActivate: [AppRouteGuard] },
                    { path: 'course-admin', loadChildren: './course-admin/course-admin.module#CourseAdminModule' },
                    { path: 'student', loadChildren: './student/student.module#StudentModule', data: { student: true }, canActivate: [AppRouteGuard] },
                    { path: 'course/:id', loadChildren: './general/course-detail/course-detail.module#CourseDetailModule', data: { permission: 'Pages.Course' }, resolve: { result: CourseDetailResolverService } },
                    { path: 'courses', loadChildren: './general/courses/courses.module#CoursesModule', data: { permission: 'Pages.Course' }, canActivate: [AppRouteGuard] },
                    {
                        path: 'quiz/:mode/:id', loadChildren: './general/quizzes/quizzes.module#QuizzesModule',
                        canActivate: [AppRouteGuard], resolve: { courseGroups: QuizzesResolverService }
                    },
                    {
                        path: 'assignment/:mode/:id', loadChildren: './general/assignments/assignments.module#AssignmentsModule',
                        data: { permission: 'Pages.Course' },
                        canActivate: [AppRouteGuard],
                        resolve: { courseGroups: AssignmentsResolverService }
                    },
                    {
                        path: 'account',
                        data: { permission: 'Pages.Account' },
                        loadChildren: './general/user-profile/user-profile.module#UserProfileModule'
                    },
                    {
                        path: 'view-quiz/:quizId/:courseInstanceId', loadChildren: './general/view-quiz/view.quiz.module#ViewQuizModule',
                        canActivate: [AppRouteGuard], resolve: { result: QuizResolverService }
                    },
                    {
                        path: 'browsing/:mode/:id', loadChildren: './general/browsing/browsing.module#BrowsingModule',
                        canActivate: [AppRouteGuard]
                    },
                    {
                        path: 'browsing/:mode/:id/:pageId', loadChildren: './general/browsing/browsing.module#BrowsingModule',
                        canActivate: [AppRouteGuard]
                    },
                    {
                        path: 'scorm/:mode/:id/:pageId', loadChildren: './general/scorm/scorm.module#ScormModule',
                        canActivate: [AppRouteGuard]
                    },
                    {
                        path: 'scorm/:mode/:id', loadChildren: './general/scorm/scorm.module#ScormModule',
                        canActivate: [AppRouteGuard]
                    },
                    {
                        path: 'view-template/:id', loadChildren: './general/view-template/view.template.module#ViewTemplateModule',
                        canActivate: [AppRouteGuard]
                    },
                    {
                        path: 'profile/:id', loadChildren: './general/public-profile/public-profile.module#PublicProfileModule',
                        canActivate: [AppRouteGuard]
                    }

                ]
            }
        ])
    ],
    exports: [RouterModule]
})
export class AppRoutingModule { }
