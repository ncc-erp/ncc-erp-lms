export interface ResultDto {
    result: UserDto;
}
export interface UserDto {
    name: string | '';
    surname: string | '';
    fullName: string | '';
    displayName: string | '';
    avatar: string | '';
    emailAddress: string | '';
    studentId: string | '';
    proficiency: string | '';
    biography: string | '';
    title: string | '';
    status: Status;
    languageId: number;
    timeZoneId: string | '';
    userPersonalInfoViewByPublic: boolean;
    userPersonalLinksViewByPublic: boolean;
    userPersonalAchievementViewByPublic: boolean;
    userPersonalCertificationViewByPublic: boolean;
    userLinks: UserLinks[];
    archievements: AchivementDto[];
    baseUtcOffset: string | undefined;
}
export interface Status {
    displayName: string
}
export interface UserLinks {
    title: string;
    link: string;
    id: string;
}

export interface AchivementDto {
    name: string;
    quantity: string;
}


export class ChangePasswordDto {
    id: number;
    oldPassword: string;
    newPassword: string;
}
