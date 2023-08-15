import { Component, OnInit, Input, Injector } from '@angular/core';
import { BaseService } from '@app/services/base-service/base.service';
import { map } from 'rxjs/operators';
import { UserDto } from '@app/models/user-dto';
import { PERMISSION } from '@app/models/constant';
import { AppComponentBase } from '@shared/app-component-base';

@Component({
  selector: 'app-user-profile',
  templateUrl: './user-profile.component.html',
  styleUrls: ['./user-profile.component.scss']
})
export class UserProfileComponent extends AppComponentBase implements OnInit {
  constructor(private injector : Injector,private _service: BaseService) { 
    super(injector);
  }

  ngOnInit() {
    this.onGetByCurrentId();
    this.onGetLanguages();
    this.onGetTimeZone();
    this._service._studentService.isChanges$.subscribe(e => {
      if (e) {
        this.onGetByCurrentId();
      }
    });
  }

  onGetTimeZone(){
    this._service._studentService.getTimeZone().pipe(map(e => e.result)).subscribe(result => {
      this._service._studentService.setTimeZone(result);
    });
  }

  onGetLanguages() {
    this._service._studentService.getAllLanguage().pipe(map(e => e.result)).subscribe(result => {
      this._service._studentService.setLanguage(result);
    });
  }

  onGetByCurrentId() {
   
    this._service._studentService.getUserById().pipe(map(r => r.result)).subscribe(result => {
      this._service._studentService.setUser(result);
    });
  }

}
