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
  passwordResetMessage: string | null = null;


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
