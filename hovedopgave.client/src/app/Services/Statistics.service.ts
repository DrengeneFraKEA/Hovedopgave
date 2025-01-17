import { Injectable } from '@angular/core';
import { HttpClient, HttpParams, HttpHeaders } from '@angular/common/http';
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

export interface TotalCounts {
  totalUsers: number;
  totalTeams: number;
  totalOrganizations: number;
  totalValorantProfiles: number;
  totalUserGameProfiles: number;
  totalLeagueProfiles: number;
  totalCompetitions: number;
}

@Injectable({
  providedIn: 'root'
})
export class StatisticsService {
  private readonly apiUrl = 'https://localhost:7213/statistics';
  constructor(private http: HttpClient) { }

  getOverviewTotals(): Observable<any> {
    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
        'Authorization': 'Bearer ' + localStorage.getItem('token')
      })
    };

    return this.http.get<TotalCounts>(`${this.apiUrl}/totals/overview`, httpOptions);
  }
}
