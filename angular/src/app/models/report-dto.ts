export class UserLogin {
    ipAddress: string;
    creationTime: Date;
    userId: number;
    action: string;
    courseName: string;
    id: number;
}
export class UserGroupLoginDto {
    userName: string;
    userId: number;
    countLogin: number;
    users: UserLogin[];
    currentPage: number = 1;
}

export class ReportInput {
    FromDate: Date | string;
    ToDate: Date | string;
    UserId: number | string;
    SearchText: string;
    SortDirection: number = 0; // 0 Asc; 1 Desc
    CurrentPage: number = 1;
    MaxResultCount: number = 0; // ItemsPerPage
}
export class ButtonSelectDto {
    index: number;
    name: string;
    isLoaded?: boolean;
    data?: UserGroupLoginDto[];
    search?: string;
    totalPage?: number = 10;          //  total of users page
    isShowIpAddress?: boolean = false;
    isShowCourseName?: boolean = false;
    currentPage: number = 1;
    itemPerPage: number = 10;
}
export class DataWriteDto {
    _: string;
    userId: number;
    user_Name: string;
    creation_Time: string;
    courseName: string;
    actions: string;
    ip_Address: string;
}
export class SelectOptionDto {
    id: number;
    value: string;
}
