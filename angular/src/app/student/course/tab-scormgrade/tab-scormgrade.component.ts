import { Component, OnInit, Input, Injector } from '@angular/core';
import { CourseSettingFeatualValueDto } from '@app/models/course-setting-dto';
import { AppComponentBase } from '@shared/app-component-base';
import { ScormService } from '@app/services/student-service/scorm.service';

@Component({
  selector: 'app-tab-scormgrade',
  templateUrl: './tab-scormgrade.component.html',
  styleUrls: ['./tab-scormgrade.component.scss']
})
export class TabScormgradeComponent extends AppComponentBase implements OnInit {
  @Input() courseInstanceId: string;
  @Input() courseId: string;
  @Input() courseSetting: CourseSettingFeatualValueDto;
  
  studentScore = [];
  quizzes = [];
  totalPercent: number;
  studentsProgress = [];

  constructor( 
    injector: Injector,
    private _scormService: ScormService
  ) { 
    super(injector);
     }

  ngOnInit() {
    this.getQuizProgress();
  }
  getQuizProgress() {
    this._scormService.GetStudentQuizProgress(this.courseInstanceId).subscribe((result: any) => {
      this.studentScore = result.result.studentStatistic.quizzesScore;
      this.totalPercent = result.result.studentStatistic.progress;
      this.quizzes = result.result.quizzes;
      this.studentsProgress =  result.result.studentsProgress;
    });
  }
}
