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
  
  constructor(private bankingApiService: BankingApiService) {
  }

  handleLoginClick(){
    console.log("Login button clicked");
    this.bankingApiService.loginApiCall(this.loginModel()).subscribe({
      next: (response) => {
        console.log("Login successful", response);
        alert("Login successful!")
      },
      error: (error) => {
        console.error("Login failed", error);
        alert("Login failed. Please try again.");
      }
    });
  }

}
