import { QuestionDto } from './../../../models/question-dto';
import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';

@Component({
  selector: 'app-popup-submit-answer',
  templateUrl: './popup-submit-answer.component.html',
  styleUrls: ['./popup-submit-answer.component.scss']
})
export class PopupSubmitAnswerComponent implements OnInit {
public message:string =""
public countUnsaveAnswered:number=0
  constructor(
    @Inject(MAT_DIALOG_DATA) public data: QuestionDto[],
    private _dialogRef: MatDialogRef<PopupSubmitAnswerComponent>,
  ) {
   }

  ngOnInit() {
     this.data.forEach((question)=>{
      if((this.isAnswerClicked(question) && !question.isSaved) || question.isAnswerChanged){
        this.countUnsaveAnswered++
      }
    
    })
   
  }

  
  isAnswerClicked(question:QuestionDto){
    if(question.selectedAnswerId || question.answerText
      || (question.answers.some(answer => answer.isSelected || answer.selectedSequenceOrder || answer.matchTo))){
        return true
      }
      return false
  }

  submit() {
    this._dialogRef.close(true);
  }

}
