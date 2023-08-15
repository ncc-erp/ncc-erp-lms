import { CategoriesService } from './../../../services/systems-admin-services/categories.service';
import { Component, ViewChild, Injector, Output, EventEmitter, ElementRef, OnInit } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { AppComponentBase } from '@shared/app-component-base';
import { finalize } from 'rxjs/operators';
import { CategoryDto as EditDto, IResult, CategoryDto } from '@app/models/categories-dto';

@Component({
  selector: 'edit-category-modal',
  templateUrl: './edit-category.component.html'  
})
export class EditCategoryComponent extends AppComponentBase {

  @ViewChild('editModal') modal: ModalDirective;
  @ViewChild('modalContent') modalContent: ElementRef;

  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  active: boolean = false;
  saving: boolean = false;

  category = {} as EditDto;

  constructor(
    injector: Injector,
    private _service: CategoriesService
  ) {
    super(injector);
  }

  

  show(id: string): void {    
    this._service.getById(id).subscribe(result => {
      this.category = result.result;
      this.active = true;
      this.modal.show();
    });
  }

  onShown(): void {
    $.AdminBSB.input.activate($(this.modalContent.nativeElement));
  }

  save(): void {    
    this.saving = true;
    this._service
      .update(this.category)
      .pipe(
        finalize(() => {
          this.saving = false;
        })
      )
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
