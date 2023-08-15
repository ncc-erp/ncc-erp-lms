import { StringResult } from "./common-dto";

export class BookMarkDto {
    moduleName: string;
    moduleId: string;
    pageName: string;
    pageId: string;
    creationTime: Date;
}

export class CreateBookMarkDto {
    id: string;
    pageId: string;
    courseInstanceId: string;
}
