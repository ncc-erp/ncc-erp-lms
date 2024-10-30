import { Component, OnInit, Injector } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { StudentService } from '@app/services/student-service/student.service';
import { StudentCertificationDto, CertificationView } from '@app/models/studentcertification-dto';
import { CertificationTemplateDto } from '@app/models/certificationtemplate-dto';
import * as jsPDF from 'jspdf';
import html2canvas from 'html2canvas';
import { async } from '@angular/core/testing';

@Component({
  selector: 'app-tab-certification',
  templateUrl: './tab-certification.component.html',
  styleUrls: ['./tab-certification.component.scss']
})
export class TabCertificationComponent extends AppComponentBase implements OnInit {

  certifications: StudentCertificationDto[] = [];
  isViewPDF = false;
  print = false;
  preview = {} as CertificationView;
  idPrintUser: string;
  dataPDF: any;

  constructor(
    injector: Injector,
    private _service: StudentService,
  ) { super(injector); }
  ngOnInit() {
    this.getCertification();
  }

  getCertification() {
    this.certifications = [];
    this._service.GetAllCertificationsByUser().subscribe((result: any) => {
      this.certifications = result.result;
    });
  }
  viewPDF(item: StudentCertificationDto) {
    this.viewTemp(item.template);
    this.isViewPDF = true;
  }
  getbackground(imageCover: string) {
    const fullpath = this.getImageServerPath(imageCover);
    this._service.getImage(fullpath).subscribe(result => {
      const reader = new FileReader();
      reader.readAsDataURL(result);
      reader.onload = event => {
        // called once readAsDataURL is completed. Preview Image
        const item: any = event;
        this.preview.imgBase64Value = item.target.result;
        if (this.print) {
          setTimeout(() => {
            this.printPDF();
          }, 200);
        }
      };
    })
  }
  getpreview(item: CertificationTemplateDto) {
    //console.log(item);
    this.preview.content = item.content;
    this.preview.orientation = item.orientation;
    if (item.orientation === 0) {
      this.preview.viewWidth = 297;
      this.preview.viewHeight = 210;
    } else {
      this.preview.viewWidth = 210;
      this.preview.viewHeight = 297;
    }
  }
  viewTemp(item: CertificationTemplateDto) {
    this.getpreview(item);
    this.getbackground(item.background);
  }

  printPDF() {
    this._service.print(this.idPrintUser).subscribe(res => {
      this.dataPDF = res.result;
      const pdf = new jsPDF({ orientation: 'landscape', unit: 'mm', format: 'a4' });
      const contentDataURL = "data:image/png;base64," + this.dataPDF;
      pdf.addImage(contentDataURL, 'PNG', 0, 0);
      pdf.save('MyCertification.pdf');
      this.preview = {} as CertificationView;
      this.isViewPDF = false;
      this.print = false;
    })
  }
  CancelPreview() {
    this.preview = {} as CertificationView;
    this.isViewPDF = false;
  }
  printItem(item: StudentCertificationDto) {
    this.idPrintUser = item.template.id;
    this.isViewPDF = true;
    this.print = true;
    this.getpreview(item.template);
    this.getbackground(item.template.background);
  }
}

