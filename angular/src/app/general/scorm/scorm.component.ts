import { Component, OnInit, Injector, Input, Injectable } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { ActivatedRoute, Router } from '@angular/router';
import { CoursesService } from '@app/services/systems-admin-services/courses.service';
import { DomSanitizer, SafeUrl, SafeResourceUrl } from '@angular/platform-browser';
import { StudentProgressService } from '@app/services/student-service/student.progress.service';
import { TestAttemptService } from '@app/services/student-service/test.attempt.service';
import { ScormService } from '@app/services/student-service/scorm.service';
import { ScormCustomKey, CourseSourse, StudentProgressStatus, AssignedStatus } from '@shared/AppEnums';
import { Common } from '@shared/Common';
import { RoleConstants } from '@app/models/constant';


import { SelectionModel } from '@angular/cdk/collections';
import { FlatTreeControl, NestedTreeControl } from '@angular/cdk/tree';
import { MatTreeFlatDataSource, MatTreeFlattener, MatTreeNestedDataSource } from '@angular/material/tree';
import { BehaviorSubject } from 'rxjs';
import { AppConsts } from '@shared/AppConsts';


@Component({
  selector: 'app-scorm',
  templateUrl: './scorm.component.html',
  styleUrls: ['./scorm.component.scss']
})
export class ScormComponent extends AppComponentBase implements OnInit {

  // @Input() course: EditCourseDto; 

  courseAssignedStudentId: string;
  courseInstanceId: string;
  scormVersion: number;

  mode: string; //0 - student, 1 - admin
  loaded = false;

  soursePath: string;
  currentUrl: SafeResourceUrl;
  scormTestAttempt = {} as ScormTestAttemptDto;
  studentScorms = [] as StudentScormDto[];

  isCompletedCourse = false;
  status = null;//courseAsignedStudent Status
  quizCount = 0 as number;

  // scormNodes = [] as TodoItemNode[];

  nestedTreeControl: NestedTreeControl<TreeNode>;
  dataSource: MatTreeNestedDataSource<TreeNode>;
  isShowMenu = true;

  constructor(
    injector: Injector,
    private _route: ActivatedRoute,
    private _router: Router,
    private _courseService: CoursesService,
    private _testAttemptService: TestAttemptService,
    private _studentProgressService: StudentProgressService,
    private _scormService: ScormService,
    private sanitizer: DomSanitizer,

  ) {
    super(injector)
    this.nestedTreeControl = new NestedTreeControl<TreeNode>(this._getChildren);
    this.dataSource = new MatTreeNestedDataSource();

    // this.dataSource.data = TREE_DATA;
  }


  ngOnInit() {
    this.onResize();
    this._route.params.subscribe(e => {
      this.courseInstanceId = e.id;

      this.mode = e.mode;
      // console.log('mode', this.mode);
      // console.log('userRoleName', Common.userRoleName);
      if (this.mode != '0' && Common.userRoleName == RoleConstants.Student) {
        this._router.navigateByUrl('/app/student/dashboard');

      } else {
        this.getData();
      }

    });
  }

  height = 600;
  onResize(event?) {
    this.height = window.innerHeight - 70;
    //console.log('height', this.height);
    //$('iframe').css('height', height);
  }

  // url: SafeResourceUrl;
  private getData() {
    this._courseService.GetScormCourseAndCourseAssignedStudent(this.courseInstanceId).subscribe(item => {
      this.courseAssignedStudentId = item.result.courseAssignedStudentId;
      this.scormTestAttempt.courseAssignedStudentId = this.courseAssignedStudentId;
      this.scormVersion = item.result.sourse;
      this.status = item.result.status;
      this.isCompletedCourse = this.status === AssignedStatus.Completed;
      // this.scormVersion = CourseSourse.SCORM_12;//for test
      this.soursePath = item.result.soursePath;
      let start = (this.appSession.tenantId + '/SCORM/').length;
      this.soursePath = this.soursePath.substring(start);
      if (this.courseAssignedStudentId == null && this.mode == '0') {
        this._router.navigateByUrl("/app/student/dashboard");
        return;
      } else {
        if (this.scormVersion == CourseSourse.SCORM_12) {
          //API
          window["API"] = this;
          window["RecordTest"] = this.RecordTest;
          //Get page list
          this._scormService.GetCourseNavigation(this.courseAssignedStudentId).subscribe(s => {
            this.dataSource.data = s.result;
            // console.log(this.dataSource.data);

            if (this.dataSource.data && this.dataSource.data.length > 0) {
              this.addToListNode(this.dataSource.data[0]);
              this.nestedTreeControl.expand(this.dataSource.data[0]);
              let node = this.findUnreadPage(this.dataSource.data[0]);
              // console.log('findUnreadNode >>  ', node);
              if (node == null) node = this.findFirstPage(this.dataSource.data[0]);
              // console.log('findFirstNode >> ', node);
              if (node != null) {
                this.getConnentPage(node);
                this.loaded = true;
              } else {

              }
            }
          })

        } else if (this.scormVersion == CourseSourse.SCORM_2004) {
          window["API_1484_11"] = this;
          this.getStudentScorms(this.courseAssignedStudentId);
          let url = AppConsts.appBaseUrl + '/assets/' + this.soursePath;
          this.currentUrl = this.sanitizer.bypassSecurityTrustResourceUrl(url);
        }
      }
    })
  }

  // pageId: string;
  currentNodeIndex = 0;
  currentNode: TreeNode;
  getConnentPage(node: TreeNode) {
    // console.log('getConnentPage currentNode >>', this.currentNode);
    // console.log('getConnentPage node >>', node);
    // console.log('this.nestedNodeMap', this.nestedNodeMap);
    if (this.currentNode && this.currentNode.id == node.id) {
      return;
    };

    if (this.currentNode) {
      this.makeCurrentNodeDone(node);
    } else {
      this.currentNode = node;
      // console.log('this.nestedNodeMap', this.nestedNodeMap);
      // this.currentNode = this.transformer(this.currentNode, this.currentNode.level);
      // console.log('this.nestedNodeMap', this.nestedNodeMap);
      // console.log('this.nestedNodeMap', this.nestedNodeMap);
    }
    // console.log('this.nestedNodeMap after', this.nestedNodeMap);
    this.currentUrl = this.getCurrentUrl(node.href);
    this.loaded = true;
  }

  makeCurrentNodeDone(nextNode?: TreeNode) {
    if (this.currentNode.isDone) {
      if (nextNode)
        this.currentNode = nextNode;

      if (this.checkAllPageIsDone()) {
        this.makeCompletedCourse12();
      }
      return;
    }
    let item = { courseAssignedStudentId: this.courseAssignedStudentId, pageId: this.currentNode.id, progress: StudentProgressStatus.Completed };
    this._scormService.CreateStudentProgressScorm(item).subscribe(result => {
      // this.currentNode.isDone = true;
      // this.currentNode = this.transformer(this.currentNode, this.currentNode.level);
      // // console.log('makeCurrentNodeDone currentNode >>', this.currentNode);
      // // console.log('makeCurrentNodeDone dataSourse >>', this.dataSource.data);
      // if (nextNode)
      //   this.currentNode = nextNode;
    })

    this.currentNode.isDone = true;
    // this.currentNode = this.transformer(this.currentNode, this.currentNode.level);
    if (nextNode)
      this.currentNode = nextNode;

    if (this.checkAllPageIsDone()) {
      this.makeCompletedCourse12();
    }
  }

  listNode = [];
  addToListNode(node: TreeNode) {
    if (node.href) {
      this.listNode.push(node);
    }
    if (node.children) {
      node.children.forEach(n => {
        if (n.children && n.children.length > 0)
          this.addToListNode(n);
        else if (n.href)
          this.listNode.push(n)
      })
    }
  }

  showHideMenu() {
    this.isShowMenu = !this.isShowMenu;
  }
  nextPage() {
    let nextNode = this.findNextNode();
    if (nextNode != null)
      this.getConnentPage(nextNode);
  }

  backPage() {
    let previousNode = this.findPreviousNode();
    if (previousNode)
      this.getConnentPage(previousNode)
  }

  findNextNode(): TreeNode {
    if (this.listNode && this.listNode.length > 0 ) {
      this.currentNodeIndex = (this.currentNodeIndex + 1) % this.listNode.length;
      return this.listNode[this.currentNodeIndex];
    }
    return null;
  }

  findPreviousNode(): TreeNode {
    if (this.listNode && this.listNode.length > 0 && this.currentNodeIndex > 0) {
      this.currentNodeIndex--;
      return this.listNode[this.currentNodeIndex];
    }
    return null;
  }


  showAll() {
    //console.log('datasoruse', this.dataSource.data);
    //console.log('currentNode', this.currentNode);
  }

  private getCurrentUrl(href: string): SafeResourceUrl {
    //  SCORM/6/a695a9ab-9d8e-4cc3-7efe-08d6bf345f35/shared/launchpage.html
    ///this.soursePath.substring()


    //  http://localhost:4200/assets/SCORM/6/bf6a2f42-8a0c-40e5-f90c-08d6bda10b5b/Playing/Playing.html
    let url = this.soursePath.replace('/shared/launchpage.html', '/') + href;
    // console.log(url);
    url = AppConsts.appBaseUrl + '/assets/' + url;
    // let url = 'http://localhost:4200/assets/SCORM/5/bf6a2f42-8a0c-40e5-f90c-08d6bda10b5b/' + href;
    // console.log(url);
    return this.sanitizer.bypassSecurityTrustResourceUrl(url);
  }

  findUnreadPage(node: TreeNode): TreeNode {
    if (node.href != null) {
      if (!node.isDone) return node;
    } else {
      if (node.children && node.children.length > 0) {
        for (let i = 0; i < node.children.length; i++) {
          let n = this.findUnreadPage(node.children[i]);
          if (n != null) return n;
        }
      }
    }
    return null;
  }

  findFirstPage(node: TreeNode): TreeNode {
    if (node.href != null) {
      return node;
    } else {
      if (node.children && node.children.length > 0) {
        for (let i = 0; i < node.children.length; i++) {
          let n = this.findFirstPage(node.children[i]);
          if (n != null) return n;
        }
      }
    }
    return null;
  }

  isAllDone(node: TreeNode) {
    if (node.children && node.children.length > 0) {
      node.isDone = node.children.filter(s => s.isDone).length == node.children.length;
      return node.isDone;
    }
    return false;
  }

  isPartiallyDone(node: TreeNode) {
    if (node.children && node.children.length > 0) {
      return !this.isAllDone(node) && node.children.find(s => s.isDone) != null;
    }
    return false;
  }

  checkAllPageIsDone(): boolean {
    if (this.isCompletedCourse) return;
    // console.log('checkAllPageIsDone', this.dataSource.data);
    let n = this.findUnreadPage(this.dataSource.data[0]);
    // console.log(n);
    return n == null;
  }
  makeCompletedCourse12() {
    this._scormService.CompletedCourseScorm12(this.courseAssignedStudentId).subscribe(result => {
      this.notify.success("You have completed this course");
      this.isCompletedCourse = true;
    })
  }


  getStudentScorms(courseAssignedStudentId: string) {
    this._scormService.GetStudentScorms(courseAssignedStudentId).subscribe(result => {
      this.studentScorms = result.result;
      this.loaded = true;
      this.isCompletedCourse = this.studentScorms.find(s => s.key == ScormCustomKey.NCC_CompletedCourse) != null;
    })
  }

  createStudentScorm(key: string, value: string) {
    let item = {} as StudentScormDto;
    item.courseAssignedStudentId = this.courseAssignedStudentId;
    item.value = value;
    item.key = key;
    // console.log('createStudentScorm', item);
    this._scormService.CreateStudentScorm(item).subscribe(result => {
      let find = this.studentScorms.find(s => s.key == key);
      // console.log('this.studentScorms', this.studentScorms);
      // console.log('find', find);
      if (find) {
        find.value = result.result.value;
      } else {
        this.studentScorms.push(result.result);
      }
      // console.log('this.studentScorms after', this.studentScorms);
    })
  }


  createScormTestAttempt(scormTestAttempt: any) {
    this._scormService.CreateScormTestAttempt(scormTestAttempt).subscribe(result => {
      this.notify.info(this.l('Create Scorm TestAttempt Successfully'));
      if (scormTestAttempt.isFinal) {
        this.createStudentScorm(ScormCustomKey.NCC_CompletedCourse, 'true');
        this.isCompletedCourse = true;
      }
    })
  }

  createScorm12TestAttempt(scormTestAttempt: any) {
    this._scormService.CreateScormTestAttempt(scormTestAttempt).subscribe(result => {
      this.notify.info(this.l('Create Scorm TestAttempt Successfully'));
      // let that = window['API'] ? window['API'] : this;
      // if (that.currentNode) that.currentNode.isDone = true
      // that.currentNode = that.transformer(that.currentNode, that.currentNode.level);      
      // if (scormTestAttempt.isFinal) {
      //   this.createStudentScorm(ScormCustomKey.RMA_CompletedCourse, 'true');
      //   this.isCompletedCourse = true;
      // }

    })
  }
  //#region for display content scorm 1.2



  //  hasNestedChild = (_: number, nodeData: TreeNode) => !nodeData.type;
  hasNestedChild = (_: number, nodeData: TreeNode) => nodeData.children && nodeData.children.length > 0;

  private _getChildren = (node: TreeNode) => node.children;
  show(node: TreeNode) {

    //console.log(AppConsts.appBaseUrl);
    //console.log(node);

  }
  //#endregion

  //#region API_1484_11

  Initialize(value: string): string {
    // console.log('Initialize');
    return 'true';
  }


  Terminate(value: string): string {
    // console.log('Terminate');
    // this._router.navigateByUrl("/app/student/dashboard");
    return 'true';
  }
  GetValue(key: any): string {
    let find = this.studentScorms.find(s => s.key == key);
    let result = '';
    if (find) {
      result = find.value;
    } else {
      if (key == 'cmi.location')
        result = '';
      else if (key == 'cmi.completion_status')
        return 'unknown';
    }
    // console.log('GetValue key = ' + key + ' >> ' + result)
    return result;
  }


  SetValue(key: any, value: string): string {
    // console.log('SetValue', key + ' ' + value);
    // if (this.isCompletedCourse || this.mode != '0') return 'true';
    //student study all pages => create attendance certification
    //SetValue cmi.completion_status completed
    if (key == 'cmi.completion_status' && value == 'completed') {
      if (this.GetValue(key) != 'completed') {
        this._studentProgressService.CreateUserAttendanceCertification(this.courseInstanceId).subscribe(result => {
          this.notify.info(this.l('Create Attendance Certification Successfully'));
        })
      }

    } else if (key == 'cmi.score.raw') {
      this.scormTestAttempt.score = parseInt(value);
    } else if (key == 'cmi.score.max') {
      this.scormTestAttempt.maxScore = parseInt(value);
    } else if (key == 'cmi.success_status' && (value == 'passed' || value == 'failed')) {
      if (this.GetValue('cmi.completion_status') == 'completed') {
        this.scormTestAttempt.isFinal = true;
      } else {
        this.scormTestAttempt.isFinal = false;
      }
      this.createScormTestAttempt(this.scormTestAttempt);
    }
    if (key != 'cmi.location' || value != '0') {
      let find = this.studentScorms.find(s => s.key == key && s.value == value);
      if (!find) {
        this.createStudentScorm(key, value);
      }
    }

    return 'true';
  }
  Commit(value: string): string {
    // console.log("commit", value);
    return "true";
  }

  GetLastError = function (): string {
    // console.log('GetLastError')
    return '123';
  }

  GetErrorString(errorCode: string): string {
    // console.log('GetErrorString', errorCode)
    return '234';
  }

  GetDiagnostic(errorCode: string): string {
    // console.log('GetErrorString', errorCode)
    return '234';
  }
  //#endregion 

  //#region SCORM 1.2

  RecordTest(score: number) {
    let that = window['API'] ? window["API"] : this;
    // console.log('RecordTest score >>', score);
    // console.log('RecordTest currentNode >>', this["API"].currentNode);
    let node = that.currentNode;
    // console.log('RecordTest node >>', node);
    let scormTestAttempt = {} as ScormTestAttemptDto;
    scormTestAttempt.courseAssignedStudentId = that.courseAssignedStudentId;
    scormTestAttempt.score = score;
    scormTestAttempt.maxScore = 100;
    scormTestAttempt.name = node.name;
    scormTestAttempt.isFinal = false;
    that.createScorm12TestAttempt(scormTestAttempt);
    that.makeCurrentNodeDone();
    if (that.checkAllPageIsDone()) {
      that.makeCompletedCourse12();
    }
  }

  abc = "fdfdfd";
  LMSInitialize(value: string): string {
    // console.log('LMSInitialize', value);
    // console.log('LMSInitialize', this.abc);
    return 'true';
    // return this.Initialize(value);
  }


  LMSFinish(value: string): string {
    // console.log('LMSFinish', value);
    return 'true';
    // return this.Terminate(value);
  }

  LMSGetValue(key: any): string {
    return this.GetDiagnostic(key);
  }


  LMSSetValue(key: any, value: string): string {
    // console.log('SetValue', key + ' ' + value);
    if (this.isCompletedCourse || this.mode != '0') return 'true';
    //student study all pages => create attendance certification
    //SetValue cmi.completion_status completed
    if ((key == 'cmi.completion_status' || key == 'cmi.core.lesson_status') && (value == 'completed')) {
      if (this.GetValue(key) != 'completed') {
        this._studentProgressService.CreateUserAttendanceCertification(this.courseInstanceId).subscribe(result => {
          this.notify.info(this.l('Create Attendance Certification Successfully'));
        })
      }

    } else if (key == 'cmi.score.raw') {
      this.scormTestAttempt.score = parseInt(value);
    } else if (key == 'cmi.score.max') {
      this.scormTestAttempt.maxScore = parseInt(value);
    } else if (key == 'cmi.success_status' && (value == 'passed' || value == 'failed')) {
      if (this.GetValue('cmi.completion_status') == 'completed' || this.GetValue('cmi.core.lesson_status') == 'completed') {
        this.scormTestAttempt.isFinal = true;
      } else {
        this.scormTestAttempt.isFinal = false;
      }
      this.createScormTestAttempt(this.scormTestAttempt);
    }
    if (key != 'cmi.location' || value != '0') {
      let find = this.studentScorms.find(s => s.key == key && s.value == value);
      if (!find) {
        this.createStudentScorm(key, value);
      }
    }

    return 'true';
  }

  LMSCommit(value: string): string {
    return this.Commit(value);
  }

  LMSGetLastError = function (): string {
    return this.GetLastError();
  }

  LMSGetErrorString(errorCode: string): string {
    return this.GetErrorString(errorCode);
  }

  LMSGetDiagnostic(errorCode: string): string {
    return this.GetDiagnostic(errorCode);
  }
  //#endregion 
}


export interface ScormTestAttemptDto {
  id: string;
  name: string;
  score: number;
  maxScore: number;
  courseAssignedStudentId: string;
  isFinal: boolean;
}

export interface StudentScormDto {
  courseAssignedStudentId: string;
  key: string;
  value: string;
}


/**
 * Node for to-do item
 */
export class TreeNode {
  children: TreeNode[];
  id: string;
  name: string;
  href: string;
  level: number;
  type: any;
  isDone?: boolean;

  expandable?: boolean;
  totalPage: number;
  completedPage: number;

}


/**
 * The Json object for to-do list data.
 */
const TREE_DATA = [
  {
    "id": "root",
    "name": "Scorm 1.2",
    "href": null,
    "level": 0,
    "children": [
      {
        "id": "1",
        "name": "Playing the Game",
        "href": null,
        "level": 1,
        "checked": true,
        "children": [
          {
            "id": "playing_playing_item_tien",
            "name": "How to Play tien",
            "href": "Playing/Playing.html",
            "level": 2,
            "checked": true,
            "children": null
          },
          {
            "id": "playing_par_item",
            "name": "Par",
            "href": "Playing/Par.html",
            "level": 2,
            "checked": true,
            "children": null
          }
        ]
      }
    ]
  }
];
