import { QuestionDto } from './question-dto';
import { TestAttemptDto } from '@app/services/student-service/test.attempt.service';
import { Data } from '@angular/router';

export interface IQuizDto {
    id: string;
    title: string;
    content: string;
    courseId: string;
    status: number;
    type: number;
    isShuffleAnswer: boolean;
    timeLimit: number;
    allowAttempts: number;
    scoreKeepType: number;
    showOneQuestionAtATime: boolean
    lookQuestionAfterAnswer: boolean;
    responseType: string;
    // point: number;
}

export class QuizDto {
    id: string;
    title: string;
    content: string;
    courseId: string;
    status: number;
    type: number;
    isShuffleAnswer: boolean;
    timeLimit: number;
    allowAttempts: number;
    scoreKeepType: number;
    showOneQuestionAtATime: boolean;
    lookQuestionAfterAnswer: boolean;
    responseType: string;
    // point: number;
    settings: QuizSettingsDto;
    groupsAssingedQuiz: GroupAssignQuizDto[];
    allowNotify: boolean;
    courseInstanceId: string;

}

export class QuizOptionDto {
    id: string;
    title: string;
    content: string;
    type: number;
    isShuffleAnswer: boolean;
    timeLimit: number;
    allowAttempts: number;
    scoreKeepType: number;
    showOneQuestionAtATime: boolean;
    nQuestionAtATime: number = 2;//not in config, show n questions at a time
    lookQuestionAfterAnswer: boolean;
    responseType: string | number;
    settings: QuizSettingsDto;
    testingAttempt: TestAttemptDto;
    testAttempts: TestAttemptDto[];
    isExpired: boolean;
}

// export interface ICreateQuizDto {
//     title: string;
//     content: string;
//     courseId: string;
//     status: number;
//     type: number;
//     isShuffleAnswer: boolean;
//     timeLimit: number;
//     allowAttempts: number;
//     scoreKeepType: number;
//     showOneQuestionAtATime: boolean
//     lookQuestionAfterAnswer: boolean;
//     responseType: any;
//     // point: number;
// }

export class CreateQuizDto {
    id: string;
    title: string;
    content: string;
    courseId: string;
    status: number;
    type: number;
    isShuffleAnswer: boolean;
    timeLimit: number;
    allowAttempts: number;
    scoreKeepType: number;
    showOneQuestionAtATime: boolean
    lookQuestionAfterAnswer: boolean;
    responseType: string;
    // point: number;
    settings: QuizSettingsDto;
    groupsAssingedQuiz: GroupAssignQuizDto[];
    allowNotify: boolean;
    courseInstanceId: string;
}
export interface IResultDto {
    result: any[];
}

export class QuizSettingsDto {
    id: string;
    assigngroup: string;
    noOfDueDays: number;
    startTimeUtc: string | Date;
    endTimeUtc: string | Date;
    courseInstanceId: string;
    point: number;
    totalNumberQuestion: number;
    applySameStartEndTimeAsCourse: boolean;
}
export class GroupAssignQuizDto {
    id: string;
}

export class QuizProgressDto {
    settingId: string;
    studentId: string;
    totalScore: number;
    studentScore: number;
}

export class StudentsProgressDto {
    studentId: string;
    studentName: string;
    totalScore: number;
    studentScore: number;
    scorePercent: string;
}
