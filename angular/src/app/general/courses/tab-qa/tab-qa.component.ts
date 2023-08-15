import { FAQQuestionAdminDto } from './../../../models/faqquestion-dto';
import { Component, OnInit, Output, Injector, EventEmitter } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { CoursesService } from '@app/services/systems-admin-services/courses.service';

@Component({
  selector: 'app-admin-tab-qa',
  templateUrl: './tab-qa.component.html',
  styleUrls: ['./tab-qa.component.scss']
})
export class TabAdminQAComponent extends AppComponentBase implements OnInit {

  @Output() isShowTabList: EventEmitter<boolean> = new EventEmitter<boolean>();

  isShowFaqDetail = false;
  faqQuestions_public: FAQQuestionAdminDto[]; //
  faqQuestions_draft: FAQQuestionAdminDto[];
  faqQuestions_archive: FAQQuestionAdminDto[];
  faqQuestion: FAQQuestionAdminDto;

  faqStates: any[]; // = [{ id: 1, value: this.l('Published'), [] }, { id: 0, value: this.l('Draft') }, { id: 2, value: 'Archived' }];

  constructor(injector: Injector,
    private _service: CoursesService,
  ) {
    super(injector);
  }



  ngOnInit() {
    this.getListFAQQuestion();
  }

  // Draft = 0,
  // Publish = 1,
  // Archived = 2
  getListFAQQuestion() {
    this._service.GetAllCourseFAQ()
      .subscribe(data => {
        //console.log('data', data);
        if (data.success) {
          this.resetFaqQuestions();
          data.result.items.forEach(element => {
            switch (element.state) {
              // case 0:
              //   this.faqQuestions_draft.push(element);
              //   break;
              case 1:
                this.faqQuestions_public.push(element);
                break;
              case 2:
                this.faqQuestions_archive.push(element);
                break;
            }
            this.faqStates = [{ id: 1, value: this.l('Published'), items: this.faqQuestions_public },
            // { id: 0, value: this.l('Draft'), items: this.faqQuestions_draft },
            { id: 2, value: this.l('Archived'), items: this.faqQuestions_archive }]
          });
        }
      })
  }
  private resetFaqQuestions() {
    this.faqQuestions_public = [];
    this.faqQuestions_draft = [];
    this.faqQuestions_archive = [];
  }
  onChangeSite() {
    this.isShowTabList.emit(false);
  }
  onGetDetailFAQ(item: FAQQuestionAdminDto) {
    this.faqQuestion = item;
    item.isReadedQuestion = false;
    item.isReadedResponse = false;
    this.isShowFaqDetail = true;
    // Add or Update to TeacherViewDiscussions

  }
  reload(isLoad: boolean): void {
    this.isShowFaqDetail = false;
    this.getListFAQQuestion();
  }

}

export class FaqState {
  value: number;
  text: string;
  faqQuestion: FAQQuestionAdminDto[];
}
