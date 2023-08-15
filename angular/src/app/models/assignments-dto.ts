
// export interface IAssignmentDto {
//     id: string;
//     title: string;
//     content: string;
//     status: number;
//     displayGrade: number;
//     submissionType: number;
//     courseId: string;
//     point: number;
//     isGroupAssignment: boolean;
//     isAssignIndividualGrade: boolean;
// }

export class AssignmentDto  {
    id: string;
    title: string;
    content: string;
    status: number;
    displayGrade: number;
    submissionType: number;
    courseId: string;
    // point: number;
    settings: AssignmentSettingsDto;
    groupsAssingedAssignment: GroupAssignDto[];
    allowNotify: boolean;
    isGroupAssignment: boolean;
    isAssignIndividualGrade: boolean;
    isDisable: boolean;
}

// export interface ICreateAssignmentDto {
//     id: string;
//     title: string;
//     content: string;
//     status: number;
//     displayGrade: number;
//     submissionType: number;
//     courseId: string;
//     point: number;
//     isGroupAssignment: boolean;
//     isAssignIndividualGrade: boolean;
// }

export class CreateAssignmentDto  {
    id: string;
    title: string;
    content: string;
    status: number;
    displayGrade: number;
    submissionType: number;
    courseId: string;    
    settings: AssignmentSettingsDto;
    groupsAssingedAssignment: GroupAssignDto[];
    isGroupAssignment: boolean;
    isAssignIndividualGrade: boolean;
    allowNotify: boolean;
    isDisable: boolean;
}
export interface IResultDto {
    result: AssignmentDto[];
}

export class AssignmentSettingsDto {
    id: string;
    numberOfDueDays: number;
    startTimeUtc: string;
    endTimeUtc: string;
    courseInstanceId: string;
    point: number;
}

export class GroupAssignDto {
    id: string;
}
