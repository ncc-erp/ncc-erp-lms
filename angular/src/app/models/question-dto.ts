import { AnswerDto } from './answer-dto';
import { StringIdNameDto } from './common-dto';

export interface IQuestionDto {
    id: string;
    title: string;
    description: string;
    mark: number;
    nWord: number;
    type: number;
    group: number;
    moduleId: string;
    courseId: string;
}

export class QuestionDto implements IQuestionDto {
    questionQuizId: string;
    id: string;
    title: string;
    description: string;
    mark: number;
    nWord: number;
    type: number;
    typeName: string;
    group: number;
    moduleId: string;
    courseId: string;
    answers: AnswerDto[] = [];

    isEditing: boolean = false;
    isExpanded: boolean = false;

    lAnswers: string[];//matching
    selectedAnswerId: string;//SCQ

    sequenceOrders: number[];//ranking
    viewType: number;//ranking

    answerText: string; //open-end

    index: number; //vị trí

    submitted: boolean = false;
    tipAnswers: string[];
    isShowedTip: boolean = false;
    isDisable: boolean = false;

    studentPoint: number;
    isSaved?: boolean;
    isAnswerChanged?:boolean;
}

export interface ICreateQuestionDto {
    title: string;
    description: string;
    mark: number;
    nWord: number;
    type: number;
    group: number;
    moduleId: string;
    courseId: string;
    quizId: string;

}

export class CreateQuestionDto implements ICreateQuestionDto {
    title: string;
    description: string;
    mark: number;
    nWord: number;
    type: number;
    group: number;
    moduleId: string;
    courseId: string;
    quizId: string;
    answers: AnswerDto[] = [];
}

export class QuestionPoolDto {
    id: string;
    title: string;
    description: string;
    mark: number;
    nWord: number;
    type: number;
    typeName: string;
    group: number;
    moduleId: string;
    courseId: string;
    answers: AnswerDto[] = [];
    linkable: boolean;
}

export class LocalDataDto {
    questions: QuestionDto[];
    quizId: string | any;
    timeLeft: number;
    createdTime: number;
    userId: string | any;
    questionIdOnScreen: string[];
    enableSetQuestion: boolean;
    pageId: string;
    hasTimeCounter: boolean;
}
