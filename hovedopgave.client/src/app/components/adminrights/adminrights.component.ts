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
  selectedUser: User | null = null;
  newFullName: string = '';
  newEmail: string = '';
  passwordResetMessage: string | null = null;
  admins: User[] = [];
  searchResult: User[] = [];
  searchQuery: string = '';
  selectedRole: string = '';
  newDisplayName: string = '';
  displayNameError: string | null = null
  currentPage: number = 1;
  pageSize: number = 5;
  loggedinUserID = localStorage.getItem("user_id");
  updateRoleError: string | null = null;
  selectedView: string = 'users';
  searchDeleted: boolean = false;

  roles: string[] = [
    'SYSTEMADMIN',
    'SUPERUSER',
    'MODERATOR',
    'CREATOR',
    'AFFILIATE',
    'USER',
    'GUEST'
  ];
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
    const query = this.searchQuery.trim();

    if (this.searchDeleted) {
      this.adminrightsService.searchDeletedUsers(query, this.currentPage, this.pageSize).subscribe(
        (data) => {
          this.searchResult = data;
        },
        (error) => {
          console.error('Error fetching deleted user: ', error);
          this.searchResult = [];
        }
      );
    } else if (query !== '') {
      this.adminrightsService.searchActiveUsers(query, this.currentPage, this.pageSize).subscribe(
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

  openEditModal(user: User) {
    this.selectedUser = user;
    this.selectedRole = user.role;
    this.newDisplayName = user.displayName;
    this.newFullName = user.fullName;
    this.newEmail = user.email;
    this.isModalOpen = true;
  }

  closeModal() {
    this.isModalOpen = false;
    this.selectedUser = null;
    this.selectedRole = '';
    this.newDisplayName = '';
    this.passwordResetMessage = null;
    this.displayNameError = null;
    this.updateRoleError = null;
  }


  sendNewPassword() {
    if (this.selectedUser) {
      this.adminrightsService.resetUserPassword(this.selectedUser.displayName).subscribe(
        () => {
          this.passwordResetMessage = 'New password sent via email.';
        },
        (error) => {
          console.error('Error resetting password: ', error);
          this.passwordResetMessage = 'Failed to reset password.';
        }
      );
    }
  }

  deleteUser() {
    if (this.selectedUser && this.loggedinUserID) {
      const confirmDelete = window.confirm(`Are you sure you want to soft delete user ${this.selectedUser.displayName}?`);
      if (confirmDelete) {
        this.adminrightsService.softDeleteUser(this.loggedinUserID, this.selectedUser.displayName).subscribe(
          () => {
            this.fetchAdmins(); // Refreshing after the user is deleted
            this.closeModal();
          },
          (error) => {
            this.updateRoleError = 'Not enough privileges to delete user';
          }
        );
      }
    }
  }
  

  changeUsersRole(role: string) {
    if (this.selectedUser && this.selectedRole !== role && this.loggedinUserID) {
      this.selectedRole = role;
      this.adminrightsService.updateUsersRole(this.loggedinUserID, this.selectedRole, this.selectedUser.displayName).subscribe(
        () => {
          this.fetchAdmins(); // Refreshing after the role is updated
          this.closeModal();
        },
        (error) => {
          this.updateRoleError = 'Not enough privileges to change role';
        }
      );
    }
  }

  changeUserDetails() {
    if (this.selectedUser && this.loggedinUserID) {
      const updatedUser: User = {
        ...this.selectedUser,
        loggedInUser: this.loggedinUserID,
        displayName: this.selectedUser.displayName,
        newDisplayName: this.newDisplayName,
        fullName: this.newFullName,
        email: this.newEmail,
        role: this.selectedRole
      };
      this.adminrightsService.updateUserDetails(updatedUser).subscribe(
        () => {
          this.fetchAdmins();
          this.closeModal();
        },
        (error: { status: number; error: { message: string | null; }; }) => {
          if (error.status === 404) {
            this.displayNameError = error.error.message;
          } else {
            this.updateRoleError = 'Not enough privileges to change user details';
          }
        }
      );
    }
  }

  navigateTo(view: string) {
    this.selectedView = view;
  }

  hardDeleteUser() {
    if (this.selectedUser && this.loggedinUserID) {
      const confirmDelete = window.confirm(`Are you sure you want to hard delete user ${this.selectedUser.displayName}? This action cannot be undone.`);
      if (confirmDelete) {
      this.adminrightsService.hardDeleteUser(this.loggedinUserID, this.selectedUser.displayName).subscribe(
        () => {
          this.fetchAdmins();
          this.closeModal();
        },
        (error) => {
          this.updateRoleError = 'Not enough privileges to hard delete user';
        }
      );
      }
    }
  }


  toggleSearchDeleted() {
    this.currentPage = 1;
    this.searchUser();
  }

  undeleteUser(user: User) {
    if (this.loggedinUserID) {
      const confirmUndelete = window.confirm(
        `Are you sure you want to restore user ${user.displayName}?`
      );
      if (confirmUndelete) {
        this.adminrightsService
          .undeleteUser(this.loggedinUserID, user.displayName)
          .subscribe(
            () => {
              this.searchUser(); // Refresh the list after undeleting
            },
            (error) => {
              console.error('Error undeleting user:', error);
              this.updateRoleError = 'Not enough privileges to undelete user';
            }
          );
      }
    }
  }


}
