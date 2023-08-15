import { HostListener } from '@angular/core';
import { Directive } from '@angular/core';

@Directive({
  selector: '[appBlockCopyPaste]'
})
export class BlockCopyPasteDirective {
  constructor() { }

  @HostListener('paste', ['$event']) blockPaste(e: KeyboardEvent) {
    e.preventDefault();
  }

  @HostListener('copy', ['$event']) blockCopy(e: KeyboardEvent) {
    e.preventDefault();
  }

  @HostListener('cut', ['$event']) blockCut(e: KeyboardEvent) {
    e.preventDefault();
  }

 /* @HostListener('keydown', ['$event']) triggerEsc(e: KeyboardEvent) {
    alert(e);
    if(e.keyCode===27){
      console.log("local esc");
      alert("esc")
    }
  }*/

   @HostListener('keydown', ['$event'])
    public onKeydownHandler(e: KeyboardEvent): void {
    if(e.keyCode===13){
      alert("enter")
    }
    }
}
