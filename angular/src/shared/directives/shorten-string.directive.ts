import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'shortenString'
})
export class ShortenStringPipe implements PipeTransform {

  transform(value: string,agr? : number): any {
    if(value != null){
      let lenght = value.length;

      if(agr != null){
        let maxLengh = agr * 2;
        if(lenght > maxLengh){
          value = value.replace(value.substring(maxLengh,lenght),"...")
          return value;
        }
        else {
          return value;
        }
      }
    }
  }

}
