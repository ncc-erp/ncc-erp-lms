

export interface IAnnoucementDto {
    id: string;
    title: string;
    content: string;
    courseInstanceId: string;

}

export class AnnoucementDto implements IAnnoucementDto {
    id: string;
    title: string;
    content: string;
    courseInstanceId: string;
    imageCover: string;
    userName: string;

}

export interface ICreateAnnoucementDto {
    title: string;
    content: string;
    courseInstanceId: string;
}

export class CreateAnnoucementDto implements ICreateAnnoucementDto {
    title: string;
    content: string;
    courseInstanceId: string;
}

export interface IResultDto {
    result: AnnoucementDto[];
}
export class StudentAnnoucementDto {
    id: string;
    title: string;
    content: string;
    imageCover: string;
    userName: string;
    creationTime: Date;
}
