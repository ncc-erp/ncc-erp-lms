import { GroupIncludeUserDto, GroupStudentDto } from './../../../models/groups-dto';
import { GroupsDto } from '@app/models/groups-dto';
import { Input, Inject } from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { DropEffect, DndDropEvent } from 'ngx-drag-drop';
import { AppComponentBase } from '@shared/app-component-base';
import { Component, Injector, OnInit } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { UserGroupService } from '@app/services/systems-admin-services/user.group.service';


@Component({
    templateUrl: 'tab-group-dialog.html',
    styles: [`    .example-container {display: flex;flex-direction: column;}
      .example-container > * {width: 100%;}  `],
})
export class DialogTabGroupComponent extends AppComponentBase implements OnInit {
    isEdit: boolean = false;
    groupName: string;
    constructor(injector: Injector,
        public dialogRef: MatDialogRef<DialogTabGroupComponent>,
        @Inject(MAT_DIALOG_DATA) public data: GroupsDto) { super(injector); }

    ngOnInit() {
        if (this.data.name) {
            this.isEdit = true;
            this.groupName = this.data.name;
        }
    }
    onNoClick(): void {
        this.dialogRef.close();
    }

}


@Component({
    selector: 'app-admin-tab-group',
    templateUrl: './tab-group.component.html',
    styleUrls: ['./tab-group.component.scss'],
    animations: [appModuleAnimation()]
})
export class TabGroupComponent extends AppComponentBase implements OnInit {

    @Input() courseId: string;
    @Input() courseInstanceId: string;

    public groupEdit: GroupStudentDto = new GroupStudentDto();
    public groupsStudents: GroupIncludeUserDto[];
    public unAssignedStudents: GroupStudentDto[];
    public unAssignedStudents_filter: GroupStudentDto[];
    isDropDrag = false;
    searchStudent: string;
    filterStudent: boolean = true;
    config: any;
    constructor(
        injector: Injector,
        private _userGroupService: UserGroupService,
        public dialog: MatDialog,
    ) {
        super(injector);
        this.config = {
            currentPage: 1,
            itemsPerPage: 20
        };
    }

    ngOnInit() {
        this.reloadGroupStudents();
    }

    getCourseGroupsWithStudents() {
        this._userGroupService.GetsGroupIncludeUser()
            .subscribe((data) => {
                this.groupsStudents = data.result.items;
            })
    }

    getUnAssignedStudents() {
        this._userGroupService.GetsUserAsStudent()
            .subscribe((data) => {
                this.unAssignedStudents = data.result.items;
                // Filter students
                this.onChangeFilterStudent();
            })
    }

    reloadGroupStudents() {
        this.getCourseGroupsWithStudents();
        this.getUnAssignedStudents();
    }



    public onCreateGroup_click() {
        const dialogRef = this.dialog.open(DialogTabGroupComponent, {
            data: {},
        });

        dialogRef.afterClosed().subscribe((group: string) => {
            if (group) {
                const tempGroup: GroupsDto = new GroupsDto();
                tempGroup.name = group;
                this._userGroupService.CreateGroup(tempGroup)
                    .subscribe(data => {
                        const temp: GroupIncludeUserDto = new GroupIncludeUserDto();
                        temp.name = group;
                        temp.id = data.result;
                        temp.userGroups = [];
                        this.groupsStudents.unshift(temp);

                        this.notify.success(this.l('SavedSuccessfully'));
                    });
            }
        });

    }

    onEditGroup_click(item: GroupIncludeUserDto) {
        const dialogRef = this.dialog.open(DialogTabGroupComponent, {
            data: { name: item.name },
        });

        dialogRef.afterClosed().subscribe(group => {
            if (group) {
                const temp: GroupsDto = new GroupsDto();
                temp.id = item.id;
                temp.name = group;
                this._userGroupService.UpdateGroup(temp)
                    .subscribe(data => {
                        if (data.success) {
                            item.name = group;
                            this.notify.info(this.l('SavedSuccessfully'));
                        }
                    });
            }
        });
    }

    onDeleteGroup_click(items: GroupIncludeUserDto[], index: number): void {
        if (items[index].userGroups.length > 0) {
            this.notify.info('Group has students.', 'Delete error!');
            return;
        }

        abp.message.confirm(
            'Delete group \'' + items[index].name + '\'?',
            (result: boolean) => {
                if (result) {
                    this._userGroupService.DeleteGroup(items[index].id)
                        .subscribe((data) => {
                            if (data.success) {
                                abp.notify.success('Deleted group: ' + items[index].name);
                                this.groupsStudents.splice(index, 1);
                            }
                        });
                }
            }
        );
    }

    /* ---end tab Group-----*/


    onDragged(item: GroupStudentDto, list: GroupStudentDto[], effect: DropEffect) {
        if (this.isDropDrag) {
            if (effect === 'copy') {
                if (this.groupEdit.groupId) {
                    this._userGroupService.CreateUserGroup(this.groupEdit)
                        .subscribe(data => {
                            if (data.success) {
                                item.countGroup++;
                                // Filter student
                                this.onChangeFilterStudent();
                                this.notify.success(this.l('AddSuccessfully'));
                            }
                        });
                }

            } else {
                const index = list.indexOf(item);
                list.splice(index, 1);
                this.isDropDrag = false;

                // test
                this.groupEdit.groupId_old = item.groupId;
                this.groupEdit.userId = item.userId;
                if (!this.groupEdit.groupId) {
                    // Call Delete UserGroup
                    this.groupEdit.groupId = item.groupId;
                    this._userGroupService.DeleteUserGroup(this.groupEdit)
                        .subscribe(data => {
                            if (data.success) {
                                // Filter student
                                this.onChangeFilterStudent();
                                this.notify.success(this.l('RemoveSuccessfully'));
                            }
                        })
                } else {
                    // Call Update UserGroup
                    this._userGroupService.UpdateUserGroup(this.groupEdit)
                        .subscribe(data => {
                            if (data.success) {
                                // Filter student
                                this.onChangeFilterStudent();
                                this.notify.success(this.l('UpdateSuccessfully'));
                            }
                        })
                }
            }
        }
    }


    onDrop(event: DndDropEvent, list?: GroupStudentDto[], groupId?: string) {
        if (list) {
            for (let i = 0; i < list.length; i++) {
                if (event.data.userId === list[i].userId) {
                    this.isDropDrag = false;
                    abp.notify.info('Existed student in group');
                    return;
                }
            }

            if (event.dropEffect === 'move' || event.dropEffect === 'copy') {
                let index = event.index;
                if (typeof index === 'undefined') {
                    index = list.length;
                }
                list.splice(index, 0, event.data);
                // console.log('call addOrUpdate: ', event.data, 'groupId: ', groupId);
                this.groupEdit = event.data;
                this.groupEdit.groupId = groupId;

                this.isDropDrag = true;
            }
        }
    }

    onDrop_student(event: DndDropEvent, list?: GroupStudentDto[]) {
        for (let i = 0; i < list.length; i++) {
            if (event.data.userId === list[i].userId) {
                list[i].countGroup--;
                this.groupEdit = {} as any;
                this.isDropDrag = true;
                return;
            }
        }
    }

    onRemoveStudent_click(items: GroupStudentDto[], index: number) {
        abp.message.confirm(
            'Remove student \'' + items[index].fullName + '\'?',
            (result: boolean) => {
                if (result) {
                    this._userGroupService.DeleteUserGroup(items[index])
                        .subscribe(data => {
                            if (data.success) {
                                for (let i = 0; i < this.unAssignedStudents.length; i++) {
                                    if (this.unAssignedStudents[i].userId === items[index].userId) {
                                        this.unAssignedStudents[i].countGroup--;
                                        break;
                                    }
                                }
                                items.splice(index, 1);
                                // Filter student
                                this.onChangeFilterStudent();
                                this.notify.success(this.l('RemoveSuccessfully'));
                            }
                        })
                }
            }
        );
    }


    onChangeFilterStudent() {
        if (this.filterStudent) {
            this.unAssignedStudents_filter = this.unAssignedStudents.filter(m => m.countGroup === 0);
        } else {
            this.unAssignedStudents_filter = this.unAssignedStudents;
        }

    }
}

