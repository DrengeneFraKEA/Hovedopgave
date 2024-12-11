import { HttpClient } from "@angular/common/http";
import { Injectable } from '@angular/core';
import { Observable } from "rxjs";
import { map } from 'rxjs/operators';

export interface graphData
{
  date: string;
  value: number;
}

@Injectable({
  providedIn: 'root'
})

export class GraphService
{
  constructor(private http: HttpClient) { }
}
