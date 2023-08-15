import { PipeTransform, Pipe } from '@angular/core';
import { DatePipe } from '@angular/common';

@Pipe({ name: 'converDateTime' })
export class ConvertDateTimePipe extends DatePipe  implements PipeTransform {
  transform(value, args?: any): any {
    return super.transform(value, Constants.DATE_TIME_FMT);
  }
}
export class Constants {
  static readonly DATE_FMT = 'dd/MMM/yyyy';
  static readonly DATE_TIME_FMT = 'dd/MM/yyyy hh:mm:ss';
}
