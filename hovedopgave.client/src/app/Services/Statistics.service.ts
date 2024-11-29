import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface SignupStats {
  totalSignups: number;
  userSignups: number;
  teamSignups: number;
  organizationSignups: number;
  dailySignups: number;
  weeklySignups: number;
  monthlySignups: number;
}

@Injectable({
  providedIn: 'root'
})
export class StatisticsService {
  private readonly apiUrl = 'https://localhost:7213/statistics'; 

  constructor(private http: HttpClient) { }

  getSignupStats(fromDate?: string | null, toDate?: string | null): Observable<SignupStats> {
    let params = new HttpParams();

    if (fromDate) {
      // Convert fromDate to UTC
      const utcFromDate = new Date(fromDate).toISOString();
      params = params.set('fromDate', utcFromDate);
    }

    if (toDate) {
      // Convert toDate to UTC
      const utcToDate = new Date(toDate).toISOString();
      params = params.set('toDate', utcToDate);
    }

    return this.http.get<SignupStats>(`${this.apiUrl}/signups`, { params });
  }
}

