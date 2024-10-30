import { IRoleDto } from './../../../models/isetting-dto';
import { IPermissionDto } from './../../../../shared/service-proxies/service-proxies';
import { AbpModule } from '@abp/abp.module';
import { AppComponentBase } from '@shared/app-component-base';
import { Component, OnInit, Injector } from '@angular/core';
import { BaseService } from '@app/services/base-service/base.service';
import { map } from 'rxjs/operators';
import { PermissonConstants, RoleConstants } from '@app/models/constant';
import { trigger } from '@angular/animations';
import { ISettingDto } from '@app/models/isetting-dto';

@Component({
  selector: 'app-tab-feature',
  templateUrl: './tab-feature.component.html',
  styleUrls: ['./tab-feature.component.scss']
})
export class TabFeatureComponent extends AppComponentBase implements OnInit {
  settingDto = { navigator: {} } as ISettingDto;
  lstNavigator: IPermissionRolesDto[] = [];
  lstReports: IPermissonDto[] = [];
  studentRoleId: number;
  adminRoleId: number;
  instructorRoleId: number;
  constructor(private inject: Injector, private _baseService: BaseService) {
    super(inject)
  }

  ngOnInit() {
    this.onGetData();
  }

  onGetData() {
    this._baseService._settingService.GetFeature().pipe(map(m => m.result)).subscribe((e) => {
      this.settingDto.navigator = e;
      this.studentRoleId = this.getRoleIdByName(RoleConstants.Student);
      this.adminRoleId = this.getRoleIdByName(RoleConstants.Admin);
      this.instructorRoleId = this.getRoleIdByName(RoleConstants.CourseAdmin);
      let text = '';
      let distinctPermissions = this.settingDto.navigator.permissionRoles.map(s => s.permissionName)
        .filter(s => !s.startsWith('Reports.'))
        .filter((value, index, self) =>
          self.indexOf(value) === index
        );
      
        //console.log('distinctPermissions', distinctPermissions);
      // Navigator View
      distinctPermissions.forEach(permissionName => {
        // let role = this.settingDto.navigator.roles.find(r => r.id === s.roleId);
        // s.roleName = role.name;
        text = permissionName;
        if (PermissonConstants.Pages_AccountCeritification === permissionName) {
          text = 'Account - Certification';
        } else if (PermissonConstants.Pages_Dashboard === permissionName) {
          text = 'Dashboard';
        } else if (PermissonConstants.Pages_Courses === permissionName) {
          text = 'Courses';
        } else if (PermissonConstants.Pages_Calendar === permissionName) {
          text = 'Calendar';
        } else if (PermissonConstants.Pages_Settings === permissionName) {
          text = 'Settings';
        } else if (PermissonConstants.Pages_UserGroups === permissionName) {
          text = 'Users & Groups';
        } else if (PermissonConstants.Pages_Report === permissionName) {
          text = 'Reports';
        }

        let lst = this.settingDto.navigator.permissionRoles;
        let student = lst.find(x => x.roleId === this.getRoleIdByName(RoleConstants.Student) && x.permissionName === permissionName);
        var admin = lst.find(x => x.roleId === this.getRoleIdByName(RoleConstants.Admin) && x.permissionName === permissionName);
        var instructor = lst.find(x => x.roleId === this.getRoleIdByName(RoleConstants.CourseAdmin) && x.permissionName === permissionName);
        this.lstNavigator.push({
          displayPName: text,
          permissionName: permissionName,
          student: student !== undefined ? student.isGranted : false,
          admin: admin !== undefined ? admin.isGranted : false,
          instructor: instructor !== undefined ? instructor.isGranted : false
        })
      })
      //console.log('lstNavigator', this.lstNavigator);
      // End Navigator View

      // Report View
      if (e.permissionRoles != null) {
        let displayPName = '';
        let sub = '';
        let permisson = e.permissionRoles.filter(s => {
          return s.permissionName.startsWith('Reports.')
        });
        permisson.forEach(p => {
          displayPName = '';
          sub = '';
          if (PermissonConstants.Reports_UserLoginAll === p.permissionName) {
            displayPName = 'User Login';
            sub = '(All)';
          } else if (PermissonConstants.Reports_UserLoginStudent === p.permissionName) {
            displayPName = 'User Login';
            sub = '(Student only)'
          } else if (PermissonConstants.Reports_UserActivitiesAll === p.permissionName) {
            displayPName = 'User Activities';
            sub = '(All)';
          } else if (PermissonConstants.Reports_UserActivitiesStudent === p.permissionName) {
            displayPName = 'User Activities';
            sub = '(Student only)';
          } else if (PermissonConstants.Reports_StudentStatistics === p.permissionName) {
            displayPName = 'Student Statistics';
            sub = '(All)';
          } else if (PermissonConstants.Reports_StudentStatisticsInCourse === p.permissionName) {
            displayPName = 'Student Statistics';
            sub = '(In Course)';
          } else if (PermissonConstants.Reports_CourseStatisticsAll === p.permissionName) {
            displayPName = 'Course Statistics';
            sub = '(All)';
          } else if (PermissonConstants.Reports_CourseStatisticsSelfCreated === p.permissionName) {
            displayPName = 'Course Statistics';
            sub = '(Self-created)';
          } else if (PermissonConstants.Reports_CourseImportExportAll === p.permissionName) {
            displayPName = 'Course Import/Export';
            sub = '(All)';
          } else if (PermissonConstants.Reports_CourseImportExportSelfCreated === p.permissionName) {
            displayPName = 'Course Import/Export';
            sub = '(Self-created)';
          } else if (PermissonConstants.InstuctorStatistics === p.permissionName) {
            displayPName = 'Instuctor Statistics';
            sub = ''
          }
          this.lstReports.push({
            permissonId: p.permissonId,
            displayPName: displayPName,
            isGranted: p.isGranted,
            permissonName: p.permissionName,
            sub: sub,
            roleId: p.roleId
          })
        });
      }
    });
  }

  onPermissionChange(permissionName, roleId, isGranted) {
    const data = {
      name: permissionName,
      roleId: roleId,
      isGranted: isGranted
    }
    this._baseService._settingService.ChangeNavigator(data).subscribe(() => {
      abp.notify.success('Changed');
      //  this.lstNavigator.find(s => s.permissionName === permissionName && roleId === )
    });
  }

  onChangeReports(id, isGranted) {
    const data = {
      id: id,
      isGranted: isGranted
    };
    this._baseService._settingService.ChangeReports(data).subscribe(() => {
      abp.notify.success('Changed');
    });
  }

  onChangeStudentDefaultView(value) {
    const data = {
      studentDefaultViewName: value
    };
    this._baseService._settingService.ChangeStudentDefaultView(data).subscribe(() => {
      abp.notify.success('Changed');
    });
  }

  onChangeDashboardDefaultView(value) {
    const data = {
      dashboardDefaultViewName: value
    };
    this._baseService._settingService.ChangeDashboardDefaultView(data).subscribe(() => {
      abp.notify.success('Changed');
    });
  }

  onChangeStudentCourseEnrollment(value) {
    const data = {
      studentCourseEnrollment: value
    };
    this._baseService._settingService.ChangeStudentEnrollment(data).subscribe(() => {
      abp.notify.success('Changed');
    });
  }

  onChangeStudentProficiency(value) {
    const data = {
      studentProficiency: value
    };
    this._baseService._settingService.ChangeStudentProficiency(data).subscribe(() => {
      abp.notify.success('Changed');
    });
  }


  getRoleIdByName(roleName: string) {
    return this.settingDto.navigator.roles.find(s => s.name === roleName).id;
  }
}

export interface IPermissionRolesDto {
  displayPName: string;
  permissionName: string;
  student: boolean; // -1 : khong cham ( ko co ban ghi), 0: x ( co ban ghi isGranted = false), 1: cham (isGranted = true)
  admin: boolean;
  instructor: boolean;

}
export interface IPermissonDto {
  permissonId: number;
  displayPName: string;
  sub: string;
  roleId: number;
  isGranted: boolean;
  permissonName: string;
}
