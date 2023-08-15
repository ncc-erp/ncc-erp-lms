import { AppConsts } from '@shared/AppConsts';
import { Pipe, PipeTransform } from '@angular/core';
import { DatePipe } from '@angular/common';

export class DateTimePipe {
  static timezone: string | undefined;  // set timezone of current user - Do NOT change
  static timezoneSecon: number = 0;
  static readonly DATE_FORMAT = {
    Default: 'dd/MM/yyyy HH:mm:ss', // using when pipe dateLocal not params
    // short: 'short', //  equivalent to 'M/d/yy, h:mm a' (6/15/15, 9:03 AM).
    // medium: 'medium', // equivalent to 'MMM d, y, h:mm:ss a' (Jun 15, 2015, 9:03:01 AM).
    // long: 'long', // equivalent to 'MMMM d, y, h:mm:ss a z' (June 15, 2015 at 9:03:01 AM GMT+1).
    // full: 'full', // equivalent to 'EEEE, MMMM d, y, h:mm:ss a zzzz' (Monday, June 15, 2015 at 9:03:01 AM GMT+01:00).
    // shortDate: 'shortDate', // equivalent to 'M/d/yy' (6/15/15).
    // mediumDate: 'mediumDate', // equivalent to 'MMM d, y' (Jun 15, 2015).
    // longDate: 'longDate', // equivalent to 'MMMM d, y' (June 15, 2015).
    // fullDate: 'fullDate', // equivalent to 'EEEE, MMMM d, y' (Monday, June 15, 2015).
    // shortTime: 'shortTime', // equivalent to 'h:mm a' (9:03 AM).
    // mediumTime: 'mediumTime', // equivalent to 'h:mm:ss a' (9:03:01 AM).
    // longTime: 'longTime', // equivalent to 'h:mm:ss a z' (9:03:01 AM GMT+1).
    // fullTime: 'fullTime', // equivalent to 'h:mm:ss a zzzz' (9:03:01 AM GMT+01:00).
  }
}

// Usage the same date pipe
@Pipe({
  name: 'dateLocal'
})
export class DateUtcLocalPipe extends DatePipe implements PipeTransform {
  transform(dateUTC: Date, format?: string): any {
    if (!dateUTC) { return null; }

    // Set date default format
    if (!format) {
      format = DateTimePipe.DATE_FORMAT.Default;
    }
    const timezone = DateTimePipe.timezone;
    if (timezone) {
      return super.transform(dateUTC, format, timezone);
    }
    return super.transform(dateUTC, format);
  }
}


