import { FormControl, FormGroup } from '@angular/forms';
export interface ValidationResult {
  [key: string]: boolean;
}

export class PasswordValidator {

  public static hasNumber(control: FormControl): ValidationResult {
    const valid = /\d/.test(control.value);
    if (!valid) {
      return { hasNumber: true };
    }
    return null;
  }
  public static hasUpper(control: FormControl): ValidationResult {
    const valid = /[A-Z]/.test(control.value);
    if (!valid) {
      return { hasUpper: true };
    }
    return null;
  }
  public static hasLower(control: FormControl): ValidationResult {
    // console.log('control.value', control.value);
    const valid = /[a-z]/.test(control.value);
    if (!valid) {
      return { hasLower: true };
    }
    return null;
  }
  public static hasSymbol(control: FormControl): ValidationResult {
    // console.log('control.value', control.value);
    const valid = /[!@#$%^&*(),.?":{}|<>]/.test(control.value);
    if (!valid) {
      return { hasSymbol: true };
    }
    return null;
  }
  public static validate(registrationFormGroup: FormGroup) {
    const password = registrationFormGroup.controls.password.value;
    const repeatPassword = registrationFormGroup.controls.repeatPassword.value;

    if (repeatPassword.length <= 0) {
      return null;
    }

    if (repeatPassword !== password) {
      return {
        doesMatchPassword: true
      };
    }
    return null;
  }

  public static passwordInvalid(pass) {
    const hasNumber = /[0-9]/.test(pass);
    const hasUpper = /[A-Z]/.test(pass);
    const hasLower = /[a-z]/.test(pass);
    const hasSymbol = /[!@#$%^&*(),.?":{}|<>]/.test(pass);
    const isValidMinLength = pass.length > 7 ? true : false;
    let count = 0;
    if (hasNumber) {
      count++;
    }
    if (hasUpper) {
      count++;
    }
    if (hasLower) {
      count++;
    }
    if (hasSymbol) {
      count++;
    }
    return (count < 3 || !isValidMinLength);
  }
}
