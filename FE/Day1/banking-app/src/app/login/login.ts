import { Component, signal } from '@angular/core';
import { LoginModel } from '../models/login.model';
import { FormsModule } from '@angular/forms';
import { BankingApiService } from '../services/bankingapi.service';

@Component({
  selector: 'app-login',
  imports: [FormsModule],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class Login {
  loginModel = signal(new LoginModel());
  progress = signal(false);
  constructor(private bankingApiService: BankingApiService) {
  }

  handleLoginClick(){
    this.progress.set(true);
    this.bankingApiService.loginApiCall(this.loginModel()).subscribe({
      next: (response:any) => {
        console.log("Login successful", response);
        sessionStorage.setItem('token', response.token);
        alert("Login successful!")
        this.progress.set(false);
      },
      error: (error) => {
        console.error("Login failed", error);
        alert("Login failed. Please try again.");
        this.progress.set(false);
      }
    });
    
  }

}
