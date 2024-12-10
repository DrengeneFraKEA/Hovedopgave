import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class StatisticsService {
  private readonly apiUrl = 'https://localhost:7213/statistics';

  constructor(private http: HttpClient) { }

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
}
