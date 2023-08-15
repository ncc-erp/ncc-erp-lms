import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { AppConsts } from '@shared/AppConsts';
import { ResultDto } from '@app/models/entity';
import { Observable } from 'rxjs';
import { INavigatorDto } from '@app/models/isetting-dto';

@Injectable({
  providedIn: 'root'
})
export class SettingService {

  constructor(private $http: HttpClient) { }
  public GetFeature(): Observable<ResultDto<INavigatorDto>> {
    return this.$http.get<ResultDto<INavigatorDto>>(AppConsts.remoteServiceBaseUrl + `/api/services/app/FeatureOption/GetFeature`);
  }
  public ChangeReports(param) {
    return this.$http.put(AppConsts.remoteServiceBaseUrl + `/api/services/app/FeatureOption/ChangeReports`, param);
  }
  public ChangeNavigator(param) {
    return this.$http.put(AppConsts.remoteServiceBaseUrl + `/api/services/app/FeatureOption/ChangeNavigator`, param);
  }
  public ChangeStudentDefaultView(params) {
    return this.$http.put(AppConsts.remoteServiceBaseUrl + `/api/services/app/FeatureOption/ChangeStudentDefaultView`, params)
  }
  public ChangeDashboardDefaultView(params) {
    return this.$http.put(AppConsts.remoteServiceBaseUrl + `/api/services/app/FeatureOption/ChangeDashboardDefaultView`, params)
  }
  public ChangeStudentEnrollment(params) {
    return this.$http.put(AppConsts.remoteServiceBaseUrl + `/api/services/app/FeatureOption/ChangeStudentEnrollment`, params)
  }
  public ChangeStudentProficiency(params) {
    return this.$http.put(AppConsts.remoteServiceBaseUrl + `/api/services/app/FeatureOption/ChangeStudentProficiency`, params)
  }
}
