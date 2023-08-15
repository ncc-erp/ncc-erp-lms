import { DomSanitizer } from "@angular/platform-browser";
import {
  Component,
  OnInit,
  OnDestroy,
  Injector,
  ViewEncapsulation,
} from "@angular/core";
import { BaseService } from "@app/services/base-service/base.service";
import { ActivatedRoute, Router } from "@angular/router";
import { map } from "rxjs/operators";
import { CModuleDto } from "@app/models/module-dto";
import { PageDto, PageLinkExamDto } from "@app/models/pages-dto";
import {
  PageLinkExamType,
  StudentProgressStatus,
  TestAttemptStatus,
} from "@shared/AppEnums";
import { StudentProgressService } from "@app/services/student-service/student.progress.service";
import { QuizOptionDto } from "@app/models/quizzes-dto";
import { LocalDataDto, QuestionDto } from "@app/models/question-dto";
import { AppComponentBase } from "@shared/app-component-base";
import { CreateBookMarkDto } from "@app/models/bookmark-dto";
import { data } from "jquery";

@Component({
  selector: "app-browsing",
  templateUrl: "./browsing.component.html",
  styleUrls: ["./browsing.component.scss"],
  encapsulation: ViewEncapsulation.None,
})
export class BrowsingComponent extends AppComponentBase implements OnInit {
  // courseId: string;
  // @Oup('startQuizz') startQuizz: boolean;
  courseInstanceId: string;
  modulesPages: CModuleDto[] = [];
  // pageContent: string;
  pageId: string;
  currentPage = {} as PageDto;
  isDisableNextPage = false;
  isDisablePreviouspage = true;
  showType: string = PageLinkExamType.Page; // show page default
  quizId: string;
  assignmentSettingId: string;

  studentProgresses: StudentProgressDto[] = [];
  questions: QuestionDto[] = [];
  quizOption = {} as QuizOptionDto;
  quizSettingId: string;
  totalPage: number;
  completedPage: number;
  textSearch = "";
  originModulesPages: CModuleDto[];
  mode: string; // 0 - student, 1 - admin
  currentQuizStatus = -1;
  checkEndPage = false;
  ModuleId: any;
  titleQuiz: string = "";
  expand: boolean = false;

  pageIndex = 0;

  constructor(
    injector: Injector,
    private router: Router,
    private _base: BaseService,
    public _studentProgressService: StudentProgressService,
    public sanitizer: DomSanitizer,
    private _route: ActivatedRoute
  ) {
    super(injector);
  }

  ngOnInit() {
    this.onResize();
    this._route.params.subscribe((e) => {
      this.courseInstanceId = e.id;
      this.mode = e.mode;
      this.pageId = e.pageId ? e.pageId : null;
      if (this.mode === "0") {
        this.getStudentProgresses();
      } else {
        this.getModulesPages();
      }
    });
  }

  getStudentProgresses() {
    this._studentProgressService
      .GetStudentProgressesByCourseInstanceId(this.courseInstanceId)
      .subscribe((result) => {
        this.studentProgresses = result.result;
        this.getModulesPagesForStudent();
      });
  }

  checkModulesPagesContainPageId(pageId: string): boolean {
    for (let i = 0; i < this.modulesPages.length; i++) {
      if (this.modulesPages[i].pages.findIndex((p) => p.id === pageId)) {
        return true;
      }
    }
    return false;
  }

  getModulesPages() {
    this._base._moduleService
      .getModulesPagesByCourseInstanceId(this.courseInstanceId)
      .pipe(map((r) => r.result))
      .subscribe((result) => {
        this.modulesPages = result;
        if (this.modulesPages.length > 0) {
          for (let index = 0; index < this.modulesPages.length; index++) {
            const element = this.modulesPages[index];
            if (element.pages.length > 0) {
              this.getCurrentPage(element.pages[0].id, element.pages[0]);
              return;
            }
          }
        }
      });
  }

  getModulesPagesForStudent() {
    this._base._moduleService
      .GetModulesPagesForStudent(this.courseInstanceId)
      .pipe(map((r) => r.result))
      .subscribe((result) => {
        this.modulesPages = result;
        this.updatePagesStatus();
        // if bookmark pageId is not exist on modulesPages => set this.pageId = null
        let checkValidPage = false;
        if (this.pageId) {
          for (let i = 0; i < this.modulesPages.length; i++) {
            if (
              this.modulesPages[i].pages.findIndex(
                (p) => p.id === this.pageId
              ) >= 0
            ) {
              checkValidPage = true;
              break;
            }
          }
          if (!checkValidPage) {
            this.notify.error(
              "Bookmarked pageId " + this.pageId + " is not exist"
            );
            this.pageId = null;
          }
        }

        // find studying pageId
        if (!this.pageId) {
          const dataLocal: LocalDataDto = this.getLocal(
            this.appSession.user.id + ""
          );
          const studyingPageIds = this.studentProgresses
            .filter(
              (x) =>
                x.progress === StudentProgressStatus.Studying &&
                this.checkModulesPagesContainPageId(x.pageId)
            )
            .map((x) => x.pageId);
          if (studyingPageIds && studyingPageIds.length > 0) {
            this.pageId =
              dataLocal && studyingPageIds.indexOf(dataLocal.pageId) > -1
                ? dataLocal.pageId
                : studyingPageIds.pop();
          }
        }

        if (this.pageId) {
          this.getCurrentPage(this.pageId);
        } else {
          if (this.modulesPages.length > 0) {
            for (let index = 0; index < this.modulesPages.length; index++) {
              const element = this.modulesPages[index];
              if (element.pages.length > 0) {
                this.getCurrentPage(element.pages[0].id, element.pages[0]);
                return;
              }
            }
          }
        }
      });
  }

  public getLocal(name: string) {
    // get item from localStorage
    return JSON.parse(localStorage.getItem(name));
  }

  updatePagesStatus() {
    this.modulesPages.forEach((m) => {
      m.totalPage = 0;
      m.completedPage = 0;
      m.pages.forEach((p) => {
        const item = this.studentProgresses.find((x) => x.pageId === p.id);
        if (item) {
          p.progress = item.progress;
        }
      });
      m.totalPage = m.pages.filter(
        (s) => s.linkType === PageLinkExamType.Page
      ).length;
      m.completedPage = m.pages.filter(
        (s) =>
          s.linkType === PageLinkExamType.Page &&
          s.progress === StudentProgressStatus.Completed
      ).length;
    });
    this.originModulesPages = this.modulesPages;
    this.checkAllPageCompleted();
  }
  checkAllPageCompleted() {
    for (let i = 0; i < this.modulesPages.length; i++) {
      if (
        this.modulesPages[i].totalPage !== this.modulesPages[i].completedPage
      ) {
        return false;
      }
    }
    this._studentProgressService
      .CreateUserAttendanceCertification(this.courseInstanceId)
      .subscribe((result) => {});
  }

  createStudentProgress(progress?: number) {
    if (this.mode !== "0") {
      return;
    }
    let item = new StudentProgressDto();
    item.pageId = this.pageId;
    item.courseInstanceId = this.courseInstanceId;
    item.progress = progress ? progress : StudentProgressStatus.Studying;
    this._studentProgressService.create(item).subscribe((result) => {
      if (result.result) {
        this.studentProgresses.push(result.result);
        this.updatePagesStatus();
      } else {
      }
    });
  }

  markPageStatus(progress: number) {
    if (this.mode !== "0") {
      return;
    }
    const item = this.studentProgresses.find((x) => x.pageId === this.pageId);
    if (!item) {
      this.createStudentProgress(progress);
      return;
    } else {
      if (
        item.progress === StudentProgressStatus.Completed &&
        (!this.currentPage.links || this.currentPage.links.length === 0)
      ) {
        return;
      }
      item.courseInstanceId = this.courseInstanceId;
      item.progress = progress;
      this._studentProgressService.update(item).subscribe((result) => {
        if (result.result) {
          for (let i = 0; i < this.studentProgresses.length; i++) {
            if (this.studentProgresses[i].id === result.result.id) {
              this.studentProgresses[i].progress = result.result.progress;
              // this.completedPageIds = this.studentProgresses.filter(s => s.progress == StudentProgressStatus.Completed).map(x => x.id);
              this.updatePagesStatus();
              break;
            }
          }
        } else {
        }
      });
    }
  }

  getCurrentPage(id: string, modulePage?) {
    // debugger;
    // for (let i = 0; i < this.modulesPages.length; i++) {
    //   for (let j = 0; j < this.modulesPages[i].pages.length; j++) {
    //     if (this.modulesPages[i].pages[j].id === id) {git
    //       this.pageIndex = i + 1;
    //       if (this.pageIndex < this.modulesPages.length) {
    //         this.checkEndPage = false;
    //       } else {
    //         this.checkEndPage = false;
    //       }
    //       break;
    //     }
    //   }
    // }
    let message = `Select page ${modulePage ? modulePage.name : ""}`;
    let args = {
      modulePage: { ...modulePage },
    };
    args.modulePage.pages = JSON.stringify(args.modulePage.pages);
    this.logStudentProcessToSentry(message, args);

    const modulesLength = this.modulesPages.length;
    let index = 0;
    let lastPageIndex = 0;
    for (let i = 0; i < modulesLength; i++) {
      lastPageIndex += this.modulesPages[i].pages.length;
    }
    loop1: for (let i = 0; i < modulesLength; i++) {
      const pagesLength = this.modulesPages[i].pages.length;
      for (let j = 0; j < pagesLength; j++) {
        if (this.modulesPages[i].pages[j].id === id) {
          break loop1;
        }
        index++;
      }
    }
    this.pageIndex = index;
    if (this.pageIndex === lastPageIndex - 1) {
      this.checkEndPage = true;
    } else {
      this.checkEndPage = false;
    }

    this._studentProgressService.showMess = true;
    if (this.currentPage && this.currentPage.id === id) {
      return;
    }
    if (this.currentQuizStatus === TestAttemptStatus.Testing) {
      abp.message.error(
        this.l("Please complete the quiz before changing page")
      );
      return;
    }
    this.currentQuizStatus = -1;
    this.pageId = id;
    if (!this.studentProgresses.find((x) => x.pageId === this.pageId)) {
      this.createStudentProgress();
    }
    this._base._pageService
      .getForStudentById(id)
      .pipe(map((r) => r.result))
      .subscribe((e) => {
        this.currentPage = e;
        this.ModuleId = e.moduleId;
        this.titleQuiz = e.name;
        for (let i = 0; i < this.modulesPages.length; i++) {
          if (
            this.modulesPages[i].pages.findIndex((p) => p.id === this.pageId) >=
            0
          ) {
            const page = this.modulesPages[i].pages.find((p) => p.id === id);
            page.files = e.files;
            break;
          }
        }
        if (this.currentPage.links.length > 0) {
          const pageLinkExam = this.currentPage.links[0];
          this.showType = pageLinkExam.linkType;
          if (pageLinkExam.linkType === PageLinkExamType.Assignment) {
            this.assignmentSettingId = pageLinkExam.linkId;
          } else {
            this.quizSettingId = pageLinkExam.linkId;
          }
        } else {
          this.showType = PageLinkExamType.Page;
        }
      });
    window.scroll({
      top: 0,
      left: 0,
      behavior: "smooth",
    });
  }

  bookmark() {
    if (this.mode !== "0") {
      return;
    }
    const item = {
      pageId: this.pageId,
      courseInstanceId: this.courseInstanceId,
    } as CreateBookMarkDto;
    this._base._pageService.bookmarkPage(item).subscribe((result) => {
      if (result.result) {
        this.currentPage.bookmarked = true;
      }
    });
  }

  unBookmark() {
    this._base._pageService.unBookmarkPage(this.pageId).subscribe((result) => {
      this.currentPage.bookmarked = false;
    });
  }

  searchModulesPages() {
    if (this.textSearch.trim() !== "") {
      this.modulesPages = [];
      this.originModulesPages.forEach((m) => {
        const pages = m.pages.filter((p) =>
          p.name.toLowerCase().includes(this.textSearch.toLowerCase())
        );
        if (pages.length > 0) {
          m.pages = pages;
          this.modulesPages.push(m);
        }
      });
    } else {
      this.modulesPages = this.originModulesPages;
    }
  }

  nextPage() {
    this._studentProgressService.showMess = true;
    const nextPageId = this.getNextPage();
    if (nextPageId && nextPageId !== "") {
      this.markPageStatus(StudentProgressStatus.Completed);
      this.getCurrentPage(nextPageId);
    } else if (this.pageId.length > 2) {
      this.markPageStatus(StudentProgressStatus.Completed);
    }
  }

  getNextPage(): string {
    // debugger;
    console.log(this.pageIndex);
    this.pageIndex = this.pageIndex + 1;
    let result = "";
    const lastPageIndex = this.modulesPages.reduce((index, modulePage) => {
      const pageLength = modulePage.pages.length;
      for (let i = 0; i < pageLength; i++) {
        index++;
        if (index === this.pageIndex) {
          result = modulePage.pages[i].id;
        }
      }
      return index;
    }, -1);
    if (this.pageIndex === lastPageIndex) {
      this.checkEndPage = true;
    }

    return result;
  }

  onResize(event?) {
    // let height = window.innerHeight - 50;
    const width = window.innerWidth;
    if (width > 800) {
      // $('.main-content1').css('min-height', height)
    } else {
      // $('.main-content1').css('min-height', '')
    }
  }
  onQuizStatusChange(status: number) {
    this.currentQuizStatus = status;

    if (this.currentQuizStatus === TestAttemptStatus.Marking) {
      this.markPageStatus(StudentProgressStatus.Completed);
    } else if (this.currentQuizStatus === TestAttemptStatus.Testing) {
      this.markPageStatus(StudentProgressStatus.Studying);
    }
  }
  // end for quiz component
  downloadAttachment(filePath: string) {
    const fullPath = this.getImageServerPath(filePath);
    window.open(fullPath);
  }

  logStudentBackToCourse() {
    let message = `Back to course page`;
    this.logStudentProcessToSentry(message, null);
  }
}

export class StudentProgressDto {
  id: string;
  pageId: string;
  courseInstanceId: string;
  progress: number;
}
