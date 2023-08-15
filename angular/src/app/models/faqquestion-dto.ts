export class FAQQuestionDto {
  id: string;
  title: string;
  content: string;
  sequenceOrder: number;
  courseId: string;
  isEdit: boolean = false;
  contentOld: string;
  titleOld: string;
  isShowAnswer: boolean;
}

export class FAQAnswerDto {
  id: string;
  sequenceOrder: number;
  content: string;
}
export class FAQQnADto extends FAQQuestionDto {
  faqAnswers: FAQAnswerDto[];
}

export class FAQQuestionAdminDto {
  id: string;
  courseInstanceId: string;
  name: string;
  startTime: Date;
  endTime: DataCue;
  totalFAQ: number;
  totalQuestion: number;
  totalResponse: number;
  isReadedQuestion: boolean;
  isReadedResponse: boolean;
  imageCover: string;
  state: number;
  creationTime: Date;
}
