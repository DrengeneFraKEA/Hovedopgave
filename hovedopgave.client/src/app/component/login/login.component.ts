import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';

interface Login {
  username: string;
  password: string;
}

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  loginData: Login = { username: '', password: '' };
  errorMessage: string | null = null;

  constructor(private http: HttpClient) { }

  onSubmit() {
    this.http.post('https://localhost:7213/login', this.loginData).subscribe(
      (response) => {
        if (response === true) {
          alert("Korrekt!");
          this.errorMessage = null;
        } else {
          this.errorMessage = "User not found!"; 
        }
      },
      (error) => {
        console.error('Login failed', error);
        this.errorMessage = "Login failed. Please try again.";
      }
    );
  }
}
