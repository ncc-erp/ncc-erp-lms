import { AppComponentBase } from '@shared/app-component-base';
import { Component, OnInit, Injector } from '@angular/core';
import { BaseService } from '@app/services/base-service/base.service';
import { ActivatedRoute } from '@angular/router';
import { map } from 'rxjs/operators';
import { UserDto } from '@app/models/user-dto';

@Component({
  selector: 'app-public-profile',
  templateUrl: './public-profile.component.html',
  styleUrls: ['./public-profile.component.scss']
})
export class PublicProfileComponent extends AppComponentBase implements OnInit {
  user = {} as UserDto;
  constructor(inject: Injector, public _base: BaseService, private route: ActivatedRoute) {
    super(inject);
  }

  ngOnInit() {
    this.route.params.subscribe((e) => {
      this.onGetData(e.id);
    })
  }

  onGetData(id) {
    this._base._studentService.getProfileById(id).pipe(map(m => m.result)).subscribe(result => {
      this.user = result;
    });
  }
}
