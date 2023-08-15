

export interface IAttachmentDto {
    id: string;
    fileName: string;
    filePath: string;
    mineType: string;
    entityId: string;
    entityType: string;
}

export class AttachmentDto implements IAttachmentDto {
    id: string;
    fileName: string;
    filePath: string;
    mineType: string;
    entityId: string;
    entityType: string;
}

export interface ICreateAttachmentDto {
    fileName: string;
    filePath: string;
    mineType: string;
    entityId: string;
    entityType: string;

}

export class CreateAttachmentDto implements ICreateAttachmentDto {
    fileName: string;
    filePath: string;
    mineType: string;
    entityId: string;
    entityType: string;
    file: File;
    assignmentSettingId: string;
    courseGroupId: string;
}

export interface IResultDto {
    result: AttachmentDto[];
}
