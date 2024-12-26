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

  searchActiveUsers(displayName: string, page: number, pageSize: number): Observable<User[]>
  {
    const params = {
      page: page.toString(),
      pageSize: pageSize.toString(),
    };
    return this.http.get<User[]>(this.apiURL + '/search-users/' + displayName, { params });
  }

  searchDeletedUsers(displayName: string, page: number, pageSize: number): Observable<User[]>
  {
    const params = {
      page: page.toString(),
      pageSize: pageSize.toString(),
    };
    const url = displayName ? `${this.apiURL}/search-deleted-users?displayName=${displayName}` : `${this.apiURL}/search-deleted-users`;
    return this.http.get<User[]>(url, { params });
  }

  softDeleteUser(loggedInUserDisplayName: string, displayName: string): Observable<any>
  {
    return this.http.put(this.apiURL + '/soft-delete/' + displayName, { loggedInUserDisplayName });
  }

  updateUsersRole(loggedInUserDisplayName: string, role: string, displayName: string): Observable<any> 
  {
    return this.http.put(this.apiURL + '/update-role/' + role + '/name/' + displayName, { loggedInUserDisplayName });
  }

  updateUserDetails(user: User): Observable<any> {
    return this.http.put(this.apiURL + '/update-user/' + user.displayName, user );
  }

  resetUserPassword(displayName: string): Observable<any> {
    return this.http.put(this.apiURL + '/reset-password/' + displayName, {});
  }

  hardDeleteUser(loggedInUserDisplayName: string, displayName: string): Observable<any> {
    return this.http.delete(this.apiURL + '/hard-delete/' + displayName, { body: { loggedInUserDisplayName } });
  }

  undeleteUser(loggedInUserDisplayName: string, displayName: string): Observable<any> {
    return this.http.put(this.apiURL + '/undelete-user/' + displayName, { loggedInUserDisplayName });
  }
}
