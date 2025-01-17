import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
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
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': 'Bearer ' + localStorage.getItem('token')
    });

    return this.http.get<User[]>(this.apiURL + '/admins/', { headers })
  }

  searchActiveUsers(displayName: string, page: number, pageSize: number): Observable<User[]>
  {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': 'Bearer ' + localStorage.getItem('token')
    });

    const params = {
      page: page.toString(),
      pageSize: pageSize.toString(),
    };
    return this.http.get<User[]>(this.apiURL + '/search-users/' + displayName, {headers, params });
  }

  searchDeletedUsers(displayName: string, page: number, pageSize: number): Observable<User[]>
  {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': 'Bearer ' + localStorage.getItem('token')
    });
    const params = {
      page: page.toString(),
      pageSize: pageSize.toString(),
    };
    const url = displayName ? `${this.apiURL}/search-deleted-users?displayName=${displayName}` : `${this.apiURL}/search-deleted-users`;
    return this.http.get<User[]>(url, {headers, params });
  }

  softDeleteUser(loggedInUserID: string, displayName: string): Observable<any>
  {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': 'Bearer ' + localStorage.getItem('token')
    });
    return this.http.put(this.apiURL + '/soft-delete/' + displayName, { loggedInUserID }, { headers });
  }

  updateUsersRole(loggedInUserID: string, role: string, displayName: string): Observable<any> 
  {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': 'Bearer ' + localStorage.getItem('token')
    });
    return this.http.put(this.apiURL + '/update-role/' + role + '/name/' + displayName, { loggedInUserID }, { headers });
  }

  updateUserDetails(user: User): Observable<any> {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': 'Bearer ' + localStorage.getItem('token')
    });

    return this.http.put(this.apiURL + '/update-user/' + user.displayName, user, { headers } );
  }

  resetUserPassword(displayName: string): Observable<any> {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': 'Bearer ' + localStorage.getItem('token')
    });
    return this.http.put(this.apiURL + '/reset-password/' + displayName, null, {headers});
  }

  hardDeleteUser(loggedInUserID: string, displayName: string): Observable<any> {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': 'Bearer ' + localStorage.getItem('token')
    });
    return this.http.delete(this.apiURL + '/hard-delete/' + displayName, {headers, body: { loggedInUserID } });
  }

  undeleteUser(loggedInUserID: string, displayName: string): Observable<any> {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': 'Bearer ' + localStorage.getItem('token')
    });
    return this.http.put(this.apiURL + '/undelete-user/' + displayName, { loggedInUserID }, { headers });
  }
}
