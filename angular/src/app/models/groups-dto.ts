export class GroupsDto {
  id: string;
  name: string;
  description: string;
  parentId: string;
}

export interface GroupsDDDto {
  id: string;
  name: string;
}

export interface PagedResultDtoOfGroupDto {
  totalCount: number | undefined;
  items: GroupsDto[] | undefined;
}

export interface Result {
  result: PagedResultDtoOfGroupDto;
}

export interface ResultObject {
  result: GroupsDto;
}


export class GroupIncludeUserDto {
  id: string;
  name: string;
  description: string;
  userGroups: GroupStudentDto[];
}

export class GroupStudentDto {
  userId: number
  imageCover: string;
  iserName: string;
  fullName: string;
  email: string;
  countGroup: number;
  userName: string;
  groupId: string;
  groupName: string;
  groupId_old: string;
}
export class OptionUserDto {
  userId: string
  fullName: string;
}


