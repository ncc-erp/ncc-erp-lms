
export interface ICourseSettingDto {
    id: string;
    courseId: string;
    allowSkip: boolean;
    startTime: Date | string;
    endTime: Date | string;
    passingMark: number;
    totalQuiz: number;
}

export class CourseSettingDto implements ICourseSettingDto {
    id: string;
    courseId: string;
    allowSkip: boolean = true;
    startTime: Date | string;
    endTime: Date | string;
    passingMark: number;
    totalQuiz: number;
    enableCourseGradingScheme: boolean;
    gradeSchemeId: string;
}

export class CourseSettingFeatualModel {
    key: string;
    entityId: string;
    name: string;
    description: string;
    value: boolean | number | string;
    checkbox?: boolean = true;
}
export class CourseSettingFeatualValueDto {
    Show_recent_annoucements_as_first_page: boolean;
    Number_of_annoucements_shown_on_homepage: number;
    Allow_students_create_disscussion_on_QA: boolean;
    Grades_Summary_List_students_by_name: boolean;
    Allow_completed_students_to_re_enroll: boolean;
}
