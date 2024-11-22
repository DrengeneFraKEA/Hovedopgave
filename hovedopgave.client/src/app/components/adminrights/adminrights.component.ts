import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-adminrights',
  templateUrl: './adminrights.component.html',
  styleUrl: './adminrights.component.css'
})
export class AdminrightsComponent implements OnInit {
  title = 'Admin Rights';
  isModalOpen: boolean = false;
  selectedUser: { username: string } | null = null;
  passwordResetMessage: string | null = null;

  constructor(private http: HttpClient, private route: Router) { }

  ngOnInit() {
    var token = localStorage.getItem("token");
    if (token === null || token === "") this.route.navigate(['login']);
  }


  openEditModal(user: { username: string }) {
    this.selectedUser = user;
    this.isModalOpen = true;
  }

  closeModal() {
    this.isModalOpen = false;
    this.selectedUser = null;
    this.passwordResetMessage = null;
  }


  sendNewPassword() {
    // Logic
    this.passwordResetMessage = 'New password sent via email.';

  }

  deleteUser() {
    // Logic
  }

  submitChanges() {
    // Logic
  }


}
