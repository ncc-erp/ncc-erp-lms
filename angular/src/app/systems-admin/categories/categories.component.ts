import { EditCategoryComponent } from './edit-category/edit-category.component';
import { CreateCategoryComponent } from './create-category/create-category.component';
import { CategoryDto as EditDto} from './../../models/categories-dto';
import { CategoriesService } from './../../services/systems-admin-services/categories.service';
import { finalize } from 'rxjs/operators';
import { PagedListingComponentBase, PagedRequestDto, PagedResultResultDto, FilterDto} from 'shared/paged-listing-component-base';
import { Component, Injector, ViewChild } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import {InputFilterDto} from 'shared/filter/filter.component';

@Component({
  selector: 'app-categories',
  templateUrl: './categories.component.html',
  animations: [appModuleAnimation()]
})
export class CategoriesComponent extends PagedListingComponentBase<EditDto> {

  @ViewChild('createModal') createModal: CreateCategoryComponent;
  @ViewChild('editModal') editModal: EditCategoryComponent;
  
  categories: EditDto[] = [];    

/**
  * list comparision:
  * 0 -> 'Equal',
  * 1 -> 'LessThan',
  * 2 -> 'LessThanOrEqual',
  * 3 -> 'GreaterThan',
  * 4 -> 'GreaterThanOrEqual',
  * 5 -> 'NotEqual',
  * 6 -> 'Contains', //for string only
  * 7 -> 'StartsWith', //for string only
  * 8 -> 'EndsWith', //for string only
  * 9 -> 'In' //for list item 
 */

  public readonly FILTER_CONFIG: InputFilterDto[] = [
    {propertyName: 'Name', comparisions: [0,6,7,8]}, 
    {propertyName: 'Description', comparisions: [0,6,7,8]}];

  constructor(
    injector: Injector,
    public _service: CategoriesService) {
    super(injector);
  }

  protected list(
    request: PagedRequestDto,
    pageNumber: number,
    finishedCallback: Function
  ): void {    
    this._service
      .getAllPagging(request)
      .pipe(finalize(() => {
        finishedCallback();
      }))
      .subscribe((result: PagedResultResultDto) => {
        this.categories = result.result.items;
        this.showPaging(result.result, pageNumber);
      });
  }

  protected delete(item: EditDto): void {
    abp.message.confirm(
      "Delete category '" + item.name + "'?",
      (result: boolean) => {
        if (result) {
          this._service.delete(item.id).subscribe(() => {
            abp.notify.info('Deleted category: ' + item.name);
            this.refresh();
          });
        }
      }
    );
  }

  createItem(): void {
    this.createModal.show();
  }

  editItem(id: string): void {
    this.editModal.show(id);
  }

 

}
