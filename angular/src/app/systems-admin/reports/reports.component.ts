import { ButtonSelectDto, DataWriteDto, SelectOptionDto } from './../../models/report-dto';
import { OptionUserDto } from './../../models/groups-dto';
import { AppComponentBase } from '@shared/app-component-base';

import { Component, OnInit, Injector } from '@angular/core';
import { _MatChipListMixinBase } from '@angular/material';
import { DatePipe } from '@angular/common';
import { ReportInput } from '@app/models/report-dto';
import { ExportService } from '@app/services/systems-admin-services/export.service';
import { ReportService } from '@app/services/systems-admin-services/report.service';

@Component({
  selector: 'app-reports',
  templateUrl: './reports.component.html',
  styleUrls: ['./reports.component.scss']
})
export class ReportsComponent extends AppComponentBase implements OnInit {
  // user: string = '0';
  private tabSession = 'tabReportSession';
  // private pagingSession = 'pagingSession';
  users: OptionUserDto[];
  paging: ReportInput = new ReportInput();  // Data input

  tabIndex: number = 0;

  showSubPaging: boolean = false;
  subItemsPerPage: number = 1;  // entries of user per page

  buttons: ButtonSelectDto[];
  itemPerPageOption: SelectOptionDto[];

  constructor(injector: Injector, private datepipe: DatePipe,
    private _reportService: ReportService,
    private _exportService: ExportService,
  ) {
    super(injector);
  }

  ngOnInit() {

    this.constructorparams();

    this.getListUserOption();
    // Load data for current tab
    this.tabIndex = this.getSession(this.tabSession) === null ? 0 : this.getSession(this.tabSession);

    this.onButtonGroup_click(this.tabIndex); // Default tab UserLogin active

  }

  private constructorparams() {
    const now: Date = new Date();
    this.paging.FromDate = now;
    this.paging.ToDate = new Date();
    this.paging.FromDate.setDate(now.getDate() - 7);
    this.paging.MaxResultCount = 25;
    this.paging.UserId = '';

    this.buttons = [
      { index: 0, name: 'User Login', currentPage: 1, itemPerPage: 25, data: [] },
      { index: 1, name: 'User Activities', currentPage: 1, itemPerPage: 25, data: [] },
      { index: 2, name: 'Student Statistics', currentPage: 1, itemPerPage: 25, data: [] },
      { index: 3, name: 'Instructor Statistics', currentPage: 1, itemPerPage: 25, data: [] },
      { index: 4, name: 'Course Statistics', currentPage: 1, itemPerPage: 25, data: [] },
      { index: 5, name: 'Course Import/Export', currentPage: 1, itemPerPage: 25, data: [] },
    ];

    this.itemPerPageOption = [
      { id: 10, value: '10' },
      { id: 25, value: '25' },
      { id: 50, value: '50' },
      { id: 1000, value: 'All' }];
  }

  getListUserOption() {
    this._reportService.GetsUserOption()
      .subscribe((data) => {
        this.users = data.result.items;
        const tempUser: OptionUserDto = new OptionUserDto();
        tempUser.userId = '';
        tempUser.fullName = ' All -';
        this.users.unshift(tempUser);
      })
  }

  onButtonGroup_click(index: number) {
    this.tabIndex = index;
    this.setSession(this.tabSession, index);
    this.getDataForCurrentTab(index);
  }
  onButtonSearchOrFilter_click() {
    if (this.paging.UserId > 0) {
      this.showSubPaging = true;
      this.subItemsPerPage = this.buttons[this.tabIndex].itemPerPage;
    } else {
      this.showSubPaging = false;
      this.subItemsPerPage = 1;
    }
    // this.forceLoad = true;
    this.buttons.forEach(element => {
      element.isLoaded = false;
      element.currentPage = 1;
    });
    this.getDataForCurrentTab(this.tabIndex);
  }
  onButtonSort_click(data: ButtonSelectDto) {
    this.paging.SortDirection === 0 ? this.paging.SortDirection = 1 : this.paging.SortDirection = 0;
    if (data.totalPage <= this.paging.MaxResultCount) {
      if (this.paging.SortDirection === 0) {
        data.data.sort((a, b) => a.userName.localeCompare(b.userName));
      } else {
        data.data.sort((a, b) => b.userName.localeCompare(a.userName));
      }
    } else {
      this.onButtonSearchOrFilter_click();
    }
  }
  getDataForCurrentTab(tabIndex: number) {
    if (this.buttons[tabIndex].isLoaded) {
      return;
    }
    this.paging.CurrentPage = this.buttons[tabIndex].currentPage;
    this.paging.MaxResultCount = this.buttons[tabIndex].itemPerPage;
    switch (tabIndex) {
      case 0: // tabUserLogin
        this._reportService.GetsGroupUserLoginInfo(this.paging)
          .subscribe(data => {
            if (data.success) {
              this.buttons[tabIndex].totalPage = data.result.totalCount;
              this.buttons[tabIndex].isShowIpAddress = true;
              this.buttons[tabIndex].isShowCourseName = false;
              this.buttons[tabIndex].isLoaded = true;
              this.buttons[tabIndex].data = data.result.items;
            }
          })
        break;
      case 1: // tab User Activities
        this._reportService.GetsGroupUserActivitiesInfo(this.paging)
          .subscribe(data => {
            if (data.success) {
              this.buttons[tabIndex].totalPage = data.result.totalCount;
              this.buttons[tabIndex].isShowIpAddress = false;
              this.buttons[tabIndex].isShowCourseName = false;
              this.buttons[tabIndex].isLoaded = true;
              this.buttons[tabIndex].data = data.result.items;
            }
          })
        break;
      case 2: // tab Student Statistics
        this._reportService.GetsGroupStudentStatisticsInfo(this.paging)
          .subscribe(data => {
            if (data.success) {
              this.buttons[tabIndex].totalPage = data.result.totalCount;
              this.buttons[tabIndex].isShowIpAddress = false;
              this.buttons[tabIndex].isShowCourseName = true;
              this.buttons[tabIndex].isLoaded = true;
              this.buttons[tabIndex].data = data.result.items;
            }
          })
        break;
      case 3: // tab Instructor Statistics
        this._reportService.GetsGroupInstructorStatisticsInfo(this.paging)
          .subscribe(data => {
            if (data.success) {
              this.buttons[tabIndex].totalPage = data.result.totalCount;
              this.buttons[tabIndex].isShowIpAddress = false;
              this.buttons[tabIndex].isShowCourseName = true;
              this.buttons[tabIndex].isLoaded = true;
              this.buttons[tabIndex].data = data.result.items;
            }
          })
        break;
      case 4: // tabCourseStatistics
        this._reportService.GetsGroupCourseStatisticsInfo(this.paging)
          .subscribe(data => {
            if (data.success) {
              this.buttons[tabIndex].totalPage = data.result.totalCount;
              this.buttons[tabIndex].isShowIpAddress = false;
              this.buttons[tabIndex].isShowCourseName = true;
              this.buttons[tabIndex].isLoaded = true;
              this.buttons[tabIndex].data = data.result.items;
            }
          })
        break;

      case 5: // tab Course Import/Export
        this._reportService.GetsGroupCourseImportExportInfo(this.paging)
          .subscribe(data => {
            if (data.success) {
              this.buttons[tabIndex].totalPage = data.result.totalCount;
              this.buttons[tabIndex].isShowIpAddress = false;
              this.buttons[tabIndex].isShowCourseName = true;
              this.buttons[tabIndex].isLoaded = true;
              this.buttons[tabIndex].data = data.result.items;
            }
          })
        break;
    }

  }

  onExportExcel(item: ButtonSelectDto) {
    this._reportService.CreateExportOfReportAuditLog('Export Excel: ' + item.name).subscribe();
    const temps: DataWriteDto[] = [];
    for (let i = 0; i < item.data.length; i++) {
      for (let j = 0; j < item.data[i].users.length; j++) {
        const temp: DataWriteDto = new DataWriteDto;
        temp._ = (i + 1) + '.' + (j + 1);
        temp.userId = item.data[i].users[j].userId;
        temp.user_Name = item.data[i].userName;
        temp.actions = item.data[i].users[j].action;
        temp.courseName = item.data[i].users[j].courseName;
        temp.creation_Time = this.datepipe.transform(item.data[i].users[j].creationTime, 'dd/MM/yyyy - hh:mm:ss a');
        temp.ip_Address = item.data[i].users[j].ipAddress;
        temps.push(temp);
      }
    };
    this._exportService.exportAsExcelFile(temps, item.name)
  }

  onExportPDF(item: ButtonSelectDto) {
    const tabIndex = item.index;
    let tableBody: any[] = [];
    let tableContent: any = {} as any;
    this._reportService.CreateExportOfReportAuditLog('Export PDF: ' + item.name).subscribe();
    switch (tabIndex) {
      case 0: // tab User Login
        // Set Header
        tableBody = [
          ['ID',
            { text: 'User Name', style: 'tableHeader' },
            { text: 'Login DateTime', style: 'tableHeader' },
            { text: 'Actions', style: 'tableHeader' },
            { text: 'IP Address', style: 'tableHeader' },
          ]];
        for (let i = 0; i < item.data.length; i++) {
          for (let j = 0; j < item.data[i].users.length; j++) {
            const tableRow: any[] = [];
            const temp: DataWriteDto = new DataWriteDto;
            // tableRow.push(((i + 1) + '.' + (j + 1)));
            tableRow.push(item.data[i].users[j].userId.toString());
            tableRow.push(item.data[i].userName);
            tableRow.push(this.datepipe.transform(item.data[i].users[j].creationTime, 'dd/MM/yyyy - hh:mm:ss a'));
            tableRow.push(item.data[i].users[j].action);
            // tableRow.push(item.data[i].users[j].courseName);
            tableRow.push(item.data[i].users[j].ipAddress);
            tableBody.push(tableRow);
          }
        };
        tableContent = {
          style: 'tableExample',
          table: {
            headerRows: 1,
            heights: 20,
            body: tableBody
          }
        }
        this._exportService.exportAsTablePdfFile(tableContent, item.name, 'A4');

        break;

      case 1: // Tab User Activities
        // Set Header
        tableBody = [
          ['ID',
            { text: 'User Name', style: 'tableHeader' },
            { text: 'DateTime', style: 'tableHeader' },
            { text: 'Actions', style: 'tableHeader' },
          ]];
        for (let i = 0; i < item.data.length; i++) {
          for (let j = 0; j < item.data[i].users.length; j++) {
            const tableRow: any[] = [];
            const temp: DataWriteDto = new DataWriteDto;
            // tableRow.push(((i + 1) + '.' + (j + 1)));
            tableRow.push(item.data[i].users[j].userId.toString());
            tableRow.push(item.data[i].userName);
            tableRow.push(this.datepipe.transform(item.data[i].users[j].creationTime, 'dd/MM/yyyy - hh:mm:ss a'));
            tableRow.push(item.data[i].users[j].action);
            tableBody.push(tableRow);
          }
        };
        tableContent = {
          style: 'tableExample',
          table: {
            headerRows: 1,
            widths: ['*', 80, 80, 300],
            heights: 20,
            body: tableBody
          }
        }

        this._exportService.exportAsTablePdfFile(tableContent, item.name, 'A4');

        break;
      case 2: // tab Student Statistics
      case 3: // tabCourseStatistics
      case 4: // tab Instructor Statistics
      case 5: // tab Course Import/Export
        // Set Header
        tableBody = [
          ['ID',
            { text: 'User Name', style: 'tableHeader' },
            { text: 'DateTime', style: 'tableHeader' },
            { text: 'Course Name', style: 'tableHeader' },
            { text: 'Actions', style: 'tableHeader' },
          ]];
        for (let i = 0; i < item.data.length; i++) {
          for (let j = 0; j < item.data[i].users.length; j++) {
            const tableRow: any[] = [];
            const temp: DataWriteDto = new DataWriteDto;
            // tableRow.push(((i + 1) + '.' + (j + 1)));
            tableRow.push(item.data[i].users[j].userId.toString());
            tableRow.push(item.data[i].userName);
            tableRow.push(this.datepipe.transform(item.data[i].users[j].creationTime, 'dd/MM/yyyy - hh:mm:ss a'));
            tableRow.push(item.data[i].users[j].courseName);
            tableRow.push(item.data[i].users[j].action);
            tableBody.push(tableRow);
          }
        };
        tableContent = {
          style: 'tableExample',
          table: {
            headerRows: 1,
            widths: [20, 75, 75, '*', 250],
            // heights: 20,
            body: tableBody
          }
        }
        this._exportService.exportAsTablePdfFile(tableContent, item.name, 'A4', true);
        break;
    };
  }



  getDataPage(page: number) {
    this.buttons[this.tabIndex].currentPage = page;
    // Allow server load
    this.buttons[this.tabIndex].isLoaded = false;
    this.getDataForCurrentTab(this.tabIndex);
    // this.paging.CurrentPage++;
  }

}
