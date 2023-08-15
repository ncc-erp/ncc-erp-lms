import { Directive, HostListener, ElementRef, Input } from "@angular/core";

@Directive({
  selector: "[inputNumber]"
})
export class InputOnlyNumberDirective {
  private regex: RegExp = new RegExp(/^(\d{1,3})$/);
  private specialKeys: Array<string> = ["Backspace", "Tab", "End", "Home", " ", "ArrowLeft", "ArrowRight", "<", ">", "="];
  private onlyNumberKeys: Array<string> = ["Backspace", "ArrowLeft", "ArrowRight"];
  @Input('inputNumber') type: string;
  constructor(private el: ElementRef) { }
  @HostListener("keydown", ["$event"])
  onKeyDown(event: KeyboardEvent) {
    if (this.type == 'number' && this.specialKeys.indexOf(event.key) !== -1) {
      return;
    }
    if (this.type == 'mix' && this.onlyNumberKeys.indexOf(event.key) !== -1) {
      return;
    }
    let current: string = this.el.nativeElement.value;
    let next: string = current.concat(event.key);
    if (next && !String(next).match(this.regex)) {
      event.preventDefault();
    }
  }
}
