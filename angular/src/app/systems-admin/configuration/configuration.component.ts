import { Component, OnInit, Injector, ViewChild, ElementRef } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { ConfigurationService } from '@app/services/systems-admin-services/configuration.service';
import { ConfigurationDto } from '@app/models/configuration-dto';

@Component({
  selector: 'app-configuration',
  templateUrl: './configuration.component.html',
  styleUrls: ['./configuration.component.scss']
})
export class ConfigurationComponent extends AppComponentBase implements OnInit {

  @ViewChild('modalContent') modalContent: ElementRef;

  config = {} as ConfigurationDto;
  // location: string = 'abcd';
  constructor(
    injector: Injector,
    public _service: ConfigurationService) {
    super(injector);
  }
  ngOnInit() {
    this.config.location = 'C:\\location';
    this.config.scormLocation = 'C:\\SCORMlocation'
    this._service.getConfiguration().subscribe((result) => {
      //console.log('result', result);

      this.config = result.result;
    });
  }
  onShown(): void {
    $.AdminBSB.input.activate($(this.modalContent.nativeElement));
  }
  save() {
    this._service.changeDirectionLocation(this.config).subscribe(() => {
      this.notify.info(this.l('SavedSuccessfully'));
    })
  }

}
