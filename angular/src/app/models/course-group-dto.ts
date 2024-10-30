export class CourseGroupDto{
    id: string;
    name: string;
    courseInstanceId: string;
}

export class CourseGroupWithMemberDto{
    id: string;
    groupName: string;
    students: StudentCourseGroupListDto[];
}
export class StudentCourseGroupListDto{
    id: string;
    studentName: string;
    assignedStudentId: string;
}

export class StudentCourseGroupDto{    
    courseGroupId: string;
    assignedStudentIds: string[];
}