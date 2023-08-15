import {StringIdNameResult, NumberIdNameResult} from './common-dto';
export interface UserGroupDto {
  id: string;
  groupId: string;
  groupName: string;
  userId: number;
  userName: string;
}

export interface CreateUserGroupDto {  
  groupId: string;
  userId: number;
}

export interface PagedResultDto {
  totalCount: number | undefined;
  items: UserGroupDto[] | undefined;
}

export interface Result {
  result: PagedResultDto;
}

export interface ResultObject {
  result: UserGroupDto;
}


export interface UsersByGroupIdResult {  
  result: NumberIdNameResult
}

export interface GroupsByUserIdResult{  
  result: StringIdNameResult
}

export interface UsersToGroupDto {
  groupId: string,
  userIds: number[]
}

export interface IGroupsToUserDto {
  userId: number,
  groupIds: string[]
}

export class UsersToGroupDto {
  groupId: string;
  userIds: number[]
}
export class GroupsToUserDto{
  userId: number;
  groupIds: string[]
}




