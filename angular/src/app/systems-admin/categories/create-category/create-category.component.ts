import { Component, ViewChild, Injector, Output, EventEmitter, ElementRef } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { CreateCategoryDto } from 'app/models/categories-dto';
import {CategoriesService} from 'app/services/systems-admin-services/categories.service';
import { AppComponentBase } from '@shared/app-component-base';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'create-category-modal',
  templateUrl: './create-category.component.html'  
})
export class CreateCategoryComponent extends AppComponentBase {

  @ViewChild('createModal') modal: ModalDirective;
  @ViewChild('modalContent') modalContent: ElementRef;

  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  active: boolean = false;
  saving: boolean = false;
  category = {} as  CreateCategoryDto;

  constructor(
      injector: Injector,
      private _categoryService: CategoriesService
  ) {
      super(injector);
  }

  show(): void {
      this.active = true;
      this.modal.show();      
  }

  onShown(): void {
      $.AdminBSB.input.activate($(this.modalContent.nativeElement));
  }

  save(): void {
      this.saving = true;
      this._categoryService.create(this.category)
          .pipe(finalize(() => { this.saving = false; }))
          .subscribe(() => {
              this.notify.info(this.l('SavedSuccessfully'));
              this.close();
              this.modalSave.emit(null);
          });
  }

  close(): void {
      this.active = false;
      this.modal.hide();
  }

}
