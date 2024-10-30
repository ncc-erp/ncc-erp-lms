export interface ISettingDto {
    navigator: INavigatorDto;

}
export interface IStudentDto {

}
export interface IRoleDto {
    id: number;
    name: string;
    displayName: string;
}

export interface IPermissionDto {
    permissonId: number;
    permissionName: string;
    isGranted: boolean;
    roleId: number;
    roleName: string;
}

export interface IDashboardDto {

}
export interface INavigatorDto {
    permissionRoles: IPermissionDto[];
    roles: IRoleDto[];
    studentDefaultViewName: number;
    dashboardDefaultViewName: number;
    studentCourseEnrollment: boolean;
    studentProficiency: boolean;
}
export interface IReportDto {

}
