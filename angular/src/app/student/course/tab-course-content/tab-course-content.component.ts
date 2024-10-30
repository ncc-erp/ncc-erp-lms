import { Component, OnInit, Input, Injector } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { ModulesService } from '@app/services/systems-admin-services/modules.service';
import { CModuleDto } from '@app/models/module-dto';
import { CourseSettingFeatualValueDto } from '@app/models/course-setting-dto';

@Component({
  selector: 'app-tab-course-content',
  templateUrl: './tab-course-content.component.html',
  styleUrls: ['./tab-course-content.component.scss']
})
export class TabCourseContentComponent extends AppComponentBase implements OnInit {
  @Input() courseInstanceId: string;
  @Input() courseId: string;
  @Input() courseSetting: CourseSettingFeatualValueDto;

  modulesPages: CModuleDto[] = [];

  constructor(
    injector: Injector,
    private _moduleService: ModulesService
  ) { super(injector)}

  ngOnInit() {
    this.getModulesPagesByCourseId();
  }

  getModulesPagesByCourseId() {
    this.modulesPages = [];    
    this._moduleService.getModulesPagesByCourseId(this.courseId).subscribe((result) => {
      this.modulesPages = result.result;
    })
  }

  expandAll(){
    this.modulesPages.forEach(e =>{      
      e.isExpanded = true;
    })
  }

  collapseAll(){
    this.modulesPages.forEach(e =>{
      e.isExpanded = false;
    })
  }

  onClick(item:CModuleDto){
    item.isExpanded = !item.isExpanded;
  }

}
