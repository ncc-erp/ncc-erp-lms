export interface IStringIdNameDto{
  id: string,
  name: string
}

export class StringIdNameDto implements  IStringIdNameDto{
  id: string;
  name: string
}

export interface INumberIdNameDto{
  id: number,
  name: string
}
export class NumberIdNameDto implements INumberIdNameDto{
  id: number;
  name: string
}

export interface StringResult {
  items: string[];
}

export interface NumberResult {
  items: number[];
}

export interface StringIdNameResult{
  items: IStringIdNameDto[] | undefined;
}

export interface NumberIdNameResult{
  items: INumberIdNameDto[] | undefined;
}

  
export interface IResult {
    result: {
      totalCount: number | undefined;
      items: any[] | undefined;   
    };
}
  
export interface IResultObject {
    result: any;
}

export interface IFileInfoDto {
  fileName: string;
  mineType: string;
  fileSize: number;
  serverPath: string;
}

export class FileInfoDto implements IFileInfoDto{
  fileName: string;
  mineType: string;
  fileSize: number;
  serverPath: string;
}

export class ListResultDto{
  result: any[]
}