import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';

interface Login {
  username: string;
  password: string;
}

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {
  loginData: Login = { username: '', password: '' };

  constructor(private http: HttpClient) { }

  onSubmit() {
    this.http.post('https://localhost:7213/login', this.loginData).subscribe(
      (response) => {
        if (response) {
          console.log('Login successful');
        }
      },
      (error) => {
        console.error('Login failed', error);
      }
    );
  }
}
