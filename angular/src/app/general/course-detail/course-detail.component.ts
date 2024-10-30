import { Component, OnInit, Injector, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CoursesService, InvitationCourseRequestDto, RequestAssignedStudentCourseDto } from 'app/services/systems-admin-services/courses.service';
import { CategoriesService } from 'app/services/systems-admin-services/categories.service';
import { IResult as IResultCategoryDto } from 'app/models/categories-dto';
import { CourseTagService } from '@app/services/systems-admin-services/course.tag.service';
import { TagsToCourseDto } from '@app/models/course-tag-dto';
import { PagedRequestDto, PagedResultResultDto, PagedResultDto, FilterDto } from '@shared/paged-listing-component-base';
import { finalize } from 'rxjs/operators';
import { CourseAssignmentStudent, AssignedStatus } from '@shared/AppEnums';
import { QuizDto } from '@app/models/quizzes-dto';
import { GroupsService } from '@app/services/systems-admin-services/groups.service';
import { UserAssignedToCourseService, GroupsToCourseDto } from '@app/services/systems-admin-services/user.assigned.to.course.service';
import { CreateStudentComponent } from './create-student/create-student.component';
import { AppComponentBase } from '@shared/app-component-base';
import { CourseDetailResolverResult } from '@app/services/systems-admin-services/course.detail.resolver.service';
@Component({
  selector: 'app-course-detail',
  templateUrl: './course-detail.component.html',
  styleUrls: ['./course-detail.component.scss']
})
export class CourseDetailComponent extends AppComponentBase implements OnInit {
  @ViewChild('createStudentModal') createStudentModal: CreateStudentComponent;

  courseInstanceId: string;
  courseId: string;
  courseState: number;
  courseSourse: number;
  assignmentQuizzes: QuizDto[] = [];
  surveyQuizzes: QuizDto[] = [];
  resolverObj: CourseDetailResolverResult;
  dropdownListTag = [];
  selectedTagItems = [];
  dropdownSettings = {
    singleSelection: false,
    idField: 'id',
    textField: 'name',
    selectAllText: 'Select All',
    unSelectAllText: 'UnSelect All',
    itemsShowLimit: 10,
    allowSearchFilter: true
  };
  //#region  tab People
  dropdownGroupList = [];
  selectedGroupItems = [];
  dropdownGroupSettings = {
    singleSelection: false,
    idField: 'id',
    textField: 'name',
    selectAllText: 'Select All',
    unSelectAllText: 'UnSelect All',
    itemsShowLimit: 10,
    allowSearchFilter: true
  };
  // get invitation courseUsers pagging
  public pageSizeAssign = 10;
  public pageNumberAssign = 1;
  public totalPagesAssign = 1;
  public totalItemsAssign: number;
  public isTableLoadingAssign = false;

  public searchTextAssign = '';
  public filterItemsAssign: FilterDto[] = [];
  courseUsers = [];

  // get inviation courseUsers accepted pagging
  public pageSizeAssignAccepted = 10;
  public pageNumberAssignAccepted = 1;
  public totalPagesAssignAccepted = 1;
  public totalItemsAssignAccepted: number;
  public isTableLoadingAssignAccepted = false;

  public searchTextAssignAccepted = '';
  public filterItemsAssignAccepted: FilterDto[] = [];
  courseUsersAccepted = [];
  constructor(
    injector: Injector,
    private route: ActivatedRoute,
    private _courseService: CoursesService,
    private _categoryService: CategoriesService,
    private _courseTagService: CourseTagService,
    private _groupService: GroupsService,
    private _userAssignedToCoursesService: UserAssignedToCourseService,
  ) {
    super(injector);
  }

  ngOnInit() {
    // this.tabIndex = this.getSession(this.tabCourseSession) === null ? 0 : this.getSession(this.tabCourseSession);
    this.courseInstanceId = this.route.snapshot.paramMap.get('id');
    this.route.data
      .subscribe((data: { result: CourseDetailResolverResult }) => {
        this.resolverObj = data.result;
        this.courseId = this.resolverObj.courseId;

        this.getSelectedGroups();
        this.refreshAssign();
        this.refreshAssignAccepted();
      });

    // this.getCourse();
    this.getTagsByCourseInstanceId();
    this.getAllTags();
    this.getAllGroup();

  }

  //#region tab tags

  getAllTags() {
    this.dropdownListTag = [];
    this._categoryService.getAllNotPagging().subscribe((result: IResultCategoryDto) => {
      this.dropdownListTag = result.result.items;
    });
  }

  getTagsByCourseInstanceId() {
    this.selectedTagItems = [];
    this._courseTagService.getTagsByCourseInstanceId(this.courseInstanceId).subscribe((result: IResultCategoryDto) => {
      this.selectedTagItems = result.result.items;
    });
  }

  saveTags() {
    const tagsToCourse = new TagsToCourseDto();
    tagsToCourse.courseId = this.courseId;
    tagsToCourse.tags = [];
    this.selectedTagItems.forEach(element => {
      tagsToCourse.tags.push(element.id);
    });
    this._courseTagService.addTagsToCourse(tagsToCourse).subscribe(() => {
      this.notify.info(this.l('SaveCourseTagssuccessfully'));
    })
  }
  //#endregion
  onStateChange(courseState: number) {
    this.courseState = courseState;
  }

  onCourseSourseChagne(sourse: number) {
    this.courseSourse = sourse;
  }

  getAllGroup() {
    this._groupService.getAllNotPaging().subscribe((result) => {
      this.dropdownGroupList = result.result.items;
    });
  }

  getSelectedGroups() {
    this.selectedGroupItems = [];
    this._userAssignedToCoursesService.getGroupsAssignedToCourse(this.courseInstanceId).subscribe((result) => {
      this.selectedGroupItems = result.result;
    })
  }

  saveGroups() {
    const gc = new GroupsToCourseDto();
    gc.courseInstanceId = this.courseInstanceId;
    gc.groups = [];
    this.selectedGroupItems.forEach(element => {
      gc.groups.push(element[this.dropdownGroupSettings.idField]);
    });
    this._userAssignedToCoursesService.addGroupsToCourse(gc).subscribe(() => {
      this.notify.info('AddGroupsToCourseSuccessful');
      this.refreshAssign();
    })
  }

  protected listAssign(
    request: PagedRequestDto,
    pageNumber: number,
    finishedCallback: Function
  ): void {
    const invitRequest = new InvitationCourseRequestDto();
    invitRequest.courseInstanceId = this.courseInstanceId;
    invitRequest.request = request;
    this._courseService
      .getInvitationStudentByCourse(invitRequest)
      .pipe(finalize(() => {
        finishedCallback();
      }))
      .subscribe((result: PagedResultResultDto) => {
        this.courseUsers = result.result.items;
        // this.courseUsers && this.courseUsers.forEach(e => {
        //   e.statusName = this.getAssignStatusName(e.status);
        // })
        this.showPagingAssign(result.result, pageNumber);
      });
  }


  refreshAssign(): void {
    this.getDataPageAssign(this.pageNumberAssign);
  }

  public showPagingAssign(result: PagedResultDto, pageNumber: number): void {
    this.totalPagesAssign = ((result.totalCount - (result.totalCount % this.pageSizeAssign)) / this.pageSizeAssign) + 1;

    this.totalItemsAssign = result.totalCount;
    this.pageNumberAssign = pageNumber;
  }

  public getDataPageAssign(page: number): void {
    const req = new PagedRequestDto();
    req.maxResultCount = this.pageSizeAssign;
    req.skipCount = (page - 1) * this.pageSizeAssign;
    req.searchText = this.searchTextAssign;
    req.filterItems = this.filterItemsAssign;

    this.isTableLoadingAssign = true;
    this.listAssign(req, page, () => {
      this.isTableLoadingAssign = false;
    });
  }

  public reloadDataAssign() {
    this.pageNumberAssign = 1;
    this.refreshAssign();
  }

  public onAddedFilterItemAssign(item: FilterDto) {
    if (this.filterItemsAssign.findIndex(i => i.propertyName === item.propertyName && i.comparison === item.comparison) < 0) {
      this.filterItemsAssign.push(item);
    }
  }

  public deleteFilterItemAssign(item: FilterDto) {
    const index = this.filterItemsAssign
      .findIndex(i => i.comparison === item.comparison && i.propertyName === item.propertyName && i.value === item.value);
    if (index >= 0) {
      this.filterItemsAssign.splice(index, 1);
    }
  }

  private getAssignStatusName(id: number): string {
    for (let i = 0; i < CourseAssignmentStudent.ASSIGNED_STATUSES.length; i++) {
      if (CourseAssignmentStudent.ASSIGNED_STATUSES[i].id === id) {
        return CourseAssignmentStudent.ASSIGNED_STATUSES[i].name;
      }
    }
    return id.toString();
  }

  editCourseUser(id: string) {
    // this.editQuestionModal.show(id);
  }
  protected listAssignAccepted(
    request: PagedRequestDto,
    pageNumber: number,
    finishedCallback: Function
  ): void {
    const invitRequest = new RequestAssignedStudentCourseDto();
    invitRequest.courseInstanceId = this.courseInstanceId;
    invitRequest.request = request;
    invitRequest.status = AssignedStatus.Accepted;
    this._courseService
      .getAssignedStudentByCourseAndStatus(invitRequest)
      .pipe(finalize(() => {
        finishedCallback();
      }))
      .subscribe((result: PagedResultResultDto) => {
        this.courseUsersAccepted = result.result.items;
        this.showPagingAssignAccepted(result.result, pageNumber);
      });
  }
  removeCourseUser(item: any) {
    abp.message.confirm(
      `Ban có chắc chắn muốn xóa học viên ${item.studentName}`,
      "Thông báo",
      (result: boolean) => {
        if (result) {
          this._userAssignedToCoursesService.unInviteToCourse(this.courseInstanceId, item.studentId).subscribe(() => {
            abp.message.success(`Xóa thành công học viên ${item.studentName}`);
            this.refreshAssign();
          })
        }
      }
    );
  }



  refreshAssignAccepted(): void {
    this.getDataPageAssignAccepted(this.pageNumberAssignAccepted);
  }

  public showPagingAssignAccepted(result: PagedResultDto, pageNumber: number): void {
    this.totalPagesAssignAccepted
      = ((result.totalCount - (result.totalCount % this.pageSizeAssignAccepted)) / this.pageSizeAssignAccepted) + 1;

    this.totalItemsAssignAccepted = result.totalCount;
    this.pageNumberAssignAccepted = pageNumber;
  }
  public getDataPageAssignAccepted(page: number): void {
    const req = new PagedRequestDto();
    req.maxResultCount = this.pageSizeAssignAccepted;
    req.skipCount = (page - 1) * this.pageSizeAssignAccepted;
    req.searchText = this.searchTextAssignAccepted;
    req.filterItems = this.filterItemsAssignAccepted;

    this.isTableLoadingAssignAccepted = true;
    this.listAssignAccepted(req, page, () => {
      this.isTableLoadingAssignAccepted = false;     
    });
    // this.refreshAssign()
    
  }

  public reloadDataAssignAccepted() {
    this.pageNumberAssignAccepted = 1;
    this.refreshAssignAccepted();
  }

  public onAddedFilterItemAssignAccepted(item: FilterDto) {
    if (this.filterItemsAssignAccepted.findIndex(i => i.propertyName === item.propertyName && i.comparison === item.comparison) < 0) {
      this.filterItemsAssignAccepted.push(item);
    }
  }

  public deleteFilterItemAssignAccepted(item: FilterDto) {
    const index = this.filterItemsAssignAccepted
      .findIndex(i => i.comparison === item.comparison && i.propertyName === item.propertyName && i.value === item.value);
    if (index >= 0) {
      this.filterItemsAssignAccepted.splice(index, 1);
    }
  }

  createPeople() {
    this.createStudentModal.show();
  }
  //#endregion
  // onTabChanged(event) {
  //   this.setSession(this.tabCourseSession, event.index);
  // }
}

