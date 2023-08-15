// export interface IModuleDto{
//     id: string;
//     name: string;
//     description: string;
//     sequenceOrder: number;
//     courseId: string;
// }

export class ModuleDto  {
    id: string;
    name: string;
    description: string;
    sequenceOrder: number;
    courseId: string;
}

// export interface ICreateModuleDto{    
//     name: string;
//     description: string;
//     sequenceOrder: number;
//     courseId: string;
// }

export class CreateModuleDto{    
    name: string;
    description: string;
    sequenceOrder: number;
    courseId: string;
}

// export interface ICPageDto{
//     id: string;
//     name: string;
//     sequenceOrder: number;
// }
export class CPageDto{
    id: string;
    name: string;
    sequenceOrder: number;
    linkType: string;
    progress: number;//student read
    files: CFileDto[];
}

// export interface ICModuleDto {
//     id: string;
//     name: string;
//     sequenceOrder: number;
//     pages: ICPageDto[];
// }

export class CModuleDto {
    id: string;
    name: string;
    sequenceOrder: number;
    pages: CPageDto[];
    isExpanded = false;

    totalPage: number;
    completedPage: number;
}

// export interface IResultDto{
//     result: ICModuleDto[];
// }

export interface IModulesPagesDto{
    courseId: string;
    modules: CModuleDto[];
}

export interface IModulePagesDto {
    moduleId: string;
    pages: CPageDto[];
}

export class CFileDto {
    fileName: string;
    filePath: string;
}
