import { Component } from '@angular/core';

@Component({
  selector: 'app-adminrights',
  templateUrl: './adminrights.component.html',
  styleUrl: './adminrights.component.css'
})
export class AdminrightsComponent {
  title = 'Admin Rights';
  isModalOpen: boolean = false;
  selectedUser: { username: string } | null = null;


  openEditModal(user: { username: string }) {
    this.selectedUser = user;
    this.isModalOpen = true;
  }

  closeModal() {
    this.isModalOpen = false;
    this.selectedUser = null;
  }


  sendNewPassword() {
    // Logic
  }

  deleteUser() {
    // Logic
  }

  submitChanges() {
    // Logic
  }


}
