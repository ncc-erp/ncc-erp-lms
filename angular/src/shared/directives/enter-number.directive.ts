import { Directive, Input, HostListener, ElementRef } from '@angular/core';

@Directive({
  selector: '[enterNumber]'
})
export class EnterNumberDirective {
  private regExp: RegExp = new RegExp(/^(\d{1,10})$/);
  private specialCharacter = ['Backspace', 'Tab', 'End', 'Home', ' ', 'ArrowLeft', 'ArrowRight'];
  constructor(private el: ElementRef) { }
  @HostListener('keydown', ['$event'])
  onKeyDown(event: KeyboardEvent) {
    if (this.specialCharacter.indexOf(event.key) !== -1) {
      return;
    }
    let current: string = this.el.nativeElement.value;
    let next: string = current.concat(event.key);
    if (next && !String(next).match(this.regExp)) {
      event.preventDefault();
    }
  }
  @HostListener('paste', ['$event'])
  onPaste(event: KeyboardEvent) {
    let current: string = this.el.nativeElement.value;
    let next: string = current.concat(event.key);
    if (!String(next).match(this.regExp)) {
      event.preventDefault();
    }
  }
}
