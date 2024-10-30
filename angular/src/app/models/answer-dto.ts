export class AnswerDto {
    id: string;
    rAnswer: string;
    lAnswer: string;    
    isCorrect: boolean;
    sequenceOrder: any;
    questionId: string;

    isSelected: boolean;
    matchTo: string;
    selectedSequenceOrder: any;
}