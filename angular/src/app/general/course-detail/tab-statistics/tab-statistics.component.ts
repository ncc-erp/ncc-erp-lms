import { PagedListingComponentBase, PagedRequestDto } from 'shared/paged-listing-component-base';
import { Component, OnInit, Injector, Input } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { StudentsToCourseDto } from '@app/services/systems-admin-services/user.assigned.to.course.service';
import { CoursesService } from '@app/services/systems-admin-services/courses.service';
import { AssignmentsService } from '@app/services/systems-admin-services/assignments.service';
import { TestAttemptDto } from '@app/services/student-service/test.attempt.service';
import { StudentFileService } from '@app/services/systems-admin-services/studentfile.service';
import { indexOf } from 'lodash';

@Component({
  selector: 'app-tab-statistics',
  templateUrl: './tab-statistics.component.html',
  styleUrls: ['./tab-statistics.component.scss']
})
export class TabStatisticsComponent extends PagedListingComponentBase<any> implements OnInit {
  protected list(request: PagedRequestDto, pageNumber: number, finishedCallback: Function): void {
    let requestBody: any = request
    requestBody.courseInstanceId = this.courseInstanceId
    this._courseService.GetCourseStatisticsPaging(request).subscribe(result => {
      if (result.result) {
        this.students = result.result.students.items;
        this.totalPage = result.result.totalPage;
        this.quizzes = result.result.quizzes;
        this.assignments = result.result.assignments;
        this.quizScores = result.result.studentQuizScores;
        this.assignmentScores = result.result.studentAssignmentScores;
        this.studentAssignments = result.result.studentAssignments;
        this.students.forEach(s => {
          s.scorePercent = s.score * 100 / s.totalScore;
          s.scorePercent = Math.round(s.scorePercent * 100) / 100;
        })
      }
      this.pageState = 0;
      this.showPaging(result.result.students, pageNumber);
    },
    );
  }
  protected delete(entity: any): void {
  }
  totalPage: number;
  totalScore: number;
  students: StudentDto[] = [];
  quizzes = [];
  assignments = [];
  studentStatistic = { quizzes: [], assignments: [] } as StatisticStudentDto;
  quizScores: number[][] = [];
  assignmentScores: number[][] = [];

  studentAssignments: StudentAssignmentDto[] = [];
  viewQuizType: string;
  pageState = 0;
  currentQuiz = {} as QuizDto;
  @Input() courseInstanceId: string;
  studentSurvey = {} as QuizDto;
  isComplete = false;
  isShowComplete = true;
  constructor(
    injector: Injector,
    private _courseService: CoursesService,
    private _assignmentService: AssignmentsService,
    private _studentFileService: StudentFileService
  ) {
    super(injector)
  }

  ngOnInit() {
    this.refresh()
  }

  

  edit(courseAssignedStudentId: string) {
    const studentObj = this.students.find(s => s.courseAssignedStudentId === courseAssignedStudentId);
    this.studentStatistic.courseAssignedStudentId = studentObj.courseAssignedStudentId;
    this.studentStatistic.studentId = studentObj.studentId;
    this.studentStatistic.nCompletedPage = studentObj.nCompletedPage;
    this.studentStatistic.studentName = studentObj.name;
    this.studentStatistic.survey = studentObj.isDoneSurvey;
    this.studentStatistic.totalPage = this.totalPage;
    this.studentStatistic.totalScore = studentObj.totalScore;
    this.studentStatistic.studentScore = studentObj.score;

    this.studentStatistic.quizzes = [];
    this.studentStatistic.assignments = [];
    if (studentObj.isDoneSurvey) {
      this.getSurvey(studentObj.courseAssignedStudentId);
    }
    for (let i = 0; i < this.students.length; i++) {
      if (this.students[i].courseAssignedStudentId === courseAssignedStudentId) {
        // quizzes
        for (let j = 0; j < this.quizzes.length; j++) {
          const score = this.quizScores[i][j];
          if (score != null) {
            const q = {
              id: this.quizzes[j].id,
              name: this.quizzes[j].name,
              quizScore: this.quizzes[j].score,
              scoreToKeepType: this.quizzes[j].scoreToKeepType,
              quizType: this.quizzes[j].quizType,
              studentScore: score
            } as QuizDto;
            this.studentStatistic.quizzes.push(q);
          }
        }

        // assignments
        for (let j = 0; j < this.assignments.length; j++) {
          const score = this.assignmentScores[i][j];
          if (score != null) {
            const sa = this.studentAssignments
            .find(s => s.courseAssignedStudentId === courseAssignedStudentId && s.assignmentSettingId === this.assignments[j].id);

            const a = {
              assignmentSettingId: this.assignments[j].id,
              id: sa != null ? sa.id : null,
              name: this.assignments[j].name,
              assignScore: this.assignments[j].score,
              studentScore: score,
              displayGrade: this.assignments[j].displayGrade,
              isAssignIndividualGrade: this.assignments[j].isAssignIndividualGrade,
              isGroupAssignment: this.assignments[j].isGroupAssignment
            } as AssignmentDto;
            this.studentStatistic.assignments.push(a);
          }
        }
      }

    }
    this.pageState = 1;
  }
  getSurvey(assignId: string) {
    this._courseService.GetStudentSurvey(assignId).subscribe(result => {
      if (result.result) {
        this.studentSurvey = {
          id: result.result.id,
          name: result.result.name,
          studentScore: 0,
          quizScore: 0,
          scoreToKeepType: result.result.scoreToKeepType,
          quizType: result.result.quizType
        }
      }
    });
  }
  save() {
    const lst: StudentAssignmentDto[] = [];
    this.studentStatistic.assignments.forEach(assign => {
      const sa = {
        assignmentSettingId: assign.assignmentSettingId,
        point: assign.studentScore,
        courseAssignedStudentId: this.studentStatistic.courseAssignedStudentId,
        isApplyForAllStudentInGroup: assign.isApplyForAllStudentInGroup
      } as StudentAssignmentDto;

      if (assign.id) {
        sa.id = assign.id;
      }
      lst.push(sa);
    })
    this._assignmentService.UpdateStudentAssignments(lst).subscribe(result => {
      this.refresh()
      this.notify.success('Successfully');
    })
    // this.isEditing = false;
  }

  cancel() {
    this.pageState = 0;
  }

  viewStudentQuiz(quiz: QuizDto) {
    this.currentQuiz = quiz;
    this.pageState = 2;
  }

  backStudentQuiz() {
    this.pageState = 1;

  }

  onQuizChangeScore(item: QuizDto) {
    for (let i = 0; i < this.students.length; i++) {
      if (this.students[i].courseAssignedStudentId === this.studentStatistic.courseAssignedStudentId) {
        // quizzes
        for (let j = 0; j < this.quizzes.length; j++) {
          if (item.id === this.quizzes[j].id) {
            this.quizScores[i][j] = item.studentScore;
            this.updateStudentScore();
            return;
          }
        }
      }
    }
  }

  updateStudentScore() {
    for (let i = 0; i < this.students.length; i++) {
      if (this.students[i].courseAssignedStudentId === this.studentStatistic.courseAssignedStudentId) {
        this.students[i].score = 0;
        // quizzes score
        for (let j = 0; j < this.quizzes.length; j++) {
          this.students[i].score += (this.quizScores[i][j] != null ? this.quizScores[i][j] : 0);
        }
        // assignment score
        for (let j = 0; j < this.assignmentScores.length; j++) {
          this.students[i].score += (this.assignmentScores[i][j] != null ? this.assignmentScores[i][j] : 0);
        }
        this.studentStatistic.studentScore = this.students[i].score;
        const scorePercent = this.students[i].score * 100 / this.students[i].totalScore;
        this.students[i].scorePercent = Math.round(scorePercent * 100) / 100;
        return;
      }
    }

  }


  viewAssignmentFile(item: AssignmentDto) {
    if (!item.studentFilePath) {
      this._studentFileService
      .getFileByAssignmentSettingIdAndStudentId(item.assignmentSettingId, this.studentStatistic.courseAssignedStudentId)
      .subscribe(result => {
        item.studentFilePath = result.result.filePath;
        window.open(this.getImageServerPath(item.studentFilePath));
      })
    } else {
      window.open(this.getImageServerPath(item.studentFilePath));
    }
  }
  viewSurvey() {
    if (this.studentSurvey != null) {
      this.viewStudentQuiz(this.studentSurvey);
    }
  }

  complete(courseAssignedStudentId, index) {
    this._courseService.completedCourse(courseAssignedStudentId).subscribe(res => {
      this.refresh()
    });
  }
}

export class StudentDto {
  studentId: number;
  name: string;
  nCompletedPage: number;
  score: number;
  totalScore: number;
  isDoneSurvey: boolean;
  scorePercent: number;
  courseAssignedStudentId: string;
  courseAssignedStudentTime: string;
  enrollCount: number;
  status: number;
}


export class StatisticStudentDto {
  courseAssignedStudentId: string;
  studentId: number;
  studentName: string;
  nCompletedPage: number;
  totalPage: number;
  studentScore: number;
  totalScore: number;
  survey: boolean;
  quizzes: QuizDto[];
  assignments: AssignmentDto[];
}

export class QuizDto {
  id: string; // quizSettingId
  name: string;
  studentScore: number;
  quizScore: number;
  scoreToKeepType: number;
  quizType: number;
}

export class AssignmentDto {
  id: string; // StudentAssignmentId
  assignmentSettingId: string; // assignmentSettingId
  name: string;
  studentScore: number;
  assignScore: number;
  displayGrade: number;
  isGroupAssignment: boolean;
  isAssignIndividualGrade: boolean;

  isApplyForAllStudentInGroup: boolean; // apply score for all students in the course group

  studentFilePath: string;
}

export class StudentAssignmentDto {
  id: string;
  courseAssignedStudentId: string;
  assignmentSettingId: string;
  point: number;
  isApplyForAllStudentInGroup: boolean;
}

export class AssignmentProgress {
  settingId: string;
  studentId: string;
  totalScore: number;
  studentScore: number;
}
