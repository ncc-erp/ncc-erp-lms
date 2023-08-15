import { CertificationTemplateDto } from '@app/models/certificationtemplate-dto';

export class StudentCertificationDto {
    courseInstanceId: string;
    status: string;
    certification: string;
    templateId: string;
    completedDate: string;
    userName: string;
    imageCover: string;
    template: CertificationTemplateDto;
    title: string;
}
export class CertificationView {
    viewWidth: number;
    viewHeight: number;
    imgBase64Value: string;
    content: string;
    orientation: number;
}
