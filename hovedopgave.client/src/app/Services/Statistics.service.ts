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

  getStats(
    entity: string,
    fromDate?: string,
    toDate?: string
  ): Observable<{ [key: string]: number }> {
    let params = new HttpParams();
    if (fromDate) params = params.set('fromDate', fromDate);
    if (toDate) params = params.set('toDate', toDate);

    return this.http.get<{ [key: string]: number }>(
      `${this.apiUrl}/${entity}/stats`,
      { params }
    );
  }

  getOverviewTotals(): Observable<TotalCounts> {
    return this.http.get<TotalCounts>(`${this.apiUrl}/totals/overview`);
  }
}
