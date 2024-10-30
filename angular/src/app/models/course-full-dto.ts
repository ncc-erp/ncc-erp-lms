export interface CourseFullDto {
    courseId: string;
    name: string;
    startTime: string;
    colorCode: string;
    endTime: string;
}
export interface CourseAcceptCalendar {
    courseId: string;
    title: string;
    start: Date;
    end: Date;
    backgroundColor: string;
}
