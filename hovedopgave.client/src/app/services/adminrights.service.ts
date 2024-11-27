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

  getUserByDisplayName(displayName: string): Observable<User>
  {
    return this.http.get<User>(this.apiURL + '/display-name/' + displayName)
  }

  softDeleteUser(displayName: string): Observable<any>
  {
    return this.http.put(this.apiURL + '/soft-delete/' + displayName, {});
  }

  updateUsersRole(role: string, displayName: string): Observable<any> 
  {
    return this.http.put(this.apiURL + '/update-role/' + role + '/user/' + displayName, {});
  }

  updateUsersDisplayName(newDisplayName: string, displayName: string): Observable<any> {
    return this.http.put(this.apiURL + '/update-name/' + newDisplayName + '/user/' + displayName, {});
  }
}
