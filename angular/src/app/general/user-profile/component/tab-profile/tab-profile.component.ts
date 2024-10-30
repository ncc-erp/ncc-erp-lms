import { Component, Injector, Input } from '@angular/core';
import { StudentService } from '@app/services/student-service/student.service';
import { UserDto } from '@app/models/user-dto';
import { PagedListingComponentBase, PagedRequestDto } from '@shared/paged-listing-component-base';
import { FormBuilder, FormGroup, FormArray, Validators } from '@angular/forms';
import { UploadService } from '@shared/upload-service/upload.service';
import { PERMISSION } from '@app/models/constant';
import { MatDialog } from '@angular/material';
import { TabProfileChangePasswordDialogComponent } from './dialog-changepassword';
import { PermissionCheckerService } from 'abp-ng2-module/dist/src/auth/permission-checker.service';



@Component({
  selector: 'app-tab-profile',
  templateUrl: './tab-profile.component.html',
  styleUrls: ['./tab-profile.component.scss']
})
export class TabProfileComponent extends PagedListingComponentBase<UserDto>{
  user = {} as UserDto;
  isEdit: boolean = false;
  profileForm: FormGroup;
  count = 0;
  userLinks: FormArray;
  checkPermisson: boolean = false;
  permissonName: string;
  get getFormArray() {
    return <FormArray>this.profileForm.get('userLinks');
  }
  get getFormGroup() {
    return <FormGroup>this.profileForm
  }
  constructor(public dialog: MatDialog,
    private _service: StudentService,
    private _uploadService: UploadService,
    private permisson: PermissionCheckerService,
    private injector: Injector, private fb: FormBuilder) {
    super(injector);
  }

  ngOnInit() {
    this.permisson.isGranted('Page')
    this.profileForm = this.fb.group({
      name: [''],
      surname: [''],
      title: [''],
      displayName: [''],
      emailAddress: [''],
      biography: [''],
      avatar: [''],
      userLinks: new FormArray([])
    });
    this._service.permissonName$.subscribe(e => {
      this.permissonName = e;
    });
    this._service.users$.subscribe(e => {
      this.user = e;
      this.profileForm.patchValue({
        name: e.name,
        surname: e.surname,
        title: e.title,
        displayName: e.displayName,
        emailAddress: e.emailAddress,
        biography: e.biography,
        avatar: e.avatar
      });
      for (const item of e.userLinks) {
        if (this.count == 0) {
          const newItem = this.addLinkForm();
          newItem.patchValue(item);
          this.getFormArray.push(newItem);
        }
      }
      this.count++;
    })
  }

  addLink() {
    this.userLinks = this.profileForm.get('userLinks') as FormArray;
    this.userLinks.push(this.addLinkForm());
  }

  removeLink(i) {
    this.getFormArray.removeAt(i);
  }

  addLinkForm(): FormGroup {
    return this.fb.group({
      id: ['00000000-0000-0000-0000-000000000000'],
      title: ['', Validators.required],
      link: ['', Validators.required]
    });
  }

  protected list(request: PagedRequestDto, pageNumber: number, finishedCallback: Function): void {
  }
  protected delete(entity: UserDto): void {
  }

  onCancel() {
    this.isEdit = false;
  }

  onSave() {
    const data = this.profileForm.getRawValue();
    const url = '/api/services/app/Account/UpdateUserInfo';
    if (data != null) {
      let user: UserDto = data;
      user.name = user.name == null ? '' : user.name.trim();
      user.surname = user.surname == null ? '' : user.surname.trim();
      user.displayName = user.displayName == null ? '' : user.displayName.trim();
      user.title = user.title == null ? '' : user.title.trim();
      user.emailAddress = user.emailAddress == null ? '' : user.emailAddress.trim();

      this._service.updateUser(user).subscribe(e => {
        abp.notify.success('Save success');
        this.isEdit = false;
        this._service.isChange(true);
      });
    }
  }

  getFile(event) {
    const url = '/api/services/app/Account/UploadImageFile'
    this._uploadService.uploadImageFile(url, event).subscribe(e => {
      if (e && e.body) {
        const linkImage: any = e.body.result;
        this.user.avatar = linkImage;
        this.getFormGroup.get('avatar').setValue(linkImage);
      }
    });
  }

  onChangePassword_click(): void {

    this.dialog.open(TabProfileChangePasswordDialogComponent, {
      width: '400px',
      data: {},
    });
  }
}
















