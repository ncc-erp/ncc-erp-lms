import { Component, OnInit, Input, Injector, Output, EventEmitter } from '@angular/core';
import { StudentService } from '@app/services/student-service/student.service';
import { QAQuestionAnswerDto, QAAnswerDto, QAAnswerInput } from '@app/models/qaquestion-dto';
import { CoursesService } from '@app/services/systems-admin-services/courses.service';
import { PagedListingComponentBase, PagedResultResultDto, PagedRequestDto } from '@shared/paged-listing-component-base';
import { finalize } from 'rxjs/operators';
import { DndDropEvent } from 'ngx-drag-drop';
import { FAQQuestionAdminDto, FAQQuestionDto } from '@app/models/faqquestion-dto';

@Component({
  selector: 'app-admin-tab-qa-detail',
  templateUrl: './tab-qa-detail.component.html',
  styleUrls: ['./tab-qa.component.scss']
})
export class TabAdminQADetailComponent extends PagedListingComponentBase<any> implements OnInit {

  @Output() isShowTabFAQList: EventEmitter<boolean> = new EventEmitter<boolean>();
  @Input() faqQuestion: FAQQuestionAdminDto;
  qaQuestions: QAQuestionAnswerDto[] = []; // Discussion
  faqQuestions: FAQQuestionDto[] = [];
  faqQuestionAdd: FAQQuestionDto = {} as FAQQuestionDto;
  isShowFaq = true;
  isShowDiscussion = true;
  isChanged = false;
  isSortFAQ = false;
  answer = '';


  constructor(injector: Injector,
    private _studentService: StudentService,
    private _coursesService: CoursesService
  ) {
    super(injector);
  }

  onDragged(item: any, list: any[]) {
    const index = list.indexOf(item);
    list.splice(index, 1);
  }
  onDrop(event: DndDropEvent, list?: any[]) {
    if (list
      && (event.dropEffect === 'copy'
        || event.dropEffect === 'move')) {

      let index = event.index;

      if (typeof index === 'undefined') {

        index = list.length;
      }

      list.splice(index, 0, event.data);
    }
  }


  onSortFAQ_change() {
    if (!this.isSortFAQ) {
      for (let index = 0; index < this.faqQuestions.length; index++) {
        this.faqQuestions[index].sequenceOrder = index + 10;
      }
      this._coursesService.UpdateFAQQuestionSequenceOrder(this.faqQuestions)
        .subscribe(data => {
          if (data.success) {
            abp.notify.success(this.l('UpdateSuccessfully'))
          }
        });
    }
  }

  ngOnInit() {
    this.pageSize = 50;
    this.getFAQQuestion();
    this.refresh();
  }

  getFAQQuestion() {
    if (this.faqQuestion.id === null) {
      return;
    }
    this._studentService.GetAllFAQQuestionByCourseId(this.faqQuestion.id, this.searchText)
      .subscribe(data => {
        if (data.success) {
          this.faqQuestions = data.result.items;
        } else {
          this.notify.info(this.l('ERROR_500'));
        }
      })
  }

  protected list(
    request: PagedRequestDto,
    pageNumber: number,
    finishedCallback: Function
  ): void {
    if (this.faqQuestion.courseInstanceId === null) {
      return;
    }
    this._studentService.GetsDiscussionByCourseInstanceIdPagging(request, this.faqQuestion.courseInstanceId)
      .pipe(finalize(() => {
        finishedCallback();
      }))
      .subscribe((result: PagedResultResultDto) => {
        this.qaQuestions = result.result.items;
        this.showPaging(result.result, pageNumber);
      });
  }

  protected delete(item: any): void {

  }
  onDeleteDiscussion_Click(item: QAQuestionAnswerDto, index: number) {
    abp.message.confirm(
      'Delete question: ' + item.title + ' ?',
      (result: boolean) => {
        if (result) {
          this._coursesService.DeleteQAQuestion(item.id)
            .subscribe(
              data => {
                if (data.success) {
                  this.isChanged = true;
                  this.qaQuestions.splice(index, 1);
                  this.isChanged = true;
                  abp.notify.success(this.l('DeleteSuccessfully'))
                }
              }
            );
        }
      }
    );
  }

  onDeleteFAQ_Click(item: FAQQuestionDto, index: number) {
    abp.message.confirm(
      'Delete FAQ: ' + item.title + ' ?',
      (result: boolean) => {
        if (result) {
          this._coursesService.DeleteFAQQuestion(item.id)
            .subscribe(
              data => {
                if (data.success) {
                  this.isChanged = true;
                  this.faqQuestions.splice(index, 1);
                  abp.notify.success(this.l('DeleteSuccessfully'))
                }
              }
            );
        }
      }
    );
  }

  onSaveFAQ_Click(item: FAQQuestionDto) {
    item.isEdit = false;
    item.courseId = item.id;
    this._coursesService.UpdateFAQQuestion(item)
      .subscribe(data => {
        if (data.success) {
          abp.notify.success(this.l('UpdateSuccessfully'))
        } else {
          this.notify.info(this.l('ERROR_400'));
        }
      });
  }

  onAddFAQ_Click() {
    this.faqQuestionAdd.courseId = this.faqQuestion.id;
    this._coursesService.CreateFAQQuestion(this.faqQuestionAdd)
      .subscribe(data => {
        if (data.success) {
          this.isChanged = true;
          this.faqQuestionAdd.id = data.result.id;
          this.faqQuestionAdd.isEdit = false;
          this.isShowFaq = true;
          this.faqQuestions.unshift(this.faqQuestionAdd);
          this.faqQuestionAdd = {} as FAQQuestionDto;
          //
          abp.notify.success(this.l('AddSuccessfully'));
        } else {
          this.notify.info(this.l('ERROR_400'));
        }
      });
  }

  onViewQuestion_Click(qaQuestion: QAQuestionAnswerDto) {
    qaQuestion.isFollow = !qaQuestion.isFollow;
    if (qaQuestion.isFollow) { // Call first time
      if (qaQuestion.isNew || qaQuestion.responses > 0) {
        // Call Add QnA to TeacherViewDiscussions
        this._studentService.getAllQAAnswerByQAQuestionId(qaQuestion.id)
          .subscribe(data => {
            if (data.success) {
              abp.notify.success(this.l('UpdateSuccessfully'));
              qaQuestion.qaAnswers = data.result.items;
            } else {
              this.notify.info(this.l('ERROR_400'));
            }
          });
        // Reset
        qaQuestion.isNew = false;
        qaQuestion.responses = 0;
      }
    }


  }

  returnPageFAQlist() { // Back button click
    this.isShowTabFAQList.emit(this.isChanged);
  }
  searchFAQnDiscussion() {
    this.getFAQQuestion();
    this.refresh();
  }

  onAddAnswerClick(qaQuestion: QAQuestionAnswerDto): void {
    if (this.answer == null || this.answer.trim() === '') {
      abp.notify.info('DataNotSave');
      return;
    }
    const tempQAAnswerDto: QAAnswerInput = new QAAnswerInput();
    tempQAAnswerDto.questionId = qaQuestion.id;
    tempQAAnswerDto.content = this.answer;
    tempQAAnswerDto.replyUserName = qaQuestion.fullName;
    this._studentService.CreateQAAnswer(tempQAAnswerDto)
      .subscribe(data => {
        if (data.success) {

          qaQuestion.qaAnswers.push(data.result);
          this.answer = '';
          this.notify.success(this.l('SaveSuccessfully'));
        } else {
          abp.notify.info('DataNotSave');
        }
      });
  }

  onReplyAnswer_Click(item: QAAnswerDto) {
    item.new_content = '<a>@' + item.fullName + '</a>:&nbsp;';
  }
  onReplySubAnswer_Click(item: QAAnswerDto, index: number) {
    item.new_content = '<a>@' + item.answers[index].fullName + '</a>:&nbsp;';
  }

  onDeleteAnswer_Click(item: QAAnswerDto, i: number, qaQuestion: QAAnswerDto[]) {
    abp.message.confirm(
      '',
      (result: boolean) => {
        if (result) {
          this._studentService.DeleteQAAnswer(item.id)
            .subscribe(data => {
              if (data.success) {
                qaQuestion.splice(i, 1);
                abp.notify.success(this.l('DeleteSuccessfully'));
              }
            });
        }
      }
    );
  }

  onDeleteSubAnswer_Click(item: QAAnswerDto, index: number) {
    abp.message.confirm(
      '',
      (result: boolean) => {
        if (result) {
          this._studentService.DeleteQAAnswer(item.answers[index].id)
            .subscribe(data => {
              if (data.success) {
                item.answers.splice(index, 1);
                abp.notify.success(this.l('DeleteSuccessfully'));
              }
            });
        }
      }
    );
  }

  onAddReplyAnswer_Click(qaAnswer: QAAnswerDto, qaQuestionId: string) {
    // item.qaAnswers.push(item.a)
    const temp: QAAnswerInput = new QAAnswerInput();
    temp.questionId = qaQuestionId;
    temp.content = qaAnswer.new_content;
    temp.responseParentId = qaAnswer.id;
    temp.replyUserName = qaAnswer.fullName;
    // temp.content = item.qaAnswers.answer
    this._studentService.CreateQAAnswer(temp)
      .subscribe(data => {
        if (data.success) {
          qaAnswer.answers.push(data.result);
          // Reset
          qaAnswer.new_content = '';
          qaAnswer.isShowAnswer = true;
          qaAnswer.isEdit = false;
        }
      });
  }

}
