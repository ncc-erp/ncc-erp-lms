import { PipeTransform, Pipe } from '@angular/core';
import { DatePipe } from '@angular/common';

@Pipe({ name: 'formatNumber' })
export class FormatNumberPipe extends DatePipe  implements PipeTransform {
  transform(value, args?: any): any {
    return Math.round(value * 100)/100;
  }
}
export class Constants {
  static readonly DATE_FMT = 'dd/MMM/yyyy';
  static readonly DATE_TIME_FMT = 'dd/MM/yyyy hh:mm:ss';
}
