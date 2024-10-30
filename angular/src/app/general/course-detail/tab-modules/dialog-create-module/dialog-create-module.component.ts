import { Component, Injector} from '@angular/core';
import { CreateModuleDto } from '@app/models/module-dto';
import { AppComponentBase } from '@shared/app-component-base';


@Component({
  selector: 'app-dialog-create-module',
  templateUrl: './dialog-create-module.component.html',
  styleUrls: ['./dialog-create-module.component.scss']
})
export class DialogCreateModuleComponent extends AppComponentBase {
  
  active: boolean = true;
  saving: boolean = false;
  module = {} as CreateModuleDto;
  constructor(    
    injector: Injector,
    ) { super(injector);}

  

}
