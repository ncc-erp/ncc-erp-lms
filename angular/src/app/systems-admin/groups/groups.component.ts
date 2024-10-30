import { EditGroupsComponent } from './edit-groups/edit-groups.component';
import { CreateGroupsComponent } from './create-groups/create-groups.component';
import { GroupsDto, PagedResultDtoOfGroupDto, Result } from './../../models/groups-dto';
import { GroupsService } from './../../services/systems-admin-services/groups.service';
import { finalize, map } from 'rxjs/operators';
import { PagedListingComponentBase, PagedRequestDto, PagedResultDto } from 'shared/paged-listing-component-base';
import { Component, OnInit, Injector, ViewChild } from '@angular/core';
import { EntityDto } from '@app/models/entity';
import { appModuleAnimation } from '@shared/animations/routerTransition';

@Component({
  selector: 'app-groups',
  templateUrl: './groups.component.html',
  styleUrls: [ './groups.component.scss' ],
  animations: [ appModuleAnimation() ]
})
export class GroupsComponent extends PagedListingComponentBase<GroupsDto> {
  @ViewChild( 'createGroupsModal' ) createGroupsModal: CreateGroupsComponent;
  @ViewChild('editGroupsModal') editGroupsModal: EditGroupsComponent;
  constructor(injector: Injector, public _groupsService: GroupsService) {
    super(injector);
  }
  lstGroups: GroupsDto[] = [];
  protected list(
    request: PagedRequestDto,
    pageNumber: number,
    finishedCallback: Function
  ): void {
    this._groupsService
      .getAllGroups(request.skipCount, request.maxResultCount)
      .pipe(finalize(() => {
          finishedCallback();
        }))
      .subscribe((result: Result) => {
        this.lstGroups = result.result.items;
        this.showPaging(result.result, pageNumber);
      });
  }

  protected delete(group: GroupsDto): void {
    abp.message.confirm(
      "Delete groups '" + group.name + "'?",
      (result: boolean) => {
        if (result) {
          this._groupsService.delete(group.id).subscribe(() => {
            abp.notify.info('Deleted Group: ' + group.name);
            this.refresh();
          });
        }
      }
    );
  }

  createGroups() : void {
    this.createGroupsModal.show();
  }

  editGroups (id : string): void {
    this.editGroupsModal.show(id);
  }


}
