// export interface ICourseDto {
//     id: string;
//     name: string;
//     description: string;
//     statusId: string;
//     type: number;
//     imageCover: string;
//     file: File;
//     realtedImage: string;
//     realtedFile: File;
//     languageId: number;
//     identifier: string;
//     studentCanOnlyParticipiateCourseBetweenTheseDate: boolean;
//     restrictStudentFromViewThisCourseAfterEndDate: boolean;
//     restrictStudentsFromViewingThisCourseBeforeEndDate: boolean
// }

export class CourseDto {
    id: string;
    courseId: string;
    name: string;
    level: string;
    imageCover: string;
    state: number;
}

export class EditCourseDto {
    id: string;
    name: string;
    description: string;
    levelId: string;
    type: number;
    imageCover: string;
    file: File;
    relatedImage: string;
    relatedFile: File;
    relatedInformation: string;
    languageId: number;
    // enableCourseGradingScheme: boolean
    identifier: string;
    studentCanOnlyParticipiateCourseBetweenTheseDate: boolean;
    restrictStudentFromViewThisCourseAfterEndDate: boolean;
    restrictStudentsFromViewingThisCourseBeforeEndDate: boolean
    syllabus: string;
    // isEdit: boolean = false;
    isEdit: boolean;

    nPageCompleted: number;
    totalPage: number;
    completedPercent: number;
    state: number;
    sourse: number;
    soursePath: string;
}

// export interface ICreateCourseDto {
//     name: string;
//     description: string;
//     levelId: string;
//     type: number;
//     imageCover: string;
//     file: File;
// }

export class CreateCourseDto {
    name: string;
    description: string;
    levelId: string;
    type: number;
    imageCover: string;

    file: File;
    fileSCORM: File;
    sourse: number;
}


export interface IPagedResultDtoOfCourseDto {
    totalCount: number | undefined;
    items: CourseDto[] | undefined;
}

export interface IResult {
    result: IPagedResultDtoOfCourseDto;
}

export interface IResultObject {
    result: CourseDto;
}


export interface ICourseStatusDto {
    id: string;
    displayName: string;
    level: number;
}

export interface ICourseTypeDto {
    id: number;
    displayName: string;
}

export class CourseDetailDto {
    course: EditCourseDto;
    categoryName: string[];
    instructors: IUserDto[];
    level: string;
    invitedInGroup: boolean;
    isSelfPaced: boolean;
    status: number;
    startTime: string;
    courseLanguage: string;
    creatorLanguage: string;
    length: number;
    nPageCompleted: number;
    totalPage: number
    completedPercent: number;
    isArchived: boolean;
    creator: IUserDto;
    canReEnroll: boolean;
    canStart: boolean;
    endTime: Date | string;
}

export interface IUserDto {
    id: number;
    fullName: string;
    avatar: string;
    userName: string;
}
export interface ICourseColor {
    id: string;
    colorCode: string;
    courseId: string;
}
