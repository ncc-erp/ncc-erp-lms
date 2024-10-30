export interface IStatictisDto {
    totalCurrent: number;
    totalExpired: number;
    totalUpComming: number;
    totalStartSoon: number;
    statictisCourseCategory
    statictisCourseLevels
}

export interface StatictisCourseLevels {
    levelName: string;
    total: number;
}
export interface StatictisCourseCategory {
    categoryName: string;
    total: number;
}

export interface ICourseDashboard {
    id: string;
    startDate: string;
    endDate: string;
    name: string;
    description: string;
    relationInfo: string;
    currentPoint: number;
    comminSoonDatePoint: number;
    alreadyStart: boolean;
    isCommingSoon: boolean;
    isUpComming: boolean;
    isSelfPaced: boolean;
    imageCover: string;
    state: number;
    status: number;
    link: string;
    isLearning: boolean;

    completedPercent: number;
    isArchived: boolean;
}
export interface ICourseInstance {
    id: string;
    startTime: string;
    endTime: string;
    createrName: string;
}
