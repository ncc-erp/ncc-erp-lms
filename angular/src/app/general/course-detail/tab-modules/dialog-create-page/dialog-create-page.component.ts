import { Component, Injector } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { CreatePageDto } from '@app/models/pages-dto';

@Component({
  selector: 'app-dialog-create-page',
  templateUrl: './dialog-create-page.component.html',
  styleUrls: ['./dialog-create-page.component.scss']
})
export class DialogCreatePageComponent extends AppComponentBase  {

  // @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  active: boolean = true;
  saving: boolean = false;
  page = {} as CreatePageDto;
  constructor(    
    injector: Injector,
    ) 
    { super(injector); }

}
