import { UserCreaterDto } from './user_creater-dto';

export class QAQuestionAnswerDto extends UserCreaterDto {
    id: string;
    title: string;
    content: string;
    qaAnswers: QAAnswerDto[];
    responses: number;
    isNew: boolean;
    isShowMore: boolean;
}
export class QAQuestionDto extends UserCreaterDto {
    id?: string;
    courseInstanceId: string;
    title: string;
    content: string;
}
export class QAAnswerDto {
    id: string;
    content: string;
    new_content: string;
    answers: QAAnswerDto[];
    userId: number;
    imageCover: string;
    userName: string;
    fullName: string;
    email: string;
    isFollow: boolean;
    creationTime: Date;
    isShowAnswer: boolean;
    isEdit: boolean;
    numberAnswer: number;
    pId: string | undefined;
}

export class QAAnswerInput {
    questionId: string;
    content: string;
    responseParentId: string;
    replyUserName: string;
}
