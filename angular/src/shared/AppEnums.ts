import { IsTenantAvailableOutputState } from '@shared/service-proxies/service-proxies';



export class AppTenantAvailabilityState {
    static Available: number = IsTenantAvailableOutputState._1;
    static InActive: number = IsTenantAvailableOutputState._2;
    static NotFound: number = IsTenantAvailableOutputState._3;
}

export class EQuestion {
    static readonly Quiz: number = 0;
    static readonly Feedback: number = 1;

    static readonly MCQ: number = 0; // Multiple Choice Questions
    static readonly SCQ: number = 1; // Single Choice Questions
    static readonly OpenEnd: number = 2; // Open-End
    static readonly FixedWord: number = 3; // Fixed Word
    static readonly Ranked: number = 4;
    static readonly Matching: number = 5;
    static readonly TrueFalse: number = 6;
    static readonly MatrixTableQuestion: number = 7;

    static readonly ViewType_Select: number = 0; //view type select
    static readonly ViewType_DragDrop: number = 1;//view type dragdrop

    static readonly QUESTION_TYPES = [
        { id: EQuestion.MCQ, name: 'Multiple Choice Questions', brifName: 'MCQ' },
        { id: EQuestion.SCQ, name: 'Single Choice Questions', brifName: 'SCQ' },
        { id: EQuestion.Ranked, name: 'Ranked', brifName: 'Ranked' },
        { id: EQuestion.OpenEnd, name: 'Open-ended', brifName: 'Open-ended' },
        { id: EQuestion.FixedWord, name: 'FixedWord', brifName: 'FixedWord' },
        { id: EQuestion.Matching, name: 'Matching', brifName: 'Matching' },
        { id: EQuestion.TrueFalse, name: 'TrueFalse', brifName: 'TrueFalse' },
        { id: EQuestion.MatrixTableQuestion, name: 'Matrix Table Question', brifName: 'MTQ' }
    ];

    static readonly QUESTION_CATES = [
        {
            id: EQuestion.Quiz, name: 'Quiz', types: [
                { id: EQuestion.MCQ, name: 'Multiple Choice Questions', brifName: 'MCQ' },
                { id: EQuestion.SCQ, name: 'Single Choice Questions', brifName: 'SCQ' },
                { id: EQuestion.Ranked, name: 'Ranked', brifName: 'Ranked' },
                { id: EQuestion.Matching, name: 'Matching', brifName: 'Matching' }]
        },
        {
            id: EQuestion.Feedback, name: 'Feedback', types: [
                { id: EQuestion.MCQ, name: 'Multiple Choice Questions', brifName: 'MCQ' },
                { id: EQuestion.SCQ, name: 'Single Choice Questions', brifName: 'SCQ' },
                { id: EQuestion.OpenEnd, name: 'Open-ended', brifName: 'Open-ended' },
                { id: EQuestion.Ranked, name: 'Ranked', brifName: 'Ranked' }
            ]
        }
    ]

}

export enum QuizType {
    Quiz = 0,
    Survey = 1
}

export enum QuizScoreKeepType {
    Hightest = 0,
    Avarage = 1
}

export class EQuiz {
    static readonly QUIZ_TYPES: any[] = [
        { id: QuizType.Quiz, name: 'Quiz', brifName: 'Q' },
        { id: QuizType.Survey, name: 'Servey', brifName: 'S' }];

    static readonly QUIZ_SCORETOKEEPTYPES: any[] = [
        { id: QuizScoreKeepType.Hightest, name: 'Highest', brifName: 'H' },
        { id: QuizScoreKeepType.Avarage, name: 'Avarage', brifName: 'A' }];

    static readonly QUIZ_STUDENTRESPONSETYPES: any[] = [
        { id: 0, name: 'OnlyAfterLastAttempt', brifName: 'OALA' },
        { id: 1, name: 'OnlyOnceAfterEachAttempt', brifName: 'OOAEA' },
        { id: 2, name: 'SeeTheCorrectAnswer', brifName: 'STCA' }];
}

export class CourseAssignmentStudent {
    static readonly ASSIGNED_STATUSES: any[] = [
        { id: 0, name: 'pending' },
        { id: 1, name: 'Approved' },
        { id: 2, name: 'Rejected' },
    ]
}

export class AssignedStatus {
    static readonly Invited = 0;
    static readonly PendingApproved = 1; // for enroll
    static readonly Accepted = 2; // table 1 only
    static readonly Rejected = 3;
    static readonly Completed = 4;
}

export class AssignedSource {
    static readonly Direct = 0;
    static readonly FromEnroll = 1;
}
export class EAssignment {
    static readonly DisplayGradeType: any[] = [
        { id: 0, name: 'CompleteInComplete' },
        { id: 1, name: 'Percentage' },
        { id: 2, name: 'Points' },
        { id: 3, name: 'NotGrade' },
    ];
    static readonly SubmissionType: any[] = [
        { id: 0, name: 'NoSubmission' },
        { id: 1, name: 'Online' },
        { id: 2, name: 'OnPaper' },
    ];
    static readonly CourseAssigmentStatus: any[] = [
        { id: 0, name: 'Draft' },
        { id: 1, name: 'Publish' },
    ];
}
export class EGradeScheme {
    static readonly HighGradeSchemeCompareOperation: any[] = [
        { id: 0, name: '<=' },
        { id: 1, name: '<' },
    ];
    static readonly LowGradeSchemeCompareOperation: any[] = [
        { id: 2, name: '>=' },
        { id: 3, name: '>' },
    ];

}
export class ETemplate {
    static readonly TemplateOrientation: any[] = [
        { id: 0, name: 'Landscape' },
        { id: 1, name: 'Portrait' },
    ];
}
export class CourseState {
    static readonly Draft = 0;
    static readonly Published = 1;
    static readonly Archived = 2;
}

export enum PageType {
    Page = 0,
    Quiz = 1,
    Assignment = 2,
    QuizFinal = 3,
    Survey = 4,
}

export enum PageLinkExamType {
    Quiz = 'quiz',
    QuizFinal = 'quiz_final',
    Survey = 'survey',
    Assignment = 'assignment',
    Page = 'page',
}

export enum CompareOperation {
    LessEqual = 0,
    LessThan = 1,
    GreaterEqual = 2,
    GreaterThan = 3,
    Equal = 4
}

export enum TestAttemptStatus {
    Open = 0,
    Testing = 1,
    Marking = 2,
    Passed = 3,
    Failed = 4
}

export enum StudentProgressStatus {
    Studying = 0,
    Completed = 1,
}

export class Common {
    static readonly abcList = ["A", "B", "C", "D", "E", "F", "G", "H"];
}

export enum CourseSourse{
    NCC = 0,
    SCORM_2004 = 1,
    SCORM_12 = 2,
}

export enum ScormCustomKey {
    NCC_CompletedCourse = 'rma.completedCourse'
}

export class ScormError{
    public static ScormErrors = [
        {code: '100', description: 'You have to enroll/re-enroll and be accepted to this course'},
        {code: '101', description: 'You completed this course. Please re-enroll this course to study again'}
    ]
}

