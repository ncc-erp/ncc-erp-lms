import { CompareOperation } from "@shared/AppEnums";

export interface UserStatusDto {
  id: string;
  displayName: string;
  level: number;
  lowCompareOperation: CompareOperation;
  requiredNumber: number;
}

export interface PagedResultDto {
  totalCount: number | undefined;
  items: UserStatusDto[] | undefined;
}

export interface Result {
  result: PagedResultDto;
}

export interface ResultObject {
  result: UserStatusDto;
}


