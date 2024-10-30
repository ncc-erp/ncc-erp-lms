import { AppConsts } from './../../shared/AppConsts';
import { EventEmitter, Output, Input } from '@angular/core';
import { Component, OnInit, ViewChild } from '@angular/core';

import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ReCaptcha2Component as ReCaptcha2 } from 'ngx-captcha';

@Component({
  selector: 'app-recaptcha2',
  templateUrl: './re-captcha2.component.html'
})
export class ReCaptcha2Component implements OnInit {

  @ViewChild('captchaElem') captchaElem: ReCaptcha2;
  @Output() Captcha: EventEmitter<boolean> = new EventEmitter<boolean>();
  @Input() lang: string = 'en';
  @Input() theme: 'light' | 'dark' = 'light';
  @Input() size: 'compact' | 'normal' = 'normal';
  @Input() type: 'image' | 'audio';
  @Input() useGlobalDomain: boolean = false;

  public readonly siteKey = AppConsts.reCaptcha.siteKey;

  public aFormGroup: FormGroup;

  constructor(
    private formBuilder: FormBuilder) { }

  ngOnInit() {
    this.aFormGroup = this.formBuilder.group({
      recaptcha: ['', Validators.required]
    });
  }

  handleSuccess(captchaResponse: string): void {
    this.Captcha.emit(true);

  }
  reset() {
    this.captchaElem.reloadCaptcha();
  }
}
