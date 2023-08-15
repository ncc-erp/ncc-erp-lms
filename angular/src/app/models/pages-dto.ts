export interface IPageDto{
    id: string;
    name: string;
    content: string;
    sequenceOrder: number;
    moduleId: string;
    courseId: string;
}

export class PageDto implements IPageDto{
    id: string;
    name: string;
    content: string;
    sequenceOrder: number;
    moduleId: string;
    courseId: string;
    links: PageLinkExamDto[] = [];
    bookmarked: boolean;//for student view
}

export class PageLinkExamDto{
    id: string;
    pageId: string;
    linkId: string;
    linkType: string;
    sequenceOrder: number;
}

export interface ICreatePageDto{    
    name: string;
    content: string;
    sequenceOrder: number;
    moduleId: string;
}

export class CreatePageDto implements ICreatePageDto{    
    name: string;
    content: string;
    sequenceOrder: number;
    moduleId: string;
}