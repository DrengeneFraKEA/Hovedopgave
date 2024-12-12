import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from "rxjs";
import { User } from "../interfaces/adminrights/user"
@Injectable({
  providedIn: 'root'
})
export class AdminrightsService {
  private apiURL = "https://localhost:7213/adminrights";
  http = inject(HttpClient)
  constructor() { }



  getAllAdmins(): Observable<User[]>
  {
    return this.http.get<User[]>(this.apiURL + '/admins/')
  }

  getUserByDisplayName(displayName: string, page: number, pageSize: number): Observable<User[]>
  {
    return this.http.get<User[]>(this.apiURL + '/display-name/' + displayName + '?page=' + page + '&pageSize=' + pageSize)
  }

  softDeleteUser(loggedInUserDisplayName: string, displayName: string): Observable<any>
  {
    return this.http.put(this.apiURL + '/soft-delete/' + displayName, { loggedInUserDisplayName });
  }

  updateUsersRole(loggedInUserDisplayName: string, role: string, displayName: string): Observable<any> 
  {
    return this.http.put(this.apiURL + '/update-role/' + role + '/name/' + displayName, { loggedInUserDisplayName });
  }

  updateUsersDisplayName(loggedInUserDisplayName: string, newDisplayName: string, displayName: string): Observable<any> {
    return this.http.put(this.apiURL + '/update-name/' + newDisplayName + '/user/' + displayName, { loggedInUserDisplayName });
  }

  resetUserPassword(displayName: string): Observable<any> {
    return this.http.put(this.apiURL + '/reset-password/' + displayName, {});
  }
}
