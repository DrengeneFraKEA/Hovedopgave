import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AdminrightsService } from '../../services/adminrights.service'
import { User } from '../../interfaces/adminrights/user'

@Component({
  selector: 'app-adminrights',
  templateUrl: './adminrights.component.html',
  styleUrl: './adminrights.component.css'
})
export class AdminrightsComponent implements OnInit {
  title = 'Admin Rights';
  isModalOpen: boolean = false;
  selectedUser: { displayName: string, role: string } | null = null;
  passwordResetMessage: string | null = null;
  admins: User[] = [];
  searchResult: User[] = [];
  searchQuery: string = '';
  selectedRole: string | null = null;
  newDisplayName: string = '';
  displayNameError: string | null = null
  currentPage: number = 1;
  pageSize: number = 5;
  constructor(private http: HttpClient, private route: Router, private adminrightsService: AdminrightsService) { }

  ngOnInit() {
    var token = localStorage.getItem("token");
    if (token === null || token === "") {
      this.route.navigate(['login']);
    }
    else {
      this.fetchAdmins();
    }
  }


  fetchAdmins() {
    this.adminrightsService.getAllAdmins().subscribe(
      (data) => {
        this.admins = data;
      },
      (error) => {
        console.error('Error fetching admins: ', error)
      }
    );
  }

  searchUser() {
    if (this.searchQuery.trim() !== '') {
      this.adminrightsService.getUserByDisplayName(this.searchQuery, this.currentPage, this.pageSize).subscribe(
        (data) => {
            this.searchResult = data;
        },
        (error) => {
          console.error('Error fetching user: ', error);
          this.searchResult = [];
        }
      );
    } else {
      this.searchResult = [];
    }
  }

  nextPage() {
    this.currentPage++;
    this.searchUser();
  }

  previousPage() {
    if (this.currentPage > 1) {
      this.currentPage--;
      this.searchUser();
    }
  }

  openEditModal(user: { displayName: string, role: string }) {
    this.selectedUser = user;
    this.selectedRole = user.role;
    this.newDisplayName = user.displayName;
    this.isModalOpen = true;
  }

  closeModal() {
    this.isModalOpen = false;
    this.selectedUser = null;
    this.selectedRole = null;
    this.newDisplayName = '';
    this.passwordResetMessage = null;
    this.displayNameError = null;
  }


  sendNewPassword() {
    // Logic
    this.passwordResetMessage = 'New password sent via email.';

  }

  deleteUser() {
    if (this.selectedUser) {
      this.adminrightsService.softDeleteUser(this.selectedUser.displayName).subscribe(
        () => {
          this.fetchAdmins(); // Refreshing after the user is deleted
          this.closeModal();
        },
        (error) => {
          console.error('Error deleting user: ', error);
        }
      );
    }
  }

  changeUsersRole(role: string) {
    if (this.selectedUser && this.selectedRole !== role) {
      this.selectedRole = role;
      this.adminrightsService.updateUsersRole(this.selectedRole, this.selectedUser.displayName).subscribe(
        () => {
          this.fetchAdmins(); // Refreshing after the role is updated
          this.closeModal();
        },
        (error) => {
          console.log('Error updating users role: ', error);
        }
      );
    }
  }

  changeUsersDisplayName() {
    if (this.selectedUser && this.newDisplayName.trim() !== '') {
      this.adminrightsService.updateUsersDisplayName(this.newDisplayName, this.selectedUser.displayName).subscribe(
        () => {
          this.fetchAdmins(); // Refreshing after the display name is upddated
          this.closeModal();
        },
        (error) => {
          if (error.status === 404) {
            this.displayNameError = error.error.message;
          } else {
            console.log('Error changing display name: ', error);
          }
        }
      );
    }
  }


}
