import { Component, OnInit, Injector, ViewChild, ElementRef } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { CertificationTemplateService } from '@app/services/systems-admin-services/certificationtemplate.service';
import { ActivatedRoute } from '@angular/router';
import { CertificationTemplateDto } from '@app/models/certificationtemplate-dto';
import * as jsPDF from 'jspdf';
import html2canvas from 'html2canvas';

@Component({
  selector: 'app-view-template',
  templateUrl: './view-template.component.html',
  styleUrls: ['./view-template.component.scss']
})
export class ViewTemplateComponent extends AppComponentBase implements OnInit {
  @ViewChild('content') content: ElementRef;

  template = {} as CertificationTemplateDto;
  base64img: string;
  constructor(
    injector: Injector,
    private route: ActivatedRoute,
    private _templateService: CertificationTemplateService

  ) { super(injector); }

  ngOnInit() {
    this.getTemplate();
  }

  getTemplate() {
    const id = this.route.snapshot.paramMap.get('id');
    this._templateService.getById(id).subscribe(item => {
      this.template = item.result;
    })
  }
  printPDF() {
    const data = document.getElementById('content');
    html2canvas(data).then(canvas => {
      const pdf = new jsPDF({ orientation: 'landscape', unit: 'mm', format: 'a4' });
      const contentDataURL = canvas.toDataURL('image/png');
      pdf.addImage(contentDataURL, 'PNG', 0, 0);
      pdf.save('MyCertification.pdf');
    });
  }
}

