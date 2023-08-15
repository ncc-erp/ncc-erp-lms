import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import {FilterDto} from 'shared/paged-listing-component-base';

@Component({
  selector: 'app-filter',
  templateUrl: './filter.component.html',
  styleUrls: ['./filter.component.scss']
})

export class FilterComponent {

  @Input()  inputFilters: InputFilterDto[];
  @Output() outputFilter = new EventEmitter<FilterDto>();
  @Output() outputDoFilterData = new EventEmitter<any>();

  selectedPropertyName: string;
  selectedComparision: number;
  value: object;

  comparisions: ComparisionDto[];

  constructor() { }


  onPropertyNameChange(event): void{
    this.selectedPropertyName = event;
    if (this.selectedPropertyName == ''){
      this.comparisions = [];
      return;
    }
    var comps =  this.inputFilters.find(i=>i.propertyName == this.selectedPropertyName).comparisions;
    this.comparisions = [];
    comps.forEach(element => {
      var com = new ComparisionDto();
      com.id = element;
      com.name = COMPARISIONS[element];
      this.comparisions.push(com);
    });
    if (this.comparisions.length > 0) this.selectedComparision = this.comparisions[0].id;
    //console.log("this.selectedComparision=" + this.selectedComparision);
  }

  onComparisionChange(event) : void{
    this.selectedComparision = event;
  }

  addFilter(){
    //console.log("addFilter()");
    if (this.selectedPropertyName !== '' && this.selectedComparision >= 0){
      var item = new FilterDto();
      item.comparison = this.selectedComparision;
      item.propertyName = this.selectedPropertyName;
      item.value = this.value;
      item.comparisionName = COMPARISIONS[item.comparison];
      this.outputFilter.emit(item);
    }
  }

  doFilterData(){
    this.outputDoFilterData.emit();
  }

}

export class InputFilterDto {
  propertyName: string;
  comparisions: number[];
}

export class ComparisionDto {
  id: number;
  name: string;
}

export const COMPARISIONS: string[] =
 [ 'Equal',
'LessThan',
'LessThanOrEqual',
'GreaterThan',
'GreaterThanOrEqual',
'NotEqual',
'Contains',
'StartsWith',
'EndsWith',
'In']
