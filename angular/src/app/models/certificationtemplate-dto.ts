

export interface ICertificationTemplateDto {
    id: string;
    isActive: boolean;
    courseId: string;
    background: string;
    name: string;
    content: string;
    certificationType: number;
    orientation: number
}

export class CertificationTemplateDto implements ICertificationTemplateDto {
    id: string;
    isActive: boolean;
    courseId: string;
    background: string;
    name: string;
    content: string;
    certificationType: number;
    isEdit: boolean;
    file: File;
    imgBase64Value: string;
    orientation: number;
    isView: boolean;


}

export interface ICreateCertificationTemplateDto {
    isActive: boolean;
    courseId: string;
    name: string;
    content: string;
    certificationType: number;
    orientation: number

}

export class CreateCertificationTemplateDto implements ICreateCertificationTemplateDto {
    isActive: boolean;
    courseId: string;
    name: string;
    content: string;
    certificationType: number;
    file: File;
    isEdit: boolean;
    orientation: number
}

export interface IResultDto {
    result: CertificationTemplateDto[];
}
