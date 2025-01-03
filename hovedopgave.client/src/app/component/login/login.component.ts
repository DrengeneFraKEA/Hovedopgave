import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';

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

  constructor
    (
    private http: HttpClient,
    private route: Router
  ) { }

  onSubmit() {
    this.http.post('https://localhost:7213/login', this.loginData).subscribe(
      (response) => {
        if (response != null) {
          var loginDto = JSON.parse(JSON.stringify(response));

          if (loginDto.error != null)
          {
            this.errorMessage = loginDto.error;
            return;
          }

          // Check if anything is missing.
          if (loginDto.user_id === null || loginDto.user_id === "" || loginDto.token === null || loginDto.token === "") return;

          // Setup login cache
          localStorage.setItem("user_id", loginDto.user_id);
          localStorage.setItem("token", loginDto.token);

          this.route.navigate(['dashboard']);
          this.errorMessage = null;
        } else
        {
          this.errorMessage = "Something went wrong."; 
        }
      },
      (error) => {
        console.error('Login failed', error);
        this.errorMessage = "Login failed. Please try again.";
      }
    );
  }
}
