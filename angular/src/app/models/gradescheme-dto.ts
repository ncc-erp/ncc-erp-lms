
export interface IGradeSchemeDto {
    id: string;
    title: string;
    status: number;
}

export class GradeSchemeDto implements IGradeSchemeDto {
    id: string;
    title: string;
    status: number;
    elementList: GradeSchemeElementDto[];
}

export interface ICreateGradeSchemeDto {
    id: string;
    title: string;
    status: number;
}

export class CreateGradeSchemeDto implements ICreateGradeSchemeDto {
    id: string;
    courseId: string;
    title: string;
    status: number;
    elementList: GradeSchemeElementDto[];
}

export interface IResultDto {
    result: GradeSchemeDto[];
}

export interface IGradeSchemeElementDto {
    id: string;
    gradeSchemeId: string;
    lowRange: number;
    highRange: number;
    name: string;
    lowCompareOperation: number;
    highCompareOpertion: number;
    editable: boolean;
}

export class GradeSchemeElementDto implements IGradeSchemeElementDto {
    id: string;
    gradeSchemeId: string;
    lowRange: number;
    highRange: number;
    name: string;
    lowCompareOperation: number;
    highCompareOpertion: number;
    editable: boolean;
    rowIndex: number;
}

