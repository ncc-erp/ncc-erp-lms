import { Component, OnInit, Input, Injector } from '@angular/core';
import { CourseSettingFeatualValueDto } from '@app/models/course-setting-dto';
import { AssignmentsService } from '@app/services/systems-admin-services/assignments.service';
import { AppComponentBase } from '@shared/app-component-base';
import { AssignmentProgress } from '@app/general/course-detail/tab-statistics/tab-statistics.component';
import { QuizzesService } from '@app/services/systems-admin-services/quizzes.service';
import { QuizDto, QuizProgressDto, StudentsProgressDto } from '@app/models/quizzes-dto';
import { StudentService } from '@app/services/student-service/student.service';

@Component({
  selector: 'app-tab-grade',
  templateUrl: './tab-grade.component.html',
  styleUrls: ['./tab-grade.component.scss']
})
export class TabGradeComponent extends AppComponentBase implements OnInit {
  @Input() courseInstanceId: string;
  @Input() courseId: string;
  @Input() courseSetting: CourseSettingFeatualValueDto;

  assignments: AssignmentProgress[] = [];
  quizzes: QuizProgressDto[] = [];
  students: StudentsProgressDto[] = [];

  totalPercent: number;

  constructor(
    injector: Injector,
    private _assignmentService: AssignmentsService,
    private _quizService: QuizzesService,
    private _studentService: StudentService,
  ) { super(injector); }

  ngOnInit() {
    this.getAssignmentProgress();
    this.getStudentsProgress();
  }
  getAssignmentProgress() {
    this._assignmentService.GetStudentAssignmentProgress(this.courseInstanceId).subscribe((result: any) => {
      this.assignments = result.result;
      this.getQuizProgress();
    });
  }
  getQuizProgress() {
    this._quizService.GetStudentQuizProgress(this.courseInstanceId).subscribe((result: any) => {
      this.quizzes = result.result;
      this.getStudentTotal();
    });
  }
  getStudentTotal() {
    this.totalPercent = 0;
    let totalScore = 0;
    let studentScore = 0;
    this.quizzes.forEach(item => {
      totalScore += item.totalScore || 0;
      studentScore += item.studentScore || 0;
    });
    this.quizzes.forEach(item => {
      totalScore += item.totalScore || 0;
      studentScore += item.studentScore || 0;
    });
    if (totalScore === 0 || studentScore === 0) {
      this.totalPercent = 0;
      return;
    }
    this.totalPercent = (studentScore / totalScore) * 100;
  }
  getStudentsProgress() {
    this._studentService.GetStudentsProgressInCourse(this.courseInstanceId).subscribe((result: any) => {

      this.students = result.result;
      this.students.forEach(item => {
        if (item.studentScore === -1) {
          item.scorePercent = '-';
          return;
        }
        if (item.totalScore === 0) {
          item.scorePercent = '100%';
          return;
        }
        item.scorePercent = String(((item.studentScore / item.totalScore) * 100).toFixed(2)) + '%';
      });
    });
  }
}
