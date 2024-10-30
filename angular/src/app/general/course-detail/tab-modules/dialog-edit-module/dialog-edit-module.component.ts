import { Component, OnInit, Inject, Injector } from '@angular/core';
import { ModuleDto } from '@app/models/module-dto';
import { AppComponentBase } from '@shared/app-component-base';
import { MAT_DIALOG_DATA } from '@angular/material';
import { ModulesService } from '@app/services/systems-admin-services/modules.service';

@Component({
  selector: 'app-dialog-edit-module',
  templateUrl: './dialog-edit-module.component.html',
  styleUrls: ['./dialog-edit-module.component.scss']
})
export class DialogEditModuleComponent extends AppComponentBase implements OnInit {

  active = true;
  saving = false;
  module = {} as ModuleDto;
  constructor(
    @Inject(MAT_DIALOG_DATA) public data: any,
    injector: Injector,
    private _service: ModulesService) { super(injector); }

  ngOnInit() {
    this._service.getById(this.data.moduleId).subscribe(result => {
      this.module = result.result;
      this.active = true;
    });
  }
}
