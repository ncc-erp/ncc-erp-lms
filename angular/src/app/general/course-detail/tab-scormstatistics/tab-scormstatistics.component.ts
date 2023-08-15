import { Component, OnInit, Input, Injector } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { ScormService } from '@app/services/student-service/scorm.service';

@Component({
  selector: 'app-tab-scormstatistics',
  templateUrl: './tab-scormstatistics.component.html',
  styleUrls: ['./tab-scormstatistics.component.scss']
})
export class TabScormstatisticsComponent  extends AppComponentBase implements OnInit {
  @Input() courseInstanceId: string;
  students = [];
  quizzes = [];
  totalScore: number;
  constructor(
    injector: Injector,
    private _scormService: ScormService,

  ) {
     super(injector)
   }

  ngOnInit() {
      this.getData();
  }
  getData() {
    this._scormService.GetScormStatistics(this.courseInstanceId).subscribe(result => {
      if (result.result) {
        this.students = result.result.studentsResult;
        this.quizzes = result.result.quizzes;
      }
    });
  }

}
