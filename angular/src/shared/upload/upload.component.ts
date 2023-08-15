import { Component, Output, EventEmitter, Input } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'file-upload',
  templateUrl: './upload.component.html',
  styleUrls: ['./upload.component.scss']
})
export class UploadComponent {

  public progress: number;
  public message: string;

  @Input() custom: string; // danger!!! don't delete me. 'toại'
  @Input() title: string;
  @Input() fileType: string;
  @Input() page: string;
  @Output() outputImgBase64: EventEmitter<File> = new EventEmitter();
  @Output() outputFileInfo: EventEmitter<File> = new EventEmitter();
  @Input() isLoading: boolean = false;


  // public fileToUpload: File;

  constructor(private http: HttpClient) { }

  // public onfileuploadChange(files) {
  //   this.fileToUpload = files[0];
  // }


  onFileSelected(event) {
    if (event.target.files && event.target.files[0]) {
      var reader = new FileReader();

      reader.readAsDataURL(event.target.files[0]); // read file as data url upload File
      this.outputFileInfo.emit(event.target.files[0]);

      if (event.target.files && event.target.files[0].type.includes('image')) {
        reader.onload = event => {
          // called once readAsDataURL is completed. Preview Image
          let item: any = event;
          this.outputImgBase64.emit(item.target.result);
        };
      }
    }
  }



  getFileType(): string {
    switch (this.fileType) {
      case 'image':
        return 'image/*';
      case 'video':
        return 'video/*';
      case 'application':
        return 'application/*';
      default:
        return this.fileType;
    }
  }
}
